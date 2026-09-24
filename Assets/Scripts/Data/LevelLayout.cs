using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>
    /// Complete geometry of a level: spawn, goal, static and dynamic elements, placement zones.
    /// Built at runtime by <c>LevelBuilder</c>; no scene or prefab is required per level.
    /// </summary>
    [Serializable]
    public class LevelLayout
    {
        public Vector2 spawn = new Vector2(-3.5f, 6f);
        public Vector2 goal = new Vector2(3.5f, 0f);
        public float goalRadius = 0.6f;
        [Tooltip("Playable area in world units. The camera fits it; leaving it (plus a margin) fails the level.")]
        public Rect bounds = new Rect(-5f, -5f, 10f, 14f);
        public List<ElementData> elements = new List<ElementData>();
        public List<PlacementZoneData> zones = new List<PlacementZoneData>();

        public LevelLayout Clone()
        {
            var copy = new LevelLayout
            {
                spawn = spawn,
                goal = goal,
                goalRadius = goalRadius,
                bounds = bounds,
                elements = new List<ElementData>(),
                zones = new List<PlacementZoneData>()
            };
            if (elements != null) foreach (ElementData e in elements) copy.elements.Add(e.Clone());
            if (zones != null) foreach (PlacementZoneData z in zones) copy.zones.Add(z.Clone());
            return copy;
        }

        public int CountElements(ElementType type)
        {
            int count = 0;
            if (elements == null) return 0;
            foreach (ElementData e in elements) if (e.type == type) count++;
            return count;
        }
    }

    [Serializable]
    public class CameraSettingsData
    {
        [Tooltip("Fit the whole level bounds on screen (recommended).")]
        public bool autoFit = true;
        [Tooltip("Used when autoFit is false.")]
        public float orthographicSize = 10f;
        [Tooltip("Follow Trykli during the simulation, clamped to the level bounds (large levels).")]
        public bool followTrykli;
    }

    [Serializable]
    public class HintData
    {
        [Tooltip("Localization key of the textual tip.")]
        public string textKey = "";
        [Tooltip("Placement zone to highlight (-1 = none).")]
        public int zoneIndex = -1;
        [Tooltip("Item recommended for that zone (empty = none).")]
        public string itemId = "";
        [Tooltip("Show a direction arrow using 'rotation'.")]
        public bool showDirection;
        public float rotation;
    }

    /// <summary>One step of the reference solution (used by hints and by the solution verifier).</summary>
    [Serializable]
    public class SolutionStep
    {
        public string itemId = "";
        public int zoneIndex;
        [Tooltip("World position for Rect / Rail zones. Ignored for Point zones.")]
        public Vector2 position;
        public float rotation;
    }

    [Serializable]
    public class ItemCount
    {
        public string itemId = "";
        public int count = 1;

        public ItemCount() { }

        public ItemCount(string itemId, int count)
        {
            this.itemId = itemId;
            this.count = count;
        }
    }
}
