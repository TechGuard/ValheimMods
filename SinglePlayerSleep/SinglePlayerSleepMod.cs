using BepInEx;
using HarmonyLib;
using System.Collections.Generic;

namespace SinglePlayerSleep
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class SinglePlayerSleepMod : BaseUnityPlugin
    {
        const string NAME = "SinglePlayerSleep";
        const string GUID = "TechGuard.SinglePlayerSleep";
        const string VERSION = "0.0.1";

        readonly Harmony harmony = new Harmony(GUID);

        public void Awake()
        {
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Game), "EverybodyIsTryingToSleep")]
    class Game_EverybodyIsTryingToSleep
    {
        public static bool Prefix(ref bool __result)
        {
            __result = EverybodyIsTryingToSleep();
            return false;
        }

        // Return true if one player is trying to sleep
        static bool EverybodyIsTryingToSleep()
        {
            List<ZDO> characters = ZNet.instance.GetAllCharacterZDOS();
            if (characters.Count == 0)
            {
                return false;
            }

            foreach (ZDO character in characters)
            {
                if (character.GetBool("inBed"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}