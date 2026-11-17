using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

namespace HalfBeam.Pieces;

/// <summary>
/// Shortens a beam mesh by removing the slab between two planes across the beam and closing the gap.
/// Triangles crossing a plane are clipped to it; triangles inside the slab are dropped; everything past the
/// slab slides back by its length. The result is then (optionally) squashed along the beam and moved to where
/// the shortened piece sits, matching how <see cref="PieceVariantFactory"/> maps the snap points.
/// Mirrors <c>valheim_assettools/cut.py</c>, where the cut positions are chosen and checked.
/// </summary>
/// <param name="center">Centre between the snap points, in the prefab root's space.</param>
/// <param name="axis">Unit direction from the first snap point to the second.</param>
/// <param name="length">Distance between the snap points.</param>
/// <param name="factor">New length as a fraction of the old.</param>
/// <param name="cut">The slab to remove.</param>
internal sealed class MeshCutter(Vector3 center, Vector3 axis, float length, float factor, MeshCut cut)
{
    private const float SideEpsilon = 1e-6f;
    private const int MaxUvChannels = 8;

    private readonly Vector3 _center = center;
    private readonly Vector3 _axis = axis;
    private readonly MeshCut _cut = cut;
    private readonly Vector3 _firstEnd = center - (axis * (length / 2f));
    private readonly Vector3 _offset = ((factor - 1f) * center) + (axis * (length * (1f - factor) / 2f));
    private readonly Dictionary<(Mesh, Matrix4x4), Mesh> _cache = [];

    /// <summary>
    /// Returns a shortened copy of a mesh drawn by a node whose transform to the prefab root is
    /// <paramref name="nodeToRoot"/>. Copies are cached, so meshes shared by several nodes are cut once.
    /// </summary>
    public Mesh Cut(Mesh source, Matrix4x4 nodeToRoot)
    {
        if (_cache.TryGetValue((source, nodeToRoot), out Mesh cached))
        {
            return cached;
        }

        Mesh result = Build(source, nodeToRoot);
        _cache[(source, nodeToRoot)] = result;
        return result;
    }

    private Mesh Build(Mesh source, Matrix4x4 nodeToRoot)
    {
        VertexData data = VertexData.Read(source);
        Matrix4x4 rootToNode = nodeToRoot.inverse;

        int count = data.Positions.Count;
        List<float> along = new(count);
        for (int i = 0; i < count; i++)
        {
            along.Add(Vector3.Dot(nodeToRoot.MultiplyPoint3x4(data.Positions[i]) - _center, _axis));
        }

        VertexData output = data.EmptyCopy();
        List<bool> shifted = [];
        Dictionary<int, int> reused = [];
        List<List<int>> submeshes = [];

        int AddOriginal(int index, bool right)
        {
            if (!reused.TryGetValue(index, out int mapped))
            {
                mapped = output.AddFrom(data, index);
                shifted.Add(right);
                reused[index] = mapped;
            }

            return mapped;
        }

        for (int sub = 0; sub < source.subMeshCount; sub++)
        {
            int[] triangles = source.GetTriangles(sub);
            List<int> kept = [];
            for (int i = 0; i + 2 < triangles.Length; i += 3)
            {
                int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
                float ta = along[a], tb = along[b], tc = along[c];
                bool left = ta <= _cut.Start + SideEpsilon && tb <= _cut.Start + SideEpsilon && tc <= _cut.Start + SideEpsilon;
                bool right = ta >= _cut.End - SideEpsilon && tb >= _cut.End - SideEpsilon && tc >= _cut.End - SideEpsilon;
                if (left || right)
                {
                    kept.Add(AddOriginal(a, right));
                    kept.Add(AddOriginal(b, right));
                    kept.Add(AddOriginal(c, right));
                    continue;
                }

                // Each side is clipped from the original triangle, so every corner lies on an original edge.
                List<Corner> corners = [new(a, ta), new(b, tb), new(c, tc)];
                AddPolygon(Clip(corners, _cut.Start, keepBelow: true), output, shifted, kept, right: false);
                AddPolygon(Clip(corners, _cut.End, keepBelow: false), output, shifted, kept, right: true);
            }

            submeshes.Add(kept);
        }

        Place(output, shifted, nodeToRoot, rootToNode);
        return output.ToMesh(source, submeshes);
    }

