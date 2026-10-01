using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HalfBeams.Dev
{
    /// <summary>
    /// Dev-only. Once the local player exists, logs for each half piece (and a vanilla control,
    /// wood_beam_1) whether it made it into the Hammer's piece table and whether the game
    /// considers it available to this character, and why not.
    /// </summary>
    internal static class PieceDiagnostics
    {
        private const string ControlPrefab = "wood_beam_1";

        public static void Run(ManualLogSource log)
        {
            Player player = Player.m_localPlayer;
            GameObject hammer = ObjectDB.instance.GetItemPrefab("Hammer");
            if (hammer == null)
            {
                log.LogWarning("[diag] Hammer prefab not found in ObjectDB");
                return;
            }

            PieceTable table = hammer.GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;
            if (table == null)
            {
                log.LogWarning("[diag] Hammer has no build piece table");
                return;
            }

            log.LogInfo("[diag] Hammer table '" + table.name + "' has " + table.m_pieces.Count + " pieces");

            var names = new List<string> { ControlPrefab };
            names.AddRange(PieceVariantRegistry.All.Select(d => d.NewPrefabName));
            foreach (string name in names)
            {
                Diagnose(name, table, player, log);
            }
        }

        /// <summary>
        /// Logs what the game put in the equipped build table's available-piece set,
        /// and whether the player has each piece in its known-recipes set.
        /// </summary>
        public static void RunEquipped(ManualLogSource log, PieceTable table)
        {
            Player player = Player.m_localPlayer;
            log.LogInfo("[diag] build tool equipped: table='" + table.name
                        + "' availablePieces=" + table.m_availablePieces.Count
                        + " hideUnavailable=" + Player.m_hideUnavailable
                        + " noPlacementCost=" + player.m_noPlacementCost
                        + " knownRecipes=" + player.m_knownRecipes.Count);

            var names = new List<string> { ControlPrefab };
            names.AddRange(PieceVariantRegistry.All.Select(d => d.NewPrefabName));
            foreach (string name in names)
            {
                GameObject go = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab(name) : null;
                Piece piece = go != null ? go.GetComponent<Piece>() : null;
                if (piece == null)
                {
                    log.LogInfo("[diag] " + name + ": prefab/piece missing");
                    continue;
                }

                log.LogInfo("[diag] " + name
                            + ": knownRecipe=" + player.m_knownRecipes.Contains(piece.m_name)
                            + " inAvailablePieces=" + table.m_availablePieces.Contains(piece));
            }
        }

        private static void Diagnose(string prefabName, PieceTable table, Player player, ManualLogSource log)
        {
            GameObject go = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab(prefabName) : null;
            if (go == null)
            {
                log.LogInfo("[diag] " + prefabName + ": NOT found in ZNetScene");
                return;
            }

            Piece piece = go.GetComponent<Piece>();
            int index = table.m_pieces.IndexOf(go);

            var sb = new StringBuilder();
            sb.Append("[diag] " + prefabName
                      + ": inHammerTable=" + (index >= 0) + (index >= 0 ? " (index " + index + ")" : "")
                      + " enabled=" + piece.m_enabled
                      + " category=" + piece.m_category
                      + " name='" + piece.m_name + "'");

            if (piece.m_craftingStation != null)
            {
                string stationName = piece.m_craftingStation.m_name;
                sb.Append(" station=" + piece.m_craftingStation.name + "/" + stationName
                          + " stationKnown=" + player.m_knownStations.ContainsKey(stationName));
            }
            else
            {
                sb.Append(" station=none");
            }

            foreach (Piece.Requirement r in piece.m_resources)
            {
                if (r.m_resItem == null)
                {
                    sb.Append(" req=<null item>");
                    continue;
                }

                string shared = (r.m_resItem.m_itemData != null && r.m_resItem.m_itemData.m_shared != null)
                    ? r.m_resItem.m_itemData.m_shared.m_name
                    : "<no shared>";
                sb.Append(" req=" + r.m_resItem.name + "/" + shared + " x" + r.m_amount
                          + " known=" + player.m_knownMaterial.Contains(shared));
            }

            sb.Append(" isKnown=" + player.HaveRequirements(piece, Player.RequirementMode.IsKnown));
            log.LogInfo(sb.ToString());
        }
    }
}
