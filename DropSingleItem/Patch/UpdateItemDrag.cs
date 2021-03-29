using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "UpdateItemDrag")]
    class InventoryGui_UpdateItemDrag
    {
        // Prevent UpdateItemDrag from clearing ItemDrag when the right mouse button is held down.
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var GetMouseButtonMethod = typeof(Input).GetMethod("GetMouseButton", new System.Type[] { typeof(int) });

            var codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; ++i)
            {
                // Find: Input.GetMouseButton(1)
                // IL_013f: ldc.i4.1
                // IL_0140: call bool[UnityEngine.InputLegacyModule] UnityEngine.Input::GetMouseButton(int32)
                if (codes[i - 1].opcode == OpCodes.Ldc_I4_1 && codes[i].Calls(GetMouseButtonMethod))
                {
                    // Replace with constant value "false"
                    // IL_013f: nop
                    // IL_0140: ldc.i4.s 0
                    codes[i - 1] = new CodeInstruction(OpCodes.Nop);
                    codes[i] = new CodeInstruction(OpCodes.Ldc_I4_S, 0);
                }
            }
            return codes.AsEnumerable();
        }
    }
}