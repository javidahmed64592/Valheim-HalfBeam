namespace HalfBeams
{
    /// <summary>What to do with a piece's (normally inactive) snow-cap child.</summary>
    internal enum SnowMode
    {
        /// <summary>Straight beams: scale the cap along the beam, exactly as vanilla does for the 1m beam.</summary>
        ScaleLength,

        /// <summary>Poles and diagonals: remove the cap (cosmetic only, revisit later).</summary>
        Remove
    }

    /// <summary>
    /// Describes one new piece derived from a vanilla piece by shortening it.
    /// Adding a new piece = adding one entry in PieceVariantRegistry.
    /// </summary>
    internal sealed class PieceVariantDefinition
    {
        public string SourcePrefabName { get; }
        public string NewPrefabName { get; }
        public string DisplayName { get; }
        public string Description { get; }

        /// <summary>Multiplier applied to the piece's length and to snap point / node positions.</summary>
        public float LengthScale { get; }

        /// <summary>Multiplier applied to the source's resource amounts (rounded up, minimum 1).</summary>
        public float CostRatio { get; }

        public SnowMode Snow { get; }

        public PieceVariantDefinition(
            string sourcePrefabName,
            string newPrefabName,
            string displayName,
            string description,
            float lengthScale,
            SnowMode snow,
            float? costRatio = null)
        {
            SourcePrefabName = sourcePrefabName;
            NewPrefabName = newPrefabName;
            DisplayName = displayName;
            Description = description;
            LengthScale = lengthScale;
            Snow = snow;
            CostRatio = costRatio ?? lengthScale;
        }
    }
}
