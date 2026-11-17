namespace HalfBeam.Pieces;

/// <summary>
/// A slab to cut out of a piece's mesh instead of scaling it, so trims and patterns keep their size.
/// Positions are metres along the beam axis (from the first snap point towards the second), measured from
/// the centre between the snap points. Values come from Valheim-AssetTools: <c>valheim-assets cut</c>.
/// </summary>
internal sealed class MeshCut(float start, float end, float scale = 1f)
{
    /// <summary>Where the removed slab starts.</summary>
    public float Start { get; } = start;

    /// <summary>Where the removed slab ends.</summary>
    public float End { get; } = end;

    /// <summary>
    /// Squash applied to the rest of the piece along its length when the slab is shorter than the length to
    /// remove (to keep whole pattern repeats). 1 means no squash.
    /// </summary>
    public float Scale { get; } = scale;

    /// <summary>The removed length.</summary>
    public float Length => End - Start;
}