    /// <summary>A polygon corner: an original vertex, or a point on the edge between two original vertices.</summary>
    private readonly struct Corner(int from, float along, int to = -1, float share = 0f)
    {
        public readonly int From = from;
        public readonly int To = to;
        public readonly float Share = share;
        public readonly float Along = along;
    }

    /// <summary>Clips a polygon to one side of a plane across the beam (Sutherland–Hodgman).</summary>
    private static List<Corner> Clip(List<Corner> polygon, float limit, bool keepBelow)
    {
        bool Inside(Corner corner)
        {
            return keepBelow ? corner.Along <= limit + SideEpsilon : corner.Along >= limit - SideEpsilon;
        }

        List<Corner> result = [];
        for (int i = 0; i < polygon.Count; i++)
        {
            Corner current = polygon[i];
            Corner previous = polygon[(i + polygon.Count - 1) % polygon.Count];
            if (Inside(current) != Inside(previous))
            {
                float share = (limit - previous.Along) / (current.Along - previous.Along);
                result.Add(new Corner(previous.From, limit, current.From, share));
            }

            if (Inside(current))
            {
                result.Add(current);
            }
        }

        // A polygon lying flat on the plane has no area on the kept side.
        bool flat = result.TrueForAll(corner => Mathf.Abs(corner.Along - limit) <= SideEpsilon);
        return flat ? [] : result;
    }

    private static void AddPolygon(List<Corner> polygon, VertexData output, List<bool> shifted, List<int> kept, bool right)
    {
        if (polygon.Count < 3)
        {
            return;
        }

        List<int> indices = [];
        foreach (Corner corner in polygon)
        {
            indices.Add(corner.To < 0
                ? output.AddFrom(output.Source, corner.From)
                : output.AddBetween(output.Source, corner.From, corner.To, corner.Share));
            shifted.Add(right);
        }

        for (int i = 1; i + 1 < indices.Count; i++)
        {
            kept.Add(indices[0]);
            kept.Add(indices[i]);
            kept.Add(indices[i + 1]);
        }
    }

