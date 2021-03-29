using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "Awake")]
    class InventoryGui_Awake
    {
        public static void Postfix(InventoryGui __instance, Button ___m_dropButton)
        {
            ___m_dropButton.gameObject.AddComponent<RightClickButtonComponent>().OnRightClick.AddListener(() => OnDropSingleOutside(__instance));
        }

        static void OnDropSingleOutside(InventoryGui __instance)
        {
            if (__instance.dragGo())
            {
                var dragItem = __instance.dragItem();
                var dragInventory = __instance.dragInventory();

                var dragAmount = __instance.dragAmount();
                var dragStack = dragItem.m_stack;

                ZLog.Log("Drop single item " + dragItem.m_shared.m_name);

                if (!dragInventory.ContainsItem(dragItem))
                {
                    __instance.SetupDragItem(null, null, 1);
                }
                else if (Player.m_localPlayer.DropItem(dragInventory, dragItem, 1))
                {
                    __instance.moveItemEffects().Create(__instance.transform.position, Quaternion.identity);

                    // SetupDragItem to reflect the new amount.
                    dragAmount -= dragStack - dragItem.m_stack;
                    if (dragInventory.ContainsItem(dragItem) && dragAmount > 0)
                    {
                        __instance.SetupDragItem(dragItem, dragInventory, dragAmount);
                    }
                    else
                    {
                        __instance.SetupDragItem(null, null, 1);
                    }

                    __instance.UpdateCraftingPanel();
                }
            }
        }
    }
}