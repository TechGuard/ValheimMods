using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EnableCheats
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class EnableCheatsMod : BaseUnityPlugin
    {
        const string NAME = "EnableCheats";
        const string GUID = "TechGuard.EnableCheats";
        const string VERSION = "0.0.1";

        readonly Harmony harmony = new Harmony(GUID);

        public void Awake()
        {
            harmony.PatchAll();

            // Enable console by default
            Console.SetConsoleEnabled(true);
        }
    }

    [HarmonyPatch(typeof(Console), "IsCheatsEnabled")]
    class Console_IsCheatsEnabled
    {
        public static bool Prefix(ref bool __result, bool ___m_cheat)
        {
            __result = ___m_cheat;
            return false;
        }
    }

    [HarmonyPatch(typeof(Console), "InputText")]
    class Console_InputText
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var IsServerMethod = typeof(ZNet).GetMethod("IsServer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var codes = new List<CodeInstruction>(instructions);
            for (var i = 2; i < codes.Count; ++i)
            {
                // Find: !ZNet.instance.IsServer()
                // IL_05a9: call class ZNet ZNet::get_instance()
                // IL_05ae: callvirt instance bool ZNet::IsServer()
                // IL_05b3: brfalse IL_112b
                if (codes[i].opcode == OpCodes.Brfalse && codes[i - 1].Calls(IsServerMethod))
                {
                    // Replace with Nop
                    codes[i] = new CodeInstruction(OpCodes.Nop);
                    codes[i - 1] = new CodeInstruction(OpCodes.Nop);
                    codes[i - 2] = new CodeInstruction(OpCodes.Nop);
                }
            }
            return codes.AsEnumerable();
        }
    }
}