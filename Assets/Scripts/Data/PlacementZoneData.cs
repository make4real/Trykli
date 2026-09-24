using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>Describes an area of the level where the player is allowed to place items.</summary>
    [Serializable]
    public class PlacementZoneData
    {
        public string id = "";
        public ZoneShape shape = ZoneShape.Point;
        public Vector2 position;
        [Tooltip("Rect: width/height. Rail: x = length. Point: ignored.")]
        public Vector2 size;
        [Tooltip("Rail / rect orientation in degrees.")]
        public float rotation;
        [Tooltip("Item ids accepted by this zone. Empty = every item.")]
        public List<string> allowedItems = new List<string>();
        public bool allowRotation = true;
        public float minRotation = -180f;
        public float maxRotation = 180f;
        [Tooltip("Rotation applied to an item when it is dropped in this zone.")]
        public float defaultRotation;
        [Tooltip("Maximum number of items in this zone.")]
        public int capacity = 1;

        public bool Accepts(string itemId)
        {
            return allowedItems == null || allowedItems.Count == 0 || allowedItems.Contains(itemId);
        }

        public PlacementZoneData Clone()
        {
            var copy = (PlacementZoneData)MemberwiseClone();
            copy.allowedItems = allowedItems != null ? new List<string>(allowedItems) : new List<string>();
            return copy;
        }
    }
}
