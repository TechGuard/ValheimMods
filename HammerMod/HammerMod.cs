using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HammerMod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("valheim.exe")]
    public class HammerMod : BaseUnityPlugin
    {
        const string NAME = "HammerMod";
        const string GUID = "TechGuard.HammerMod";
        const string VERSION = "0.0.1";

        readonly Harmony harmony = new Harmony(GUID);

        public static float RemoveHoldDelay { get { return ConfigRemoveHoldDelayMs.Value / 1000f; } }
        static ConfigEntry<int> ConfigRemoveHoldDelayMs;

        public static KeyCode PickBuildPieceKeyCode
        {
            get
            {
                if (System.Enum.TryParse(ConfigPickBuildPieceKeyCode.Value, out KeyCode keycode))
                {
                    return keycode;
                }
                return KeyCode.Mouse2;
            }
        }
        static ConfigEntry<string> ConfigPickBuildPieceKeyCode;

        public void Awake()
        {
            ConfigRemoveHoldDelayMs = Config.Bind("Input", "RemoveHoldDelay", 250,
                "Time (in milliseconds) that the player needs to hold the 'Remove' button.");

            ConfigPickBuildPieceKeyCode = Config.Bind("Input", "PickBuildPieceKeyCode", "Mouse2",
                "Keybinding for picking a build piece (Mouse2 = middle mouse button).\n" +
                "Available KeyCodes can be found here: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            harmony.PatchAll();
        }
    }
}