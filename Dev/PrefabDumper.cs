using BepInEx;
using Jotunn.Managers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HalfBeams.Dev
{
    /// <summary>
    /// Dev-only helper. Once vanilla prefabs are available it writes a text file to the
    /// BepInEx folder describing every beam/pole-like piece: name, cost, wear-state
    /// children, snap points, mesh bounds and colliders (all in the prefab's root space).
    ///
    /// Hook it up in Plugin.Awake behind a config flag:
    ///     if (_dumpPrefabs.Value) HalfBeams.Dev.PrefabDumper.Register();
    /// </summary>
    internal static class PrefabDumper
    {
        // Edit these to widen/narrow what gets dumped (e.g. add "wall" for the half-wall work later).
        private static readonly string[] NameFilters = { "beam", "pole" };

        // Prefabs that also get a full local-TRS hierarchy dump (which node carries the scale,
        // which are rotated, which components live where). Kept small to keep the file readable.
        private static readonly HashSet<string> HierarchyNames = new HashSet<string>
        {
            "wood_beam", "wood_beam_1", "wood_beam_26", "wood_beam_45", "wood_beam_67",
            "wood_pole", "wood_pole2"
        };

        private const string OutputFileName = "HalfBeams_prefab_dump.txt";

        public static void Register()
        {
            // OnVanillaPrefabsAvailable fires at the main menu, before ZNetScene exists
            // (ZNetScene.instance is null there). OnPrefabsRegistered fires from
            // ZNetScene.Awake, i.e. when a world starts loading, so m_prefabs is populated.
            PrefabManager.OnPrefabsRegistered += Dump;
        }

        private static void Dump()
        {
            if (ZNetScene.instance == null || ZNetScene.instance.m_prefabs == null)
            {
                UnityEngine.Debug.Log("[HalfBeams] ZNetScene not ready yet, skipping dump.");
                return;
            }

            // Only unsubscribe once we know we can dump; the event fires on every world load.
            PrefabManager.OnPrefabsRegistered -= Dump;

            var sb = new StringBuilder();
            List<GameObject> candidates = ZNetScene.instance.m_prefabs
                .Where(p => p != null
                            && p.GetComponent<Piece>() != null
                            && NameFilters.Any(f => p.name.ToLowerInvariant().Contains(f)))
                .OrderBy(p => p.name)
                .ToList();

            sb.AppendLine("== Candidate prefabs (" + candidates.Count + ") ==");
            foreach (GameObject p in candidates)
            {
                sb.AppendLine(p.name + "  ->  " + p.GetComponent<Piece>().m_name);
            }
            sb.AppendLine();

            foreach (GameObject p in candidates)
            {
                try
                {
                    DumpPrefab(p, sb);
                }
                catch (System.Exception e)
                {
                    sb.AppendLine("!!! failed to dump " + p.name + ": " + e);
                    sb.AppendLine();
                }
            }

            string path = Path.Combine(Paths.BepInExRootPath, OutputFileName);
            File.WriteAllText(path, sb.ToString());
            UnityEngine.Debug.Log("[HalfBeams] Prefab dump written to " + path);
        }

        private static void DumpPrefab(GameObject prefab, StringBuilder sb)
        {
            // Instantiate under an inactive parent so ZNetView/Piece Awake never runs,
            // and so we get real transform matrices to work with.
            var holder = new GameObject("HalfBeams_dump_holder");
            holder.SetActive(false);
            try
            {
                GameObject inst = Object.Instantiate(prefab, holder.transform);
                Transform root = inst.transform;

                sb.AppendLine("=== " + prefab.name + " ===");

                Piece piece = inst.GetComponent<Piece>();
                sb.AppendLine("  piece: name=" + piece.m_name + " category=" + piece.m_category);
                if (piece.m_resources != null)
                {
                    foreach (Piece.Requirement r in piece.m_resources)
                    {
                        string item = r.m_resItem != null ? r.m_resItem.name : "?";
                        sb.AppendLine("  cost: " + item + " x" + r.m_amount + " (recover=" + r.m_recover + ")");
                    }
                }

                WearNTear wnt = inst.GetComponent<WearNTear>();
                if (wnt != null)
                {
                    sb.AppendLine("  wearNTear: health=" + wnt.m_health + " supports=" + wnt.m_supports);
                    sb.AppendLine("    m_new=" + NameOf(wnt.m_new)
                                  + " m_worn=" + NameOf(wnt.m_worn)
                                  + " m_broken=" + NameOf(wnt.m_broken)
                                  + " m_wet=" + NameOf(wnt.m_wet));
                }

                // Snap points (tag "snappoint"), with the farthest pair as a length estimate.
                var snaps = new List<Transform>();
                piece.GetSnapPoints(snaps);
                sb.AppendLine("  snapPoints (" + snaps.Count + "):");
                foreach (Transform s in snaps)
                {
                    sb.AppendLine("    " + PathOf(s, root) + " @ " + Fmt(root.InverseTransformPoint(s.position)));
                }
                float maxDist = 0f;
                for (int i = 0; i < snaps.Count; i++)
                {
                    for (int j = i + 1; j < snaps.Count; j++)
                    {
                        float d = Vector3.Distance(snaps[i].position, snaps[j].position);
                        if (d > maxDist) maxDist = d;
                    }
                }
                sb.AppendLine("  farthestSnapPairDistance: " + maxDist.ToString("F3"));

                // Meshes and their bounds in root space.
                bool haveUnion = false;
                Bounds union = default(Bounds);
                foreach (MeshFilter mf in inst.GetComponentsInChildren<MeshFilter>(true))
                {
                    if (mf.sharedMesh == null) continue;
                    Bounds b = BoundsInRoot(mf.sharedMesh, mf.transform, root);
                    if (!haveUnion) { union = b; haveUnion = true; } else { union.Encapsulate(b); }
                    sb.AppendLine("  mesh: " + PathOf(mf.transform, root)
                                  + " mesh=" + mf.sharedMesh.name
                                  + " verts=" + mf.sharedMesh.vertexCount
                                  + " readable=" + mf.sharedMesh.isReadable
                                  + " boundsSize=" + Fmt(b.size)
                                  + " boundsCenter=" + Fmt(b.center));
                }
                if (haveUnion)
                {
                    sb.AppendLine("  meshUnionSize: " + Fmt(union.size) + " center: " + Fmt(union.center));
                }

                // Colliders.
                foreach (Collider c in inst.GetComponentsInChildren<Collider>(true))
                {
                    string line = "  collider: " + PathOf(c.transform, root) + " type=" + c.GetType().Name;
                    var box = c as BoxCollider;
                    var meshCol = c as MeshCollider;
                    if (box != null)
                    {
                        line += " size=" + Fmt(box.size) + " center=" + Fmt(box.center);
                    }
                    else if (meshCol != null && meshCol.sharedMesh != null)
                    {
                        Bounds b = BoundsInRoot(meshCol.sharedMesh, c.transform, root);
                        line += " mesh=" + meshCol.sharedMesh.name + " boundsSize=" + Fmt(b.size);
                    }
                    sb.AppendLine(line);
                }

                if (HierarchyNames.Contains(prefab.name))
                {
                    sb.AppendLine("  hierarchy (local TRS):");
                    foreach (Transform t in inst.GetComponentsInChildren<Transform>(true))
                    {
                        string comps = string.Join(",", t.GetComponents<Component>()
                            .Where(c => c != null)
                            .Select(c => c.GetType().Name)
                            .ToArray());
                        sb.AppendLine("    " + PathOf(t, root)
                                      + " active=" + t.gameObject.activeSelf
                                      + " pos=" + Fmt(t.localPosition)
                                      + " euler=" + Fmt(t.localEulerAngles)
                                      + " scale=" + Fmt(t.localScale)
                                      + " [" + comps + "]");
                    }
                }

                sb.AppendLine();
            }
            finally
            {
                Object.DestroyImmediate(holder);
            }
        }

        private static Bounds BoundsInRoot(Mesh mesh, Transform node, Transform root)
        {
            Matrix4x4 m = root.worldToLocalMatrix * node.localToWorldMatrix;
            Vector3 c = mesh.bounds.center;
            Vector3 e = mesh.bounds.extents;

            Bounds result = default(Bounds);
            for (int i = 0; i < 8; i++)
            {
                var corner = c + new Vector3(
                    (i & 1) == 0 ? -e.x : e.x,
                    (i & 2) == 0 ? -e.y : e.y,
                    (i & 4) == 0 ? -e.z : e.z);
                Vector3 p = m.MultiplyPoint3x4(corner);
                if (i == 0) result = new Bounds(p, Vector3.zero);
                else result.Encapsulate(p);
            }
            return result;
        }

        private static string PathOf(Transform t, Transform root)
        {
            var parts = new List<string>();
            while (t != null && t != root)
            {
                parts.Add(t.name);
                t = t.parent;
            }
            parts.Reverse();
            return parts.Count == 0 ? "<root>" : string.Join("/", parts.ToArray());
        }

        private static string NameOf(GameObject go)
        {
            return go != null ? go.name : "null";
        }

        private static string Fmt(Vector3 v)
        {
            return "(" + v.x.ToString("F3") + ", " + v.y.ToString("F3") + ", " + v.z.ToString("F3") + ")";
        }
    }
}
