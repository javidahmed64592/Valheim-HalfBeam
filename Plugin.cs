using BepInEx;
using BepInEx.Configuration;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using UnityEngine;

namespace HalfBeams
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class HalfBeamPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "javidahmed64592.halfbeam";
        public const string PluginName = "HalfBeam";
        public const string PluginVersion = "0.1.0";

        private ConfigEntry<bool> _dumpPrefabs;
        private ConfigEntry<bool> _diagnosePieces;
        private bool _knownRecipesRefreshed;
        private bool _diagnosed;
        private bool _diagnosedEquipped;
        private float _equippedTimer;

        private void Awake()
        {
            _dumpPrefabs = Config.Bind(
                "Dev",
                "Dump Prefabs",
                false,
                "Write beam/pole prefab data to the BepInEx folder (development only).");

            _diagnosePieces = Config.Bind(
                "Dev",
                "Diagnose Pieces",
                true,
                "Log whether the half pieces are in the Hammer table and available to the player (development only).");

            if (_dumpPrefabs.Value)
            {
                Dev.PrefabDumper.Register();
            }

            // Fires at the main menu once vanilla prefabs can be cloned.
            PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
        }

        private void Update()
        {
            if (Player.m_localPlayer == null || ObjectDB.instance == null)
            {
                // Reset per-session state so it runs again on the next world load.
                _knownRecipesRefreshed = false;
                _diagnosed = false;
                _diagnosedEquipped = false;
                _equippedTimer = 0f;
                return;
            }

            // Force a recipe-list refresh once per session so Jotunn-registered
            // pieces (added before the player spawns) are added to m_knownRecipes.
            // Without this, PieceTable.UpdateAvailable skips them because it filters
            // on m_knownRecipes.Contains(piece.m_name).
            if (!_knownRecipesRefreshed)
            {
                _knownRecipesRefreshed = true;
                Player.m_localPlayer.UpdateKnownRecipesList();
            }

            if (!_diagnosePieces.Value)
            {
                return;
            }

            // Stage 1: once, as soon as a player exists in a loaded world.
            if (!_diagnosed)
            {
                _diagnosed = true;
                Dev.PieceDiagnostics.Run(Logger);
                return;
            }

            // Stage 2: once, a moment after a build tool (the hammer) is equipped, when the game has
            // built the list of pieces actually shown in the menu.
            if (_diagnosedEquipped)
            {
                return;
            }

            ItemDrop.ItemData right = Player.m_localPlayer.GetRightItem();
            if (right != null && right.m_shared.m_buildPieces != null)
            {
                _equippedTimer += Time.deltaTime;
                if (_equippedTimer > 1f)
                {
                    _diagnosedEquipped = true;
                    Dev.PieceDiagnostics.RunEquipped(Logger, right.m_shared.m_buildPieces);
                }
            }
            else
            {
                _equippedTimer = 0f;
            }
        }

        private void RegisterPieces()
        {
            // The event fires on every menu start; only build once.
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;

            foreach (PieceVariantDefinition definition in PieceVariantRegistry.All)
            {
                try
                {
                    PieceVariantFactory.Build(definition, Logger);
                }
                catch (Exception e)
                {
                    Logger.LogError("Failed to build " + definition.NewPrefabName + ": " + e);
                }
            }
        }
    }
}
