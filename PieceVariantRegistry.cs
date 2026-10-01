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
        private const float Quarter = 0.25f;

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

            // 67 degrees: spans 2 across / 4 up in vanilla -> 1 across / 2 up.
            // Baked meshes, so this one relies on the factory's pivot path.
            new PieceVariantDefinition(
                "wood_beam_67", "half_wood_beam_67",
                "Wood beam 67\u00B0 (half)", "A half-length wooden beam at 67\u00B0.",
                Half, SnowMode.Remove),

            // ---------------------------------------------------------------------------------
            // Ashwood (Blackwood). Same pattern as wood: 1m/2m straight pieces exist, so the
            // straight ones halve the 1m piece; diagonals halve the vanilla diagonal.
            // ---------------------------------------------------------------------------------
            new PieceVariantDefinition(
                "ashwood_beam_1m", "half_ashwood_beam",
                "Ashwood beam 0.5m", "A half-length ashwood beam.",
                Half, SnowMode.ScaleLength),

            new PieceVariantDefinition(
                "ashwood_pole_1m", "half_ashwood_pole",
                "Ashwood pole 0.5m", "A half-length ashwood pole.",
                Half, SnowMode.Remove),

            new PieceVariantDefinition(
                "ashwood_wall_beam_26", "half_ashwood_beam_26",
                "Ashwood beam 26\u00B0 (half)", "A half-length ashwood beam at 26\u00B0.",
                Half, SnowMode.Remove),

            new PieceVariantDefinition(
                "ashwood_wall_beam_45", "half_ashwood_beam_45",
                "Ashwood beam 45\u00B0 (half)", "A half-length ashwood beam at 45\u00B0.",
                Half, SnowMode.Remove),

            new PieceVariantDefinition(
                "ashwood_wall_beam_67", "half_ashwood_beam_67",
                "Ashwood beam 67\u00B0 (half)", "A half-length ashwood beam at 67\u00B0.",
                Half, SnowMode.Remove),

            // ---------------------------------------------------------------------------------
            // Darkwood (Wood + Tar). Vanilla only has 2m straight pieces, so to get the same
            // 0.5m / 1m / 2m set as wood we add both a 1m (x0.5) and a 0.5m (x0.25) piece.
            // Diagonals halve the vanilla diagonal, like wood.
            // ---------------------------------------------------------------------------------
            new PieceVariantDefinition(
                "darkwood_beam", "half_darkwood_beam",
                "Darkwood beam 1m", "A 1m darkwood beam.",
                Half, SnowMode.Remove),

            new PieceVariantDefinition(
                "darkwood_beam", "quarter_darkwood_beam",
                "Darkwood beam 0.5m", "A half-metre darkwood beam.",
                Quarter, SnowMode.Remove),

            new PieceVariantDefinition(
                "darkwood_pole", "half_darkwood_pole",
                "Darkwood pole 1m", "A 1m darkwood pole.",
                Half, SnowMode.Remove),

            new PieceVariantDefinition(
                "darkwood_pole", "quarter_darkwood_pole",
                "Darkwood pole 0.5m", "A half-metre darkwood pole.",
                Quarter, SnowMode.Remove),

            new PieceVariantDefinition(
                "darkwood_beam_26", "half_darkwood_beam_26",
                "Darkwood beam 26\u00B0 (half)", "A half-length darkwood beam at 26\u00B0.",
                Half, SnowMode.Remove),

            new PieceVariantDefinition(
                "darkwood_beam_45", "half_darkwood_beam_45",
                "Darkwood beam 45\u00B0 (half)", "A half-length darkwood beam at 45\u00B0.",
                Half, SnowMode.Remove),

            new PieceVariantDefinition(
                "darkwood_beam_67", "half_darkwood_beam_67",
                "Darkwood beam 67\u00B0 (half)", "A half-length darkwood beam at 67\u00B0.",
                Half, SnowMode.Remove),
        };
    }
}
