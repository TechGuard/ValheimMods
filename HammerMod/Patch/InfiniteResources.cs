using HarmonyLib;

namespace HammerMod.Patch
{
    [HarmonyPatch(typeof(Player), "ConsumeResources")]
    class Player_ConsumeResources
    {
        const bool Enabled = false;

        public static bool Prefix()
        {
            return !Enabled;
        }
    }
}
