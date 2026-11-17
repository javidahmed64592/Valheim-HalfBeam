using System.Collections.Generic;

namespace HalfBeam.Pieces;

/// <summary>
/// The single place to edit when adding pieces. Sources are the shortest vanilla piece
/// of each family, halved (vanilla goes 2m -> 1m; we go 1m -> 0.5m).
/// Pieces with trims or patterns have a <see cref="MeshCut"/>: positions along the snap axis from the centre,
/// found and checked with Valheim-AssetTools (<c>valheim-assets cut &lt;source&gt; --factor &lt;f&gt;</c>).
/// </summary>
internal static class PieceVariantRegistry
{
    private const float Half = 0.5f;
    private const float Quarter = 0.25f;
    private const float Eighth = 0.125f;
    private const float Full = 1f;

    public static readonly IReadOnlyList<PieceVariantDefinition> All =
    [
        // ---------------------------------------------------------------------------------
        // Regular wood
        // ---------------------------------------------------------------------------------

        // Vertical pole: 1m -> 0.5m.
        new PieceVariantDefinition(
            "wood_pole",
            "half_wood_pole",
            "$piece_half_wood_pole",
            "$piece_halfbeam_wood_description",
            Half,
            SnowMode.Remove
        ),

        // Straight beam: 1m -> 0.5m.
        new PieceVariantDefinition(
            "wood_beam_1",
            "half_wood_beam",
            "$piece_half_wood_beam",
            "$piece_halfbeam_wood_description",
            Half,
            SnowMode.ScaleLength
        ),

        // 26 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 0.25m up.
        new PieceVariantDefinition(
            "wood_beam_26",
            "half_wood_beam_26",
            "$piece_half_wood_beam_26",
            "$piece_halfbeam_wood_description",
            Half,
            SnowMode.Remove
        ),

        // 45 degrees: spans 1m across / 1m up in vanilla -> 0.5m across / 0.5m up.
        new PieceVariantDefinition(
            "wood_beam_45",
            "half_wood_beam_45",
            "$piece_half_wood_beam_45",
            "$piece_halfbeam_wood_description",
            Half,
            SnowMode.Remove
        ),

        // 67 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
        new PieceVariantDefinition(
            "wood_beam_67",
            "half_wood_beam_67",
            "$piece_half_wood_beam_67",
            "$piece_halfbeam_wood_description",
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
            "$piece_quarter_darkwood_pole",
            "$piece_halfbeam_darkwood_description",
            Quarter,
            SnowMode.Remove,
            cut: new MeshCut(-0.7505f, 0.7495f)
        ),

        // 2m straight pole -> 1m.
        new PieceVariantDefinition(
            "darkwood_pole",
            "half_darkwood_pole",
            "$piece_half_darkwood_pole",
            "$piece_halfbeam_darkwood_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.5005f, 0.4995f)
        ),

        // 2m straight beam -> 0.5m.
        new PieceVariantDefinition(
            "darkwood_beam",
            "quarter_darkwood_beam",
            "$piece_quarter_darkwood_beam",
            "$piece_halfbeam_darkwood_description",
            Quarter,
            SnowMode.Remove,
            cut: new MeshCut(-0.7505f, 0.7495f)
        ),

        // 2m straight beam -> 1m.
        new PieceVariantDefinition(
            "darkwood_beam",
            "half_darkwood_beam",
            "$piece_half_darkwood_beam",
            "$piece_halfbeam_darkwood_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.5005f, 0.4995f)
        ),

        // 26 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
        new PieceVariantDefinition(
            "darkwood_beam_26",
            "half_darkwood_beam_26",
            "$piece_half_darkwood_beam_26",
            "$piece_halfbeam_darkwood_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.5575f, 0.5605f)
        ),

        // 45 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
        new PieceVariantDefinition(
            "darkwood_beam_45",
            "half_darkwood_beam_45",
            "$piece_half_darkwood_beam_45",
            "$piece_halfbeam_darkwood_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.7052f, 0.7090f)
        ),

        // 67 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
        new PieceVariantDefinition(
            "darkwood_beam_67",
            "half_darkwood_beam_67",
            "$piece_half_darkwood_beam_67",
            "$piece_halfbeam_darkwood_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-1.1176f, 1.1185f)
        ),

        // ---------------------------------------------------------------------------------
        // Ashwood (Blackwood)
        // ---------------------------------------------------------------------------------

        // 1m straight pole -> 0.5m.
        new PieceVariantDefinition(
            "ashwood_pole_1m",
            "half_ashwood_pole",
            "$piece_half_ashwood_pole",
            "$piece_halfbeam_ashwood_description",
            Half,
            SnowMode.Remove
        ),

        // 1m straight beam -> 0.5m.
        new PieceVariantDefinition(
            "ashwood_beam_1m",
            "half_ashwood_beam",
            "$piece_half_ashwood_beam",
            "$piece_halfbeam_ashwood_description",
            Half,
            SnowMode.ScaleLength
        ),

        // 26 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
        new PieceVariantDefinition(
            "ashwood_wall_beam_26",
            "half_ashwood_beam_26",
            "$piece_half_ashwood_beam_26",
            "$piece_halfbeam_ashwood_description",
            Half,
            SnowMode.Remove
        ),

        // 45 degrees: spans 1m across / 1m up in vanilla -> 0.5m across / 0.5m up.
        new PieceVariantDefinition(
            "ashwood_wall_beam_45",
            "half_ashwood_beam_45",
            "$piece_half_ashwood_beam_45",
            "$piece_halfbeam_ashwood_description",
            Half,
            SnowMode.Remove
        ),

        // 67 degrees: spans 1m across / 2m up in vanilla -> 0.5m across / 1m up.
        new PieceVariantDefinition(
            "ashwood_wall_beam_67",
            "half_ashwood_beam_67",
            "$piece_half_ashwood_beam_67",
            "$piece_halfbeam_ashwood_description",
            Half,
            SnowMode.Remove
        ),

        // ---------------------------------------------------------------------------------
        // Iron-reinforced wood (Wood + Iron)
        // ---------------------------------------------------------------------------------

        // 2m straight pole -> 0.5m.
        new PieceVariantDefinition(
            "woodiron_pole",
            "quarter_woodiron_pole",
            "$piece_quarter_woodiron_pole",
            "$piece_halfbeam_woodiron_description",
            Quarter,
            SnowMode.Remove,
            cut: new MeshCut(-0.7480f, 0.7520f)
        ),

        // 2m straight pole -> 1m.
        new PieceVariantDefinition(
            "woodiron_pole",
            "half_woodiron_pole",
            "$piece_half_woodiron_pole",
            "$piece_halfbeam_woodiron_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.5080f, 0.4920f)
        ),

        // 2m straight beam -> 0.5m.
        new PieceVariantDefinition(
            "woodiron_beam",
            "quarter_woodiron_beam",
            "$piece_quarter_woodiron_beam",
            "$piece_halfbeam_woodiron_description",
            Quarter,
            SnowMode.Remove,
            cut: new MeshCut(-0.7501f, 0.7499f)
        ),

        // 2m straight beam -> 1m.
        new PieceVariantDefinition(
            "woodiron_beam",
            "half_woodiron_beam",
            "$piece_half_woodiron_beam",
            "$piece_halfbeam_woodiron_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.4951f, 0.5049f)
        ),

        // 26 degrees: half length.
        new PieceVariantDefinition(
            "woodiron_beam_26",
            "half_woodiron_beam_26",
            "$piece_half_woodiron_beam_26",
            "$piece_halfbeam_woodiron_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.5579f, 0.5601f)
        ),

        // 45 degrees: half length.
        new PieceVariantDefinition(
            "woodiron_beam_45",
            "half_woodiron_beam_45",
            "$piece_half_woodiron_beam_45",
            "$piece_halfbeam_woodiron_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.7069f, 0.7073f)
        ),

        // 67 degrees: half length.
        new PieceVariantDefinition(
            "woodiron_beam_67",
            "half_woodiron_beam_67",
            "$piece_half_woodiron_beam_67",
            "$piece_halfbeam_woodiron_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-1.1167f, 1.1194f)
        ),

        // ---------------------------------------------------------------------------------
        // Timber (Frostwood, Deep North). Scaled, not cut: the plank is slightly barrel-shaped and its
        // grain runs along the whole length, so any cut shows a step and a texture jump at the seam.
        // ---------------------------------------------------------------------------------

        // 2m straight pole -> 0.5m.
        new PieceVariantDefinition(
            "stave_pole_2m",
            "quarter_stave_pole",
            "$piece_quarter_stave_pole",
            "$piece_halfbeam_stave_description",
            Quarter,
            SnowMode.Remove
        ),

        // 2m straight pole -> 1m.
        new PieceVariantDefinition(
            "stave_pole_2m",
            "half_stave_pole",
            "$piece_half_stave_pole",
            "$piece_halfbeam_stave_description",
            Half,
            SnowMode.Remove
        ),

        // 2m straight beam -> 0.5m.
        new PieceVariantDefinition(
            "stave_beam_2m",
            "quarter_stave_beam",
            "$piece_quarter_stave_beam",
            "$piece_halfbeam_stave_description",
            Quarter,
            SnowMode.Remove
        ),

        // 2m straight beam -> 1m.
        new PieceVariantDefinition(
            "stave_beam_2m",
            "half_stave_beam",
            "$piece_half_stave_beam",
            "$piece_halfbeam_stave_description",
            Half,
            SnowMode.Remove
        ),

        // 26 degrees: half length.
        new PieceVariantDefinition(
            "stave_beam_26",
            "half_stave_beam_26",
            "$piece_half_stave_beam_26",
            "$piece_halfbeam_stave_description",
            Half,
            SnowMode.Remove
        ),

        // 45 degrees: half length.
        new PieceVariantDefinition(
            "stave_beam_45",
            "half_stave_beam_45",
            "$piece_half_stave_beam_45",
            "$piece_halfbeam_stave_description",
            Half,
            SnowMode.Remove
        ),

        // 67 degrees: half length.
        new PieceVariantDefinition(
            "stave_beam_67",
            "half_stave_beam_67",
            "$piece_half_stave_beam_67",
            "$piece_halfbeam_stave_description",
            Half,
            SnowMode.Remove
        ),

        // ---------------------------------------------------------------------------------
        // Decorated timber (Frostwood, Deep North). The rings repeat every ~0.33 m, so the quarters
        // remove four whole rings and squash the rest by 25%, and the 26 degree beam removes three and squashes 10.8%
        // ---------------------------------------------------------------------------------

        // 2m straight pole -> 0.5m.
        new PieceVariantDefinition(
            "stave_deco_pole_2m",
            "quarter_stave_deco_pole",
            "$piece_quarter_stave_deco_pole",
            "$piece_halfbeam_stave_deco_description",
            Quarter,
            SnowMode.Remove,
            cut: new MeshCut(-0.6674f, 0.6659f, 0.75f)
        ),

        // 2m straight pole -> 1m.
        new PieceVariantDefinition(
            "stave_deco_pole_2m",
            "half_stave_deco_pole",
            "$piece_half_stave_deco_pole",
            "$piece_halfbeam_stave_deco_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.5024f, 0.4976f)
        ),

        // 2m straight beam -> 0.5m.
        new PieceVariantDefinition(
            "stave_deco_beam_2m",
            "quarter_stave_deco_beam",
            "$piece_quarter_stave_deco_beam",
            "$piece_halfbeam_stave_deco_description",
            Quarter,
            SnowMode.Remove,
            cut: new MeshCut(-0.6664f, 0.6669f, 0.75f)
        ),

        // 2m straight beam -> 1m.
        new PieceVariantDefinition(
            "stave_deco_beam_2m",
            "half_stave_deco_beam",
            "$piece_half_stave_deco_beam",
            "$piece_halfbeam_stave_deco_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.5014f, 0.4986f)
        ),

        // 26 degrees: half length.
        new PieceVariantDefinition(
            "stave_deco_beam_26",
            "half_stave_deco_beam_26",
            "$piece_half_stave_deco_beam_26",
            "$piece_halfbeam_stave_deco_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.4928f, 0.4897f, 0.8919f)
        ),

        // 45 degrees: half length.
        new PieceVariantDefinition(
            "stave_deco_beam_45",
            "half_stave_deco_beam_45",
            "$piece_half_stave_deco_beam_45",
            "$piece_halfbeam_stave_deco_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-0.8453f, 0.5689f)
        ),

        // 67 degrees: half length.
        new PieceVariantDefinition(
            "stave_deco_beam_67",
            "half_stave_deco_beam_67",
            "$piece_half_stave_deco_beam_67",
            "$piece_halfbeam_stave_deco_description",
            Half,
            SnowMode.Remove,
            cut: new MeshCut(-1.1176f, 1.1184f)
        ),

        // ---------------------------------------------------------------------------------
        // Dvergr (Yggdrasil wood + Copper). Vanilla can't build this pole, so a full-length copy is added too.
        // The copper band keeps its real size and only the wood is scaled (the band may not spawn: RandomSpawn)
        // ---------------------------------------------------------------------------------

        // 2m pole, buildable (vanilla has it disabled).
        new PieceVariantDefinition(
            "piece_dvergr_pole",
            "full_dvergr_pole",
            "$piece_full_dvergr_pole",
            "$piece_halfbeam_dvergr_description",
            Full,
            SnowMode.Remove
        ),

        // 2m pole -> 1m, band kept.
        new PieceVariantDefinition(
            "piece_dvergr_pole",
            "half_dvergr_pole",
            "$piece_half_dvergr_pole",
            "$piece_halfbeam_dvergr_description",
            Half,
            SnowMode.Remove,
            keep: new KeepSpan(-0.467f, -0.090f)
        ),

        // 2m pole -> 0.5m, band kept.
        new PieceVariantDefinition(
            "piece_dvergr_pole",
            "quarter_dvergr_pole",
            "$piece_quarter_dvergr_pole",
            "$piece_halfbeam_dvergr_description",
            Quarter,
            SnowMode.Remove,
            keep: new KeepSpan(-0.467f, -0.090f)
        ),

        // ---------------------------------------------------------------------------------
        // Core wood logs (scaled)
        // ---------------------------------------------------------------------------------

        // 2m log pole -> 1m.
        new PieceVariantDefinition(
            "wood_pole_log",
            "half_log_pole",
            "$piece_half_log_pole",
            "$piece_halfbeam_log_description",
            Half,
            SnowMode.Remove
        ),

        // 2m log pole -> 0.5m.
        new PieceVariantDefinition(
            "wood_pole_log",
            "quarter_log_pole",
            "$piece_quarter_log_pole",
            "$piece_halfbeam_log_description",
            Quarter,
            SnowMode.Remove
        ),

        // 2m log beam -> 1m. Keeps all four snap points (top and bottom at each end).
        new PieceVariantDefinition(
            "wood_wall_log",
            "half_log_beam",
            "$piece_half_log_beam",
            "$piece_halfbeam_log_description",
            Half,
            SnowMode.ScaleLength
        ),

        // 2m log beam -> 0.5m.
        new PieceVariantDefinition(
            "wood_wall_log",
            "quarter_log_beam",
            "$piece_quarter_log_beam",
            "$piece_halfbeam_log_description",
            Quarter,
            SnowMode.ScaleLength
        ),

        // 26 degrees: half length.
        new PieceVariantDefinition(
            "wood_log_26",
            "half_log_beam_26",
            "$piece_half_log_beam_26",
            "$piece_halfbeam_log_description",
            Half,
            SnowMode.Remove
        ),

        // 45 degrees: half length.
        new PieceVariantDefinition(
            "wood_log_45",
            "half_log_beam_45",
            "$piece_half_log_beam_45",
            "$piece_halfbeam_log_description",
            Half,
            SnowMode.Remove
        ),

        // 67 degrees: half length.
        new PieceVariantDefinition(
            "wood_log_67",
            "half_log_beam_67",
            "$piece_half_log_beam_67",
            "$piece_halfbeam_log_description",
            Half,
            SnowMode.Remove
        ),

        // 4m worn log pole -> 2m (vanilla only has the 4m worn log).
        new PieceVariantDefinition(
            "wood_pole_log_4_worn",
            "half_worn_log_pole",
            "$piece_half_worn_log_pole",
            "$piece_halfbeam_log_description",
            Half,
            SnowMode.Remove
        ),

        // 4m worn log pole -> 1m.
        new PieceVariantDefinition(
            "wood_pole_log_4_worn",
            "quarter_worn_log_pole",
            "$piece_quarter_worn_log_pole",
            "$piece_halfbeam_log_description",
            Quarter,
            SnowMode.Remove
        ),

        // 4m worn log pole -> 0.5m.
        new PieceVariantDefinition(
            "wood_pole_log_4_worn",
            "eighth_worn_log_pole",
            "$piece_eighth_worn_log_pole",
            "$piece_halfbeam_log_description",
            Eighth,
            SnowMode.Remove
        ),

        // ---------------------------------------------------------------------------------
        // Flametal beams and pillars (scaled; the meshes aren't readable, which scaling doesn't need)
        // ---------------------------------------------------------------------------------

        // 2m beam -> 1m.
        new PieceVariantDefinition(
            "Piece_flametal_beam",
            "half_flametal_beam",
            "$piece_half_flametal_beam",
            "$piece_halfbeam_flametal_description",
            Half,
            SnowMode.Remove
        ),

        // 2m beam -> 0.5m.
        new PieceVariantDefinition(
            "Piece_flametal_beam",
            "quarter_flametal_beam",
            "$piece_quarter_flametal_beam",
            "$piece_halfbeam_flametal_description",
            Quarter,
            SnowMode.Remove
        ),

        // 2m pillar -> 1m.
        new PieceVariantDefinition(
            "Piece_flametal_pillar",
            "half_flametal_pillar",
            "$piece_half_flametal_pillar",
            "$piece_halfbeam_flametal_description",
            Half,
            SnowMode.Remove
        ),

        // 2m pillar -> 0.5m.
        new PieceVariantDefinition(
            "Piece_flametal_pillar",
            "quarter_flametal_pillar",
            "$piece_quarter_flametal_pillar",
            "$piece_halfbeam_flametal_description",
            Quarter,
            SnowMode.Remove
        ),

        // ---------------------------------------------------------------------------------
        // Ashwood alternative diagonals (scaled)
        // ---------------------------------------------------------------------------------

        // 26 degrees (alt): half length.
        new PieceVariantDefinition(
            "ashwood_wall_beam_26_alt",
            "half_ashwood_beam_26_alt",
            "$piece_half_ashwood_beam_26_alt",
            "$piece_halfbeam_ashwood_description",
            Half,
            SnowMode.Remove
        ),

        // 45 degrees (alt): half length.
        new PieceVariantDefinition(
            "ashwood_wall_beam_45_alt",
            "half_ashwood_beam_45_alt",
            "$piece_half_ashwood_beam_45_alt",
            "$piece_halfbeam_ashwood_description",
            Half,
            SnowMode.Remove
        ),
    ];
}
