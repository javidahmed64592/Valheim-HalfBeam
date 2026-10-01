using BepInEx;
using Jotunn.Managers;
using Jotunn.Utils;
using System;

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

        private bool _knownRecipesRefreshed;

        private void Awake()
        {
            // Fires at the main menu once vanilla prefabs can be cloned.
            PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
        }

        private void Update()
        {
            if (Player.m_localPlayer == null || ObjectDB.instance == null)
            {
                // Reset per-session state so it runs again on the next world load.
                _knownRecipesRefreshed = false;
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
