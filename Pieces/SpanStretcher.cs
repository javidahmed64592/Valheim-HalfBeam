using System.Collections.Generic;

using UnityEngine;

namespace HalfBeam.Pieces;

/// <summary>
/// Shortens a mesh by scaling it along the beam everywhere except a kept span, which only moves.
/// Nothing is removed, so there's no seam: used where the wood's grain runs the whole length but one
/// feature (a band) must keep its size. Mirrors <c>keep_positions</c> in <c>valheim_assettools/cut.py</c>.
/// </summary>
/// <param name="center">Centre between the snap points, in the prefab root's space.</param>
/// <param name="axis">Unit direction along the beam.</param>
/// <param name="length">The beam's length between its snap points.</param>
/// <param name="factor">New length as a fraction of the old.</param>
/// <param name="keep">The span that keeps its size.</param>
internal sealed class SpanStretcher(Vector3 center, Vector3 axis, float length, float factor, KeepSpan keep)
{
    private const float SideEpsilon = 1e-4f;

    private readonly Vector3 _center = center;
    private readonly Vector3 _axis = axis;
    private readonly float _length = length;
    private readonly float _factor = factor;
    private readonly KeepSpan _keep = keep;
    private readonly float _scale = ((length * factor) - keep.Length) / (length - keep.Length);
    private readonly Dictionary<(Mesh, Matrix4x4), Mesh> _cache = [];

    /// <summary>Whether the kept span fits in the new length at all.</summary>
    public bool Fits => _scale > 0f;

    /// <summary>
    /// Returns a stretched copy of a mesh drawn by a node whose transform to the prefab root is
    /// <paramref name="nodeToRoot"/>. Copies are cached, so meshes shared by several nodes are done once.
    /// </summary>
    public Mesh Stretch(Mesh source, Matrix4x4 nodeToRoot)
    {
        if (_cache.TryGetValue((source, nodeToRoot), out Mesh cached))
        {
            return cached;
        }

        Mesh mesh = Object.Instantiate(source);
        mesh.name = source.name + "_halfbeam";
        Matrix4x4 rootToNode = nodeToRoot.inverse;

        // Outside the span, geometry is squashed along the beam; its normals need the inverse transpose.
        Matrix4x4 squash = Matrix4x4.identity;
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                squash[row, column] -= (1f - _scale) * _axis[row] * _axis[column];
            }
        }

        Matrix4x4 nodeSquash = rootToNode * squash * nodeToRoot;
        Matrix4x4 normalMatrix = nodeSquash.inverse.transpose;

        List<Vector3> vertices = [];
        List<Vector3> normals = [];
        List<Vector4> tangents = [];
        mesh.GetVertices(vertices);
        mesh.GetNormals(normals);
        mesh.GetTangents(tangents);

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 point = nodeToRoot.MultiplyPoint3x4(vertices[i]);
            float along = Vector3.Dot(point - _center, _axis);
            point = (_factor * _center) + (point - _center) + (_axis * (Map(along) - along));
            vertices[i] = rootToNode.MultiplyPoint3x4(point);

            if (along > _keep.Start + SideEpsilon && along < _keep.End - SideEpsilon)
            {
                continue;
            }

            if (i < normals.Count)
            {
                normals[i] = normalMatrix.MultiplyVector(normals[i]).normalized;
            }

            if (i < tangents.Count)
            {
                Vector4 tangent = tangents[i];
                Vector3 direction = nodeSquash.MultiplyVector(tangent).normalized;
                tangents[i] = new Vector4(direction.x, direction.y, direction.z, tangent.w);
            }
        }

        mesh.SetVertices(vertices);
        if (normals.Count > 0)
        {
            mesh.SetNormals(normals);
        }

        if (tangents.Count > 0)
        {
            mesh.SetTangents(tangents);
        }

        mesh.RecalculateBounds();
        _cache[(source, nodeToRoot)] = mesh;
        return mesh;
    }

    /// <summary>Where a position along the beam (from its centre) ends up in the shortened piece.</summary>
    private float Map(float along)
    {
        float start = -_length * _factor / 2f;
        float before = start + (_scale * (Mathf.Min(along, _keep.Start) + (_length / 2f)));
        float inside = Mathf.Clamp(along, _keep.Start, _keep.End) - _keep.Start;
        float after = _scale * (Mathf.Max(along, _keep.End) - _keep.End);
        return before + inside + after;
    }
}
