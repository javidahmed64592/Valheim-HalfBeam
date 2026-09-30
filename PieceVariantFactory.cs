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
        public static void Build(PieceVariantDefinition def, ManualLogSource log)
        {
            GameObject source = PrefabManager.Instance.GetPrefab(def.SourcePrefabName);
            if (source == null)
            {
                log.LogError("Source prefab not found: " + def.SourcePrefabName);
                return;
            }

            Piece sourcePiece = source.GetComponent<Piece>();
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

            // Snap points: uniformly halving positions keeps ends on the 0.5m grid for
            // straight and diagonal pieces alike (e.g. 45deg 2x2 span -> 1x1).
            var snaps = new List<Transform>();
            piece.GetSnapPoints(snaps);
            float before = FarthestDistance(snaps);
            foreach (Transform snap in snaps)
            {
                snap.localPosition *= f;
            }

            // Mesh and collider nodes carry the length in their localScale (vanilla resizes one shared
            // cube mesh this way), so scaling the longest axis handles beams, poles and diagonals,
            // and needs no readable meshes.
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

                if (child.GetComponent<MeshFilter>() != null || child.GetComponent<Collider>() != null)
                {
                    child.localPosition *= f;
                    child.localScale = ScaleLongestAxis(child.localScale, f);
                }
            }

            // The LODGroup caches its bounds from the old renderer sizes.
            LODGroup lod = clone.GetComponent<LODGroup>();
            if (lod != null)
            {
                lod.RecalculateBounds();
            }

            log.LogInfo(def.NewPrefabName + ": snap distance " + before.ToString("F3")
                        + " -> " + FarthestDistance(snaps).ToString("F3")
                        + " (expected " + (before * f).ToString("F3") + ")");
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

        // Multiplies whichever scale component is largest (the beam's length axis).
        private static Vector3 ScaleLongestAxis(Vector3 scale, float f)
        {
            float ax = Mathf.Abs(scale.x);
            float ay = Mathf.Abs(scale.y);
            float az = Mathf.Abs(scale.z);

            if (ax >= ay && ax >= az) scale.x *= f;
            else if (ay >= az) scale.y *= f;
            else scale.z *= f;

            return scale;
        }

        private static float FarthestDistance(List<Transform> points)
        {
            float max = 0f;
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    max = Mathf.Max(max, Vector3.Distance(points[i].localPosition, points[j].localPosition));
                }
            }
            return max;
        }
    }
}
