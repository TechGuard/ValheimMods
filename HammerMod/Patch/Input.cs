using HarmonyLib;

namespace HammerMod.Patch
{
    [HarmonyPatch(typeof(ZInput), "Reset")]
    class Input_Reset
    {
        public static void Postfix(ZInput __instance)
        {
            __instance.AddButton("PickBuildPiece", HammerMod.PickBuildPieceKeyCode);
        }
    }
}
