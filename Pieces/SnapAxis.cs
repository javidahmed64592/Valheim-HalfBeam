namespace HalfBeam.Pieces;

/// <summary>
/// A local axis of the piece's root, for pieces whose snap points don't show which way the beam runs
/// (the grausten medium pieces are 1m cubes with snap points on every face).
/// </summary>
internal enum SnapAxis
{
    /// <summary>The root's local X axis.</summary>
    X = 0,

    /// <summary>The root's local Y axis (up).</summary>
    Y = 1,

    /// <summary>The root's local Z axis.</summary>
    Z = 2
}
