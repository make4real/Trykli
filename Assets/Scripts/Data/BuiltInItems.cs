using System.Collections.Generic;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>
    /// Default definitions of the placeable items described in the GDD (section 13).
    /// Used to generate the ItemDefinition assets and as a runtime fallback when they do not exist.
    /// </summary>
    public static class BuiltInItems
    {
        public const string Spring = "spring";
        public const string Ramp = "ramp";
        public const string Fan = "fan";
        public const string PortalAB = "portal_ab";
        public const string PortalCD = "portal_cd";
        public const string Magnet = "magnet";
        public const string Bomb = "bomb";
        public const string Bumper = "bumper";
        public const string MiniPlatform = "mini_platform";
        public const string GravitySwitch = "gravity_switch";

        public static readonly string[] AllIds =
        {
            Spring, Ramp, Fan, PortalAB, PortalCD, Magnet, Bomb, Bumper, MiniPlatform, GravitySwitch
        };

        public static ItemDefinition Create(string itemId)
        {
            var item = ScriptableObject.CreateInstance<ItemDefinition>();
            item.name = "Item_" + itemId;
            item.itemId = itemId;
            item.displayNameKey = "item." + itemId;
            switch (itemId)
            {
                case Spring:
                    Setup(item, ItemKind.Spring, ArtId.ItemSpring, "#FFB020", true, 15f);
                    break;
                case Ramp:
                    Setup(item, ItemKind.Ramp, ArtId.ItemRamp, "#D9A066", true, 15f);
                    break;
                case Fan:
                    Setup(item, ItemKind.Fan, ArtId.ItemFan, "#4FC3F7", true, 15f);
                    break;
                case PortalAB:
                    Setup(item, ItemKind.Portal, ArtId.ItemPortal, "#9B59FF", true, 45f);
                    item.piecesPerUnit = 2;
                    item.portalChannel = 0;
                    break;
                case PortalCD:
                    Setup(item, ItemKind.Portal, ArtId.ItemPortal, "#FF8C42", true, 45f);
                    item.piecesPerUnit = 2;
                    item.portalChannel = 1;
                    break;
                case Magnet:
                    Setup(item, ItemKind.Magnet, ArtId.ItemMagnet, "#E74C3C", false, 0f);
                    break;
                case Bomb:
                    Setup(item, ItemKind.Bomb, ArtId.ItemBomb, "#34495E", false, 0f);
                    break;
                case Bumper:
                    Setup(item, ItemKind.Bumper, ArtId.ItemBumper, "#FF4FA3", false, 0f);
                    break;
                case MiniPlatform:
                    Setup(item, ItemKind.MiniPlatform, ArtId.ItemPlatform, "#6C7A89", true, 15f);
                    break;
                case GravitySwitch:
                    Setup(item, ItemKind.GravitySwitch, ArtId.ItemGravity, "#2ECC71", false, 0f);
                    break;
                default:
                    Debug.LogWarning($"[TRYKLI] Unknown built-in item '{itemId}'.");
                    Setup(item, ItemKind.Spring, ArtId.ItemSpring, "#FFFFFF", true, 15f);
                    break;
            }

            return item;
        }

        public static List<ItemDefinition> CreateAll()
        {
            var list = new List<ItemDefinition>();
            foreach (string id in AllIds) list.Add(Create(id));
            return list;
        }

        private static void Setup(ItemDefinition item, ItemKind kind, ArtId icon, string color, bool rotatable, float step)
        {
            item.kind = kind;
            item.icon = icon;
            item.color = ColorUtils.Hex(color);
            item.rotatable = rotatable;
            item.rotationStep = step;
        }
    }
}
