using BepInEx;
using HarmonyLib;

namespace BetterStamina
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("valheim.exe")]
    public class BetterStaminaMod : BaseUnityPlugin
    {
        const string NAME = "BetterStamina";
        const string GUID = "TechGuard.BetterStamina";
        const string VERSION = "0.0.1";

        readonly Harmony harmony = new Harmony(GUID);

        public void Awake()
        {
            harmony.PatchAll();
        }
    }
}