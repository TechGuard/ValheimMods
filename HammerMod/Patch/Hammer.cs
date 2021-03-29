using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace HammerMod.Patch
{
    [HarmonyPatch(typeof(Player), "UpdatePlacement")]
    class Hammer
    {
        public static bool TryRemoveHold = false;
        public static bool RemoveHoldActivated = false;
        public static float RemoveHoldTimer = 0.0f;
        public static Piece LastHoveringPiece = null;

        public static Vector2Int? PrevSelectedIndex = null;

        public static bool InPlaceMode = false;
        public static bool CanRemovePieces = false;

        public static void Prefix(Player __instance, PieceTable ___m_buildPieces, ref float ___m_lastToolUseTime)
        {
            InPlaceMode = __instance.InPlaceMode();
            CanRemovePieces = __instance.GetRightItem()?.m_shared.m_buildPieces?.m_canRemovePieces ?? false;

            // No delay for remove/place.
            ___m_lastToolUseTime = 0f;

            Piece hovering_piece = __instance.GetHoveringPiece();
            UpdateRemoveHoldInput(hovering_piece);

            bool can_pick_piece = !Hud.IsPieceSelectionVisible() && InPlaceMode;
            if (can_pick_piece && hovering_piece && ZInput.GetButtonUp("PickBuildPiece"))
            {
                if (FindPieceInTable(___m_buildPieces, hovering_piece, out var category_idx, out var piece_idx))
                {
                    if ((int)___m_buildPieces.m_selectedCategory != category_idx)
                    {
                        __instance.SetBuildCategory(category_idx);
                    }
                    __instance.SetSelectedPiece(piece_idx);
                }
                else
                {
                    __instance.SetSelectedPiece(new Vector2Int(0, 0));
                }
                PrevSelectedIndex = null;
            }
            
            if (PrevSelectedIndex == null && ZInput.GetButton("PickBuildPiece"))
            {
                if (___m_buildPieces)
                {
                    // Equip repair mode
                    PrevSelectedIndex = ___m_buildPieces.GetSelectedIndex();
                    __instance.SetSelectedPiece(new Vector2Int(0, 0));
                }
            }
            else if (PrevSelectedIndex != null && !ZInput.GetButton("PickBuildPiece"))
            {
                // Equip previous piece
                __instance.SetSelectedPiece((Vector2Int)PrevSelectedIndex);
                PrevSelectedIndex = null;
            }
        }

        static void UpdateRemoveHoldInput(Piece hovering_piece)
        {
            if (LastHoveringPiece != hovering_piece)
            {
                LastHoveringPiece = hovering_piece;
                RemoveHoldTimer = 0.0f;
            }

            // Try to remove on hold when Pick and Remove are the same keybinding
            bool same_input = ZInput.GetButton("PickBuildPiece") && ZInput.GetButton("Remove");
            if (same_input)
            {
                if (!hovering_piece)
                {
                    return;
                }

                if (TryRemoveHold)
                {
                    RemoveHoldTimer += Time.unscaledDeltaTime;
                }
                else
                {
                    TryRemoveHold = true;
                    RemoveHoldActivated = false;
                    RemoveHoldTimer = 0.0f;
                }

                if (RemoveHoldTimer >= HammerMod.RemoveHoldDelay)
                {
                    RemoveHoldActivated = true;
                    RemoveHoldTimer = 0.0f;
                    GetButtonDef("Remove").m_down = true;
                }
                else
                {
                    GetButtonDef("Remove").m_down = false;
                }
            }
            else if (TryRemoveHold)
            {
                TryRemoveHold = false;
                if (RemoveHoldActivated)
                {
                    GetButtonDef("PickBuildPiece").m_up = false;
                }
            }
        }

        static bool FindPieceInTable(PieceTable piece_table, Piece piece_to_find, out int found_category_idx, out Vector2Int found_piece_idx)
        {
            found_category_idx = -1;
            found_piece_idx = new Vector2Int();

            if (!piece_table)
            {
                return false;
            }

            // Beautiful linear search
            var available_pieces = Traverse.Create(piece_table).Field("m_availablePieces").GetValue<List<List<Piece>>>();
            for (int category_idx = 0; category_idx < available_pieces.Count; ++category_idx)
            {
                var pieces = available_pieces[category_idx];
                for (int piece_idx = 0; piece_idx < pieces.Count; ++piece_idx)
                {
                    if (IsSamePieceType(pieces[piece_idx], piece_to_find))
                    {
                        found_category_idx = category_idx;
                        found_piece_idx.x = piece_idx % PieceTable.m_gridWidth;
                        found_piece_idx.y = piece_idx / PieceTable.m_gridWidth;
                        return true;
                    }
                }
            }
            return false;
        }

        static bool IsSamePieceType(Piece a, Piece b)
        {
            return a.m_name == b.m_name;
        }

        static ZInput.ButtonDef GetButtonDef(string name)
        {
            TryGetButtonDef(name, out var button);
            return button;
        }

        static bool TryGetButtonDef(string name, out ZInput.ButtonDef button)
        {
            var buttons = Traverse.Create(ZInput.instance).Field("m_buttons").GetValue<Dictionary<string, ZInput.ButtonDef>>();
            return buttons.TryGetValue(name, out button);
        }
    }
}