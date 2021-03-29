using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace HammerMod.Patch
{
    [HarmonyPatch(typeof(WearNTear), "Highlight")]
    class WearNTear_Highlight
    {
#pragma warning disable CS0649
        public struct OldMeshData
        {
            public Renderer m_renderer;
            public Material[] m_materials;
            public Color[] m_color;
            public Color[] m_emissiveColor;
        }
#pragma warning restore CS0649

        public static void Postfix(List<OldMeshData> ___m_oldMaterials)
        {
            if (!Hammer.TryRemoveHold || !Hammer.InPlaceMode || !Hammer.CanRemovePieces)
            {
                return;
            }

            float remove_progress = Interpolate.Ease(Interpolate.EaseType.EaseOutCubic)(0.0f, 1.0f, Hammer.RemoveHoldTimer, HammerMod.RemoveHoldDelay);

            float H = 0.0f;
            float S = Mathf.Lerp(0.4f, 1f, remove_progress);
            float V = Mathf.Lerp(0.8f, 1.2f, remove_progress);
            Color color = Color.HSVToRGB(H, S, V);

            foreach (var oldMaterial in ___m_oldMaterials)
            {
                if (oldMaterial.m_renderer)
                {
                    var materials = oldMaterial.m_renderer.materials;
                    foreach (Material obj in materials)
                    {
                        obj.SetColor("_EmissionColor", color * 0.4f);
                        obj.color = color;
                    }
                }
            }
        }
    }
}