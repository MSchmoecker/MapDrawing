using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;

namespace MapDrawing {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    // [BepInDependency(Jotunn.Main.ModGuid)]
    internal class MapDrawing : BaseUnityPlugin {
        public const string PluginGUID = "com.jotunn.jotunnmodstub";
        public const string PluginName = "MapDrawing";
        public const string PluginVersion = "0.0.1";

        public static MapDrawing Instance { get; private set; }

        public static ManualLogSource Log => Instance.Logger;

        private void Awake() {
            Instance = this;

            Harmony harmony = new Harmony(PluginGUID);
            harmony.PatchAll();
        }
    }
}
