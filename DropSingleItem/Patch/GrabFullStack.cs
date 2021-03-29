using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGrid), "OnLeftClick")]
    class InventoryGrid_OnLeftClick
    {
        const float DoubleClickThreshold = 0.2f;

        static float LastItemClickedTime = 0f;

        public struct State
        {
            public bool DoubleClicked;
        }

        public static bool Prefix(ref State __state)
        {
            __state.DoubleClicked = (Time.unscaledTime - LastItemClickedTime) < DoubleClickThreshold;
            LastItemClickedTime = Time.unscaledTime;

            var __instance = InventoryGui.instance;
            if (__state.DoubleClicked && __instance.dragGo())
            {
                __state.DoubleClicked = false;
                GrabFullStackItemDrag(__instance);
                return false;
            }
            return true;
        }

        public static void Postfix(State __state)
        {
            var __instance = InventoryGui.instance;
            if (__state.DoubleClicked && __instance.dragGo())
            {
                GrabFullStackItemDrag(__instance);
            }
        }

        static void GrabFullStackItemDrag(InventoryGui __instance)
        {
            var drag_amount = __instance.dragAmount();

            // 1st: Try open container
            var currentContainer = __instance.currentContainer();
            if (currentContainer != null)
            {
                GrabFullStackItemDrag(__instance, currentContainer.GetInventory());
            }

            // 2nd: Player inventory
            GrabFullStackItemDrag(__instance, Player.m_localPlayer.GetInventory());

            // SetupDragItem
            var dragItem = __instance.dragItem();
            if (drag_amount != dragItem.m_stack)
            {
                __instance.moveItemEffects().Create(__instance.transform.position, Quaternion.identity);
                __instance.SetupDragItem(dragItem, __instance.dragInventory(), dragItem.m_stack);
                __instance.UpdateCraftingPanel();
            }
        }

        static void GrabFullStackItemDrag(InventoryGui __instance, Inventory inventory)
        {
            var dragItem = __instance.dragItem();
            var dragInventory = __instance.dragInventory();

            List<ItemDrop.ItemData> items = new List<ItemDrop.ItemData>();
            inventory.GetAllItems(dragItem.m_shared.m_name, items);

            while (items.Any() && dragItem.m_stack < dragItem.m_shared.m_maxStackSize)
            {
                var item = items.Last();
                items.RemoveAt(items.Count - 1);

                if (Utils.CanStackItem(dragItem, item))
                {
                    dragInventory.MoveItemToThis(inventory, item, item.m_stack, dragItem.m_gridPos.x, dragItem.m_gridPos.y);
                }
            }
        }
    }
}