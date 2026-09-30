using System.Collections.Generic;

namespace HalfBeams
{
    /// <summary>
    /// The single place to edit when adding pieces. Sources are the shortest vanilla piece
    /// of each family, halved (vanilla goes 2m -> 1m; we go 1m -> 0.5m).
    /// </summary>
    internal static class PieceVariantRegistry
    {
        private const float Half = 0.5f;

        public static readonly IReadOnlyList<PieceVariantDefinition> All = new List<PieceVariantDefinition>
        {
            // Straight beam: 1m -> 0.5m.
            new PieceVariantDefinition(
                "wood_beam_1", "half_wood_beam",
                "Wood beam 0.5m", "A half-length wooden beam.",
                Half, SnowMode.ScaleLength),

            // Vertical pole: 1m -> 0.5m.
            new PieceVariantDefinition(
                "wood_pole", "half_wood_pole",
                "Wood pole 0.5m", "A half-length wooden pole.",
                Half, SnowMode.Remove),

            // 26 degrees: spans 2 across / 1 up in vanilla -> 1 across / 0.5 up.
            new PieceVariantDefinition(
                "wood_beam_26", "half_wood_beam_26",
                "Wood beam 26\u00B0 (half)", "A half-length wooden beam at 26\u00B0.",
                Half, SnowMode.Remove),

            // 45 degrees: spans 2 across / 2 up in vanilla -> 1 across / 1 up.
            new PieceVariantDefinition(
                "wood_beam_45", "half_wood_beam_45",
                "Wood beam 45\u00B0 (half)", "A half-length wooden beam at 45\u00B0.",
                Half, SnowMode.Remove),
        };
    }
}
