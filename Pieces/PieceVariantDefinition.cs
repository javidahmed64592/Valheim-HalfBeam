namespace HalfBeam.Pieces;

/// <summary>What to do with a piece's (normally inactive) snow-cap child.</summary>
internal enum SnowMode
{
    /// <summary>Straight beams: scale the cap along the beam, exactly as vanilla does for the 1m beam.</summary>
    ScaleLength = 0,

    /// <summary>Poles and diagonals: remove the cap (cosmetic only, revisit later).</summary>
    Remove = 1
}

/// <summary>
/// Describes one new piece derived from a vanilla piece by shortening it.
/// Adding a new piece = adding one entry in PieceVariantRegistry.
/// </summary>
internal sealed class PieceVariantDefinition(
    string sourcePrefabName,
    string newPrefabName,
    string displayName,
    string description,
    float lengthScale,
    SnowMode snow,
    float? costRatio = null,
    MeshCut cut = null,
    KeepSpan keep = null)
{
    public string SourcePrefabName { get; } = sourcePrefabName;
    public string NewPrefabName { get; } = newPrefabName;
    public string DisplayName { get; } = displayName;
    public string Description { get; } = description;

    /// <summary>Multiplier applied to the piece's length and to snap point / node positions.</summary>
    public float LengthScale { get; } = lengthScale;

    /// <summary>Multiplier applied to the source's resource amounts (rounded up, minimum 1).</summary>
    public float CostRatio { get; } = costRatio ?? lengthScale;

    public SnowMode Snow { get; } = snow;

    /// <summary>
    /// When set, the visual meshes (WearNTear's new/worn/broken) are shortened by cutting this slab out,
    /// keeping trims and patterns at their real size, instead of being scaled along the beam.
    /// </summary>
    public MeshCut Cut { get; } = cut;

    /// <summary>
    /// When set (and <see cref="Cut"/> isn't), the visual meshes are scaled along the beam everywhere except
    /// this span, which keeps its real size. Nothing is removed, so there's no seam.
    /// </summary>
    public KeepSpan Keep { get; } = keep;
}
