using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>
    /// Definition of an object the player can place (spring, fan, portal pair...).
    /// </summary>
    [CreateAssetMenu(menuName = "TRYKLI/Item Definition", fileName = "Item_")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [Tooltip("Stable identifier used by level data (e.g. \"spring\", \"portal_ab\").")]
        public string itemId = "";
        public ItemKind kind;
        public string displayNameKey = "";
        [Header("Visuals")]
        public ArtId icon = ArtId.ItemSpring;
        [Tooltip("Optional sprite replacing the generated icon.")]
        public Sprite iconOverride;
        [Tooltip("Optional prefab replacing the generated item object. Must contain the matching mechanism component.")]
        public GameObject prefabOverride;
        public Color color = Color.white;
        [Header("Placement")]
        public bool rotatable = true;
        [Tooltip("Rotation increment in degrees (15 or 30 recommended).")]
        public float rotationStep = 15f;
        public float defaultRotation;
        [Tooltip("Number of pieces placed per inventory unit (2 for a portal pair A/B).")]
        public int piecesPerUnit = 1;
        [Tooltip("Portal pair channel for portal items (-1 for other items).")]
        public int portalChannel = -1;
        [Tooltip("Optional strength override for the spawned mechanism (0 = default).")]
        public float power;

        public Sprite GetIcon()
        {
            return iconOverride != null ? iconOverride : PlaceholderArt.Get(icon);
        }

        public int PiecesPerUnit => Mathf.Max(1, piecesPerUnit);

        /// <summary>Number of inventory units consumed by <paramref name="pieces"/> placed pieces.</summary>
        public int UnitsForPieces(int pieces)
        {
            if (pieces <= 0) return 0;
            return (pieces + PiecesPerUnit - 1) / PiecesPerUnit;
        }

        public void CopyFrom(ItemDefinition other)
        {
            itemId = other.itemId;
            kind = other.kind;
            displayNameKey = other.displayNameKey;
            icon = other.icon;
            color = other.color;
            rotatable = other.rotatable;
            rotationStep = other.rotationStep;
            defaultRotation = other.defaultRotation;
            piecesPerUnit = other.piecesPerUnit;
            portalChannel = other.portalChannel;
            power = other.power;
        }
    }
}
