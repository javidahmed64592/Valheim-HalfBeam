namespace HalfBeam.Pieces;

/// <summary>
/// A span along the beam that keeps its real size while the rest of the visual meshes is scaled to fit,
/// for pieces with one feature to protect (the dvergr pole's copper band) on wood that can't be cut cleanly.
/// Positions are metres along the beam axis (from the first snap point towards the second), measured from
/// the centre between the snap points. Values come from Valheim-AssetTools: <c>valheim-assets cut --keep</c>.
/// </summary>
internal sealed class KeepSpan(float start, float end)
{
    /// <summary>Where the kept span starts.</summary>
    public float Start { get; } = start;

    /// <summary>Where the kept span ends.</summary>
    public float End { get; } = end;

    /// <summary>The kept length.</summary>
    public float Length => End - Start;
}
