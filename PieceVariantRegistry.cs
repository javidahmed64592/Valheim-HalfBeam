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
            // ---------------------------------------------------------------------------------
            // Regular wood
            // ---------------------------------------------------------------------------------

            // Vertical pole: 1m -> 0.5m.
            new PieceVariantDefinition(
                "wood_pole",
                "half_wood_pole",
                "Wood Pole 0.5 m",
                "A half-length sturdy wooden support.",
                Half,
                SnowMode.Remove
            ),

            // Straight beam: 1m -> 0.5m.
            new PieceVariantDefinition(
                "wood_beam_1",
                "half_wood_beam",
                "Wood Beam 0.5 m",
                "A half-length sturdy wooden support.",
                Half,
                SnowMode.ScaleLength
            ),

            // 26 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 0.25m up.
            new PieceVariantDefinition(
                "wood_beam_26",
                "half_wood_beam_26",
                "Wood Beam 26\u00B0 (Half)",
                "A half-length sturdy wooden support.",
                Half,
                SnowMode.Remove
            ),

            // 45 degrees: spans 1m across / 1m up in vanilla -> 0.5m across / 0.5m up.
            new PieceVariantDefinition(
                "wood_beam_45",
                "half_wood_beam_45",
                "Wood Beam 45\u00B0 (Half)",
                "A half-length sturdy wooden support.",
                Half,
                SnowMode.Remove
            ),

            // 67 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
            new PieceVariantDefinition(
                "wood_beam_67",
                "half_wood_beam_67",
                "Wood Beam 67\u00B0 (Half)",
                "A half-length sturdy wooden support.",
                Half,
                SnowMode.Remove
            ),

            // ---------------------------------------------------------------------------------
            // Darkwood (Wood + Tar)
            // ---------------------------------------------------------------------------------

            // 2m straight pole -> 0.5m.
            new PieceVariantDefinition(
                "darkwood_pole",
                "quarter_darkwood_pole",
                "Darkwood Pole 0.5 m",
                "Intricate designs run along this half-length support structure.",
                Quarter,
                SnowMode.Remove
            ),

            // 2m straight pole -> 1m.
            new PieceVariantDefinition(
                "darkwood_pole",
                "half_darkwood_pole",
                "Darkwood Pole 1 m",
                "Intricate designs run along this half-length support structure.",
                Half,
                SnowMode.Remove
            ),

            // 2m straight beam -> 0.5m.
            new PieceVariantDefinition(
                "darkwood_beam",
                "quarter_darkwood_beam",
                "Darkwood Beam 0.5 m",
                "Intricate designs run along this half-length support structure.",
                Quarter,
                SnowMode.Remove
            ),

            // 2m straight beam -> 1m.
            new PieceVariantDefinition(
                "darkwood_beam",
                "half_darkwood_beam",
                "Darkwood Beam 1 m",
                "Intricate designs run along this half-length support structure.",
                Half,
                SnowMode.Remove
            ),

            // 26 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
            new PieceVariantDefinition(
                "darkwood_beam_26",
                "half_darkwood_beam_26",
                "Darkwood Beam 26\u00B0 (Half)",
                "Intricate designs run along this half-length support structure.",
                Half,
                SnowMode.Remove
            ),

            // 45 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
            new PieceVariantDefinition(
                "darkwood_beam_45",
                "half_darkwood_beam_45",
                "Darkwood Beam 45\u00B0 (Half)",
                "Intricate designs run along this half-length support structure.",
                Half,
                SnowMode.Remove
            ),

            // 67 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
            new PieceVariantDefinition(
                "darkwood_beam_67",
                "half_darkwood_beam_67",
                "Darkwood Beam 67\u00B0 (Half)",
                "Intricate designs run along this half-length support structure.",
                Half,
                SnowMode.Remove
            ),

            // ---------------------------------------------------------------------------------
            // Ashwood (Blackwood)
            // ---------------------------------------------------------------------------------

            // 1m straight pole -> 0.5m.
            new PieceVariantDefinition(
                "ashwood_pole_1m",
                "half_ashwood_pole",
                "Ashwood Pole 0.5 m",
                "These half-length supports are always warm to the touch.",
                Half,
                SnowMode.Remove
            ),

            // 1m straight beam -> 0.5m.
            new PieceVariantDefinition(
                "ashwood_beam_1m",
                "half_ashwood_beam",
                "Ashwood Beam 0.5 m",
                "These half-length supports are always warm to the touch.",
                Half,
                SnowMode.ScaleLength
            ),

            // 26 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
            new PieceVariantDefinition(
                "ashwood_wall_beam_26",
                "half_ashwood_beam_26",
                "Ashwood Beam 26\u00B0 (Half)",
                "These half-length supports are always warm to the touch.",
                Half,
                SnowMode.Remove
            ),

            // 45 degrees: spans 1m across / 1m up in vanilla -> 0.5m across / 0.5m up.
            new PieceVariantDefinition(
                "ashwood_wall_beam_45",
                "half_ashwood_beam_45",
                "Ashwood Beam 45\u00B0 (Half)",
                "These half-length supports are always warm to the touch.",
                Half,
                SnowMode.Remove
            ),

            // 67 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
            new PieceVariantDefinition(
                "ashwood_wall_beam_67",
                "half_ashwood_beam_67",
                "Ashwood Beam 67\u00B0 (Half)",
                "These half-length supports are always warm to the touch.",
                Half,
                SnowMode.Remove
            ),
        };
    }
}
