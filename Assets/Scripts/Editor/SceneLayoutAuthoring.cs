using System.Collections.Generic;
using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Placement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Opens a level layout in a temporary scene so it can be edited with the standard Unity tools (move / rotate
    /// elements, zones, spawn and goal), then writes the result back into the LevelData.
    /// </summary>
    public static class SceneLayoutAuthoring
    {
        public const string RootName = "TRYKLI_LEVEL_AUTHORING";

        public static LevelAuthoringRoot FindRoot()
        {
            return Object.FindFirstObjectByType<LevelAuthoringRoot>();
        }

        public static LevelAuthoringRoot Open(LevelData level)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return null;
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            return Build(level);
        }

        /// <summary>Rebuilds the authoring objects from the LevelData (after editing parameters).</summary>
        public static LevelAuthoringRoot Build(LevelData level)
        {
            LevelAuthoringRoot existing = FindRoot();
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var rootGo = new GameObject(RootName);
            var root = rootGo.AddComponent<LevelAuthoringRoot>();
            root.level = level;
            LevelLayout layout = level.layout ?? new LevelLayout();
            WorldTheme theme = WorldTheme.Default;

            var elements = new GameObject("Elements").transform;
            elements.SetParent(rootGo.transform, false);
            foreach (ElementData data in layout.elements)
            {
                GameObject go = ElementFactory.Create(data, elements, theme, allowPrefab: false);
                go.AddComponent<ElementAuthoring>().data = data.Clone();
            }

            var zones = new GameObject("Zones").transform;
            zones.SetParent(rootGo.transform, false);
            ItemCatalog catalog = ItemAssetGenerator.LoadCatalog();
            for (int i = 0; i < layout.zones.Count; i++) PlacementZone.Create(zones, layout.zones[i], i, catalog);

            root.spawnMarker = CreateMarker(rootGo.transform, "Spawn (Trykli)", layout.spawn, new Color(1f, 0.6f, 0.2f));
            root.goalMarker = GoalZone.Create(rootGo.transform, layout.goal, layout.goalRadius, theme).transform;
            root.goalMarker.name = "Goal (exit)";

            if (Camera.main != null)
            {
                Camera.main.orthographic = true;
                Camera.main.orthographicSize = layout.bounds.height * 0.6f;
                Camera.main.transform.position = new Vector3(layout.bounds.center.x, layout.bounds.center.y, -10f);
            }

            Selection.activeGameObject = rootGo;
            SceneView.lastActiveSceneView?.FrameSelected();
            return root;
        }

        private static Transform CreateMarker(Transform parent, string name, Vector2 position, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = Utilities.PlaceholderArt.Get(Utilities.ArtId.Circle);
            renderer.color = color;
            go.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            renderer.sortingOrder = SortingOrders.Trykli;
            return go.transform;
        }

        /// <summary>Writes positions / rotations / parameters of the scene back into the LevelData.</summary>
        public static bool Apply(LevelAuthoringRoot root)
        {
            if (root == null || root.level == null) return false;
            LevelData level = root.level;
            Undo.RecordObject(level, "Apply TRYKLI layout");
            var elements = new List<ElementData>();
            foreach (ElementAuthoring authoring in root.GetComponentsInChildren<ElementAuthoring>())
            {
                elements.Add(authoring.Capture());
            }

            var zones = new List<PlacementZoneData>();
            var zoneComponents = new List<PlacementZone>(root.GetComponentsInChildren<PlacementZone>());
            zoneComponents.Sort((a, b) => a.Index.CompareTo(b.Index));
            foreach (PlacementZone zone in zoneComponents)
            {
                PlacementZoneData data = zone.Data.Clone();
                data.position = ElementAuthoring.RoundVector(zone.transform.position);
                if (data.shape != ZoneShape.Point) data.rotation = Mathf.Round(zone.transform.eulerAngles.z);
                zones.Add(data);
            }

            level.layout.elements = elements;
            level.layout.zones = zones;
            if (root.spawnMarker != null) level.layout.spawn = ElementAuthoring.RoundVector(root.spawnMarker.position);
            if (root.goalMarker != null) level.layout.goal = ElementAuthoring.RoundVector(root.goalMarker.position);
            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();
            return true;
        }

        /// <summary>Adds a new element with default parameters at the scene view pivot.</summary>
        public static void AddElement(LevelAuthoringRoot root, ElementType type)
        {
            if (root == null) return;
            ElementData data = ElementDefaults.Create(type);
            Vector3 pivot = SceneView.lastActiveSceneView != null ? SceneView.lastActiveSceneView.pivot : Vector3.zero;
            data.position = ElementAuthoring.RoundVector(pivot);
            Transform parent = root.transform.Find("Elements") ?? root.transform;
            GameObject go = ElementFactory.Create(data, parent, WorldTheme.Default, allowPrefab: false);
            go.AddComponent<ElementAuthoring>().data = data;
            Undo.RegisterCreatedObjectUndo(go, "Add TRYKLI element");
            Selection.activeGameObject = go;
        }

        /// <summary>Adds a placement zone accepting every item.</summary>
        public static void AddZone(LevelAuthoringRoot root, ZoneShape shape)
        {
            if (root == null) return;
            Transform parent = root.transform.Find("Zones") ?? root.transform;
            int index = root.GetComponentsInChildren<PlacementZone>().Length;
            Vector3 pivot = SceneView.lastActiveSceneView != null ? SceneView.lastActiveSceneView.pivot : Vector3.zero;
            var data = new PlacementZoneData
            {
                id = "zone" + index,
                shape = shape,
                position = ElementAuthoring.RoundVector(pivot),
                size = shape == ZoneShape.Point ? Vector2.zero : new Vector2(3f, shape == ZoneShape.Rect ? 2f : 0f)
            };
            PlacementZone zone = PlacementZone.Create(parent, data, index, ItemAssetGenerator.LoadCatalog());
            Undo.RegisterCreatedObjectUndo(zone.gameObject, "Add TRYKLI zone");
            Selection.activeGameObject = zone.gameObject;
        }

        public static bool IsAuthoringScene()
        {
            return FindRoot() != null && SceneManager.GetActiveScene().path == string.Empty;
        }
    }
}
