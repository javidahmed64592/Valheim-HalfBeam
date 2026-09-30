using BepInEx.Logging;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HalfBeams
{
    internal static class PieceVariantFactory
    {
        // A local axis counts as "along the beam" when it is within ~3.5 degrees of the beam direction.
        private const float AxisAlignmentThreshold = 0.998f;
        private const float IdentityTolerance = 0.001f;

        /// <summary>
        /// Geometry of a two-snap-point piece (beam or pole) in its root's space. Shortening by
        /// <see cref="Scale"/> keeps the piece's thickness but moves the ends as if the whole piece
        /// were scaled about the root origin, so ends stay on the 0.5m grid.
        /// </summary>
        private struct BeamFrame
        {
            public readonly Vector3 Center;
            public readonly Vector3 Axis;
            public readonly float Scale;

            public BeamFrame(Vector3 center, Vector3 axis, float scale)
            {
                Center = center;
                Axis = axis;
                Scale = scale;
            }

            /// <summary>Scale a vector along the beam axis only.</summary>
            public Vector3 Directional(Vector3 v)
            {
                return v - (1f - Scale) * Vector3.Dot(v, Axis) * Axis;
            }

            /// <summary>Where a point of the original piece ends up on the shortened piece.</summary>
            public Vector3 MapPoint(Vector3 p)
            {
                return Scale * Center + Directional(p - Center);
            }
        }

        public static void Build(PieceVariantDefinition def, ManualLogSource log)
        {
            GameObject source = PrefabManager.Instance.GetPrefab(def.SourcePrefabName);
            if (source == null)
            {
                log.LogError("Source prefab not found: " + def.SourcePrefabName);
                return;
            }

            Piece sourcePiece = source.GetComponent<Piece>();

            var sourceSnaps = new List<Transform>();
            sourcePiece.GetSnapPoints(sourceSnaps);
            if (sourceSnaps.Count != 2)
            {
                log.LogError(def.SourcePrefabName + ": expected exactly 2 snap points (beam or pole), found "
                             + sourceSnaps.Count + ". Skipped.");
                return;
            }

            GameObject clone = PrefabManager.Instance.CreateClonedPrefab(def.NewPrefabName, source);

            Resize(clone, def, log);

            var config = new PieceConfig
            {
                Name = def.DisplayName,
                Description = def.Description,
                PieceTable = "Hammer",
                Category = sourcePiece.m_category.ToString(),
                CraftingStation = sourcePiece.m_craftingStation != null ? sourcePiece.m_craftingStation.name : "",
                Requirements = sourcePiece.m_resources
                    .Select(r => new RequirementConfig
                    {
                        Item = r.m_resItem.name,
                        Amount = Math.Max(1, (int)Math.Ceiling(r.m_amount * def.CostRatio)),
                        Recover = r.m_recover
                    })
                    .ToArray()
            };

            PieceManager.Instance.AddPiece(new CustomPiece(clone, false, config));
            log.LogInfo("Registered " + def.NewPrefabName + " (from " + def.SourcePrefabName + ")");
        }

        private static void Resize(GameObject clone, PieceVariantDefinition def, ManualLogSource log)
        {
            float f = def.LengthScale;
            Transform root = clone.transform;
            Piece piece = clone.GetComponent<Piece>();

            var snaps = new List<Transform>();
            piece.GetSnapPoints(snaps);

            Vector3 a = snaps[0].localPosition;
            Vector3 b = snaps[1].localPosition;
            var frame = new BeamFrame((a + b) * 0.5f, (b - a).normalized, f);
            float before = (b - a).magnitude;

            // Snap points sit on the beam's centre line, so this is the same as halving their positions.
            foreach (Transform snap in snaps)
            {
                snap.localPosition = frame.MapPoint(snap.localPosition);
            }

            Transform pivot = null;
            int wrapped = 0;

            foreach (Transform child in root.Cast<Transform>().ToList())
            {
                if (snaps.Contains(child))
                {
                    continue;
                }

                if (child.name.IndexOf("snow", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    HandleSnow(child, def);
                    continue;
                }

                bool isCollider = child.GetComponent<Collider>() != null;
                bool isMesh = child.GetComponent<MeshFilter>() != null;

                // Case 1: a node that carries the length in its own scale (vanilla resizes one shared
                // cube mesh this way, and box colliders work the same). If one of the node's local axes
                // runs along the beam, scaling that axis is exact and needs no readable meshes.
                if (isCollider || isMesh)
                {
                    int axis;
                    if (TryFindAlignedAxis(child, frame.Axis, out axis))
                    {
                        child.localPosition = frame.MapPoint(child.localPosition);
                        child.localScale = ScaleAxis(child.localScale, axis, f);
                        continue;
                    }

                    if (isCollider)
                    {
                        log.LogWarning(def.NewPrefabName + ": collider '" + child.name
                                       + "' has no axis along the beam; left unchanged.");
                        continue;
                    }
                }

                if (child.GetComponentInChildren<Renderer>(true) == null)
                {
                    continue;
                }

                // Case 2: baked geometry (the mesh itself is angled, node transform is identity).
                // Put it under a pivot that scales along the beam axis only.
                if (!IsIdentity(child))
                {
                    log.LogWarning(def.NewPrefabName + ": '" + child.name
                                   + "' has a non-identity transform and baked geometry; left unchanged.");
                    continue;
                }

                if (pivot == null)
                {
                    pivot = CreatePivot(root, frame);
                }

                Wrap(child, pivot, frame);
                wrapped++;
            }

            // LODGroups cache their bounds from the old renderer sizes.
            foreach (LODGroup lod in clone.GetComponentsInChildren<LODGroup>(true))
            {
                lod.RecalculateBounds();
            }

            log.LogInfo(def.NewPrefabName + ": snap distance " + before.ToString("F3")
                        + " -> " + (snaps[0].localPosition - snaps[1].localPosition).magnitude.ToString("F3")
                        + " (expected " + (before * f).ToString("F3") + ")"
                        + (wrapped > 0 ? ", " + wrapped + " baked node(s) scaled via pivot" : ""));
        }

        // Finds the node's local axis (0=x, 1=y, 2=z) that points along the beam, if any.
        private static bool TryFindAlignedAxis(Transform node, Vector3 beamAxis, out int axisIndex)
        {
            Quaternion rot = node.localRotation;
            Vector3[] axes = { rot * Vector3.right, rot * Vector3.up, rot * Vector3.forward };

            axisIndex = 0;
            float best = 0f;
            for (int i = 0; i < axes.Length; i++)
            {
                float alignment = Mathf.Abs(Vector3.Dot(axes[i], beamAxis));
                if (alignment > best)
                {
                    best = alignment;
                    axisIndex = i;
                }
            }

            return best >= AxisAlignmentThreshold;
        }

        private static Vector3 ScaleAxis(Vector3 scale, int axis, float f)
        {
            scale[axis] *= f;
            return scale;
        }

        private static bool IsIdentity(Transform t)
        {
            return t.localPosition.sqrMagnitude < IdentityTolerance
                   && Quaternion.Angle(t.localRotation, Quaternion.identity) < 0.01f
                   && (t.localScale - Vector3.one).sqrMagnitude < IdentityTolerance;
        }

        // Pivot whose local Z runs along the beam and is scaled by f. Children are counter-rotated
        // so that the net effect is a plain scale along the beam axis (no shear of the mesh itself).
        private static Transform CreatePivot(Transform root, BeamFrame frame)
        {
            var go = new GameObject("HalfBeams_pivot");
            go.transform.SetParent(root, false);
            go.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, frame.Axis);
            go.transform.localPosition = frame.Scale * frame.Center;
            go.transform.localScale = new Vector3(1f, 1f, frame.Scale);
            return go.transform;
        }

        private static void Wrap(Transform node, Transform pivot, BeamFrame frame)
        {
            Quaternion inverse = Quaternion.Inverse(pivot.localRotation);
            node.SetParent(pivot, false);
            node.localRotation = inverse;
            node.localPosition = -(inverse * frame.Center);
        }

        private static void HandleSnow(Transform node, PieceVariantDefinition def)
        {
            if (def.Snow == SnowMode.ScaleLength)
            {
                Vector3 p = node.localPosition;
                p.x *= def.LengthScale;
                node.localPosition = p;

                Vector3 s = node.localScale;
                s.x *= def.LengthScale;
                node.localScale = s;
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(node.gameObject);
            }
        }
    }
}