    /// <summary>Slides the far side back, squashes if needed, moves to the shortened position and back to node space.</summary>
    private void Place(VertexData output, List<bool> shifted, Matrix4x4 nodeToRoot, Matrix4x4 rootToNode)
    {
        // The squash is linear in root space; in node space it's rootToNode * squash * nodeToRoot.
        Matrix4x4 squash = Matrix4x4.identity;
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                squash[row, column] -= (1f - _cut.Scale) * _axis[row] * _axis[column];
            }
        }

        Matrix4x4 nodeSquash = rootToNode * squash * nodeToRoot;
        Matrix4x4 normalMatrix = nodeSquash.inverse.transpose;

        for (int i = 0; i < output.Positions.Count; i++)
        {
            Vector3 point = nodeToRoot.MultiplyPoint3x4(output.Positions[i]);
            if (shifted[i])
            {
                point -= _axis * _cut.Length;
            }

            float fromEnd = Vector3.Dot(point - _firstEnd, _axis);
            point -= _axis * (fromEnd * (1f - _cut.Scale));
            output.Positions[i] = rootToNode.MultiplyPoint3x4(point + _offset);
        }

        if (Mathf.Approximately(_cut.Scale, 1f))
        {
            return;
        }

        for (int i = 0; i < output.Normals.Count; i++)
        {
            output.Normals[i] = normalMatrix.MultiplyVector(output.Normals[i]).normalized;
        }

        for (int i = 0; i < output.Tangents.Count; i++)
        {
            Vector4 tangent = output.Tangents[i];
            Vector3 direction = nodeSquash.MultiplyVector(tangent).normalized;
            output.Tangents[i] = new Vector4(direction.x, direction.y, direction.z, tangent.w);
        }
    }

    /// <summary>Per-vertex attributes of a mesh, read once and written back to a new mesh.</summary>
    private sealed class VertexData
    {
        public VertexData Source { get; private set; }
        public List<Vector3> Positions { get; } = [];
        public List<Vector3> Normals { get; } = [];
        public List<Vector4> Tangents { get; } = [];
        public List<Color> Colors { get; } = [];
        public List<Vector4>[] Uvs { get; } = new List<Vector4>[MaxUvChannels];
        public int[] UvDimensions { get; } = new int[MaxUvChannels];

        public static VertexData Read(Mesh mesh)
        {
            VertexData data = new();
            mesh.GetVertices(data.Positions);
            mesh.GetNormals(data.Normals);
            mesh.GetTangents(data.Tangents);
            mesh.GetColors(data.Colors);
            for (int channel = 0; channel < MaxUvChannels; channel++)
            {
                VertexAttribute attribute = VertexAttribute.TexCoord0 + channel;
                if (!mesh.HasVertexAttribute(attribute))
                {
                    continue;
                }

                data.UvDimensions[channel] = mesh.GetVertexAttributeDimension(attribute);
                data.Uvs[channel] = [];
                mesh.GetUVs(channel, data.Uvs[channel]);
            }

            return data;
        }

        public VertexData EmptyCopy()
        {
            VertexData copy = new() { Source = this };
            for (int channel = 0; channel < MaxUvChannels; channel++)
            {
                if (Uvs[channel] != null)
                {
                    copy.Uvs[channel] = [];
                    copy.UvDimensions[channel] = UvDimensions[channel];
                }
            }

            return copy;
        }

        public int AddFrom(VertexData from, int index)
        {
            return AddBetween(from, index, index, 0f);
        }

        public int AddBetween(VertexData from, int a, int b, float share)
        {
            Positions.Add(Vector3.Lerp(from.Positions[a], from.Positions[b], share));
            if (from.Normals.Count > 0)
            {
                Normals.Add(Vector3.Lerp(from.Normals[a], from.Normals[b], share).normalized);
            }

            if (from.Tangents.Count > 0)
            {
                Vector4 tangent = Vector4.Lerp(from.Tangents[a], from.Tangents[b], share);
                Vector3 direction = ((Vector3)tangent).normalized;
                Tangents.Add(new Vector4(direction.x, direction.y, direction.z, from.Tangents[a].w));
            }

            if (from.Colors.Count > 0)
            {
                Colors.Add(Color.Lerp(from.Colors[a], from.Colors[b], share));
            }

            for (int channel = 0; channel < MaxUvChannels; channel++)
            {
                if (from.Uvs[channel] != null)
                {
                    Uvs[channel].Add(Vector4.Lerp(from.Uvs[channel][a], from.Uvs[channel][b], share));
                }
            }

            return Positions.Count - 1;
        }

        public Mesh ToMesh(Mesh source, List<List<int>> submeshes)
        {
            Mesh mesh = new()
            {
                name = source.name + "_halfbeam",
                indexFormat = Positions.Count > ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16,
            };
            mesh.SetVertices(Positions);
            if (Normals.Count > 0)
            {
                mesh.SetNormals(Normals);
            }

            if (Tangents.Count > 0)
            {
                mesh.SetTangents(Tangents);
            }

            if (Colors.Count > 0)
            {
                mesh.SetColors(Colors);
            }

            for (int channel = 0; channel < MaxUvChannels; channel++)
            {
                if (Uvs[channel] != null)
                {
                    SetUvs(mesh, channel, Uvs[channel], UvDimensions[channel]);
                }
            }

            mesh.subMeshCount = submeshes.Count;
            for (int sub = 0; sub < submeshes.Count; sub++)
            {
                mesh.SetTriangles(submeshes[sub], sub);
            }

            mesh.RecalculateBounds();
            return mesh;
        }

        private static void SetUvs(Mesh mesh, int channel, List<Vector4> uvs, int dimension)
        {
            switch (dimension)
            {
                case 2:
                    mesh.SetUVs(channel, uvs.ConvertAll(uv => new Vector2(uv.x, uv.y)));
                    break;
                case 3:
                    mesh.SetUVs(channel, uvs.ConvertAll(uv => new Vector3(uv.x, uv.y, uv.z)));
                    break;
                default:
                    mesh.SetUVs(channel, uvs);
                    break;
            }
        }
    }
}
