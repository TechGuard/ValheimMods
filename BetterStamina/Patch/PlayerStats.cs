using HarmonyLib;

namespace BetterStamina.Patch
{
    [HarmonyPatch(typeof(Player), "Awake")]
    class Player_Awake
    {
        public static void Postfix(Player __instance)
        {
            __instance.m_runStaminaDrain = 0f;
            __instance.m_sneakStaminaDrain = 0f;
            __instance.m_dodgeStaminaUsage = 0f;
            __instance.m_jumpStaminaUsage = 0f;

            __instance.m_staminaRegenDelay = 0.2f;
        }
    }

    [HarmonyPatch(typeof(ItemDrop), "Awake")]
    class ItemDrop_Awake
    {
        public static void Postfix(ItemDrop __instance)
        {
            var shared = __instance.m_itemData.m_shared;
            switch (shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.Misc:
                case ItemDrop.ItemData.ItemType.Utility:
                case ItemDrop.ItemData.ItemType.Tool:
                    shared.m_attack.m_attackStamina = 0f;
                    shared.m_secondaryAttack.m_attackStamina = 0f;
                    break;
            }
            switch (shared.m_skillType)
            {
                case Skills.SkillType.Axes:
                case Skills.SkillType.Pickaxes:
                    shared.m_attack.m_attackStamina = 0f;
                    shared.m_secondaryAttack.m_attackStamina = 0f;
                    break;
            }
        }
    }
}