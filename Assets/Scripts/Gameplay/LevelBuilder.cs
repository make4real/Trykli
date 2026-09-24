using Trykli.Data;
using Trykli.Placement;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{

    /// <summary>
    /// Builds a playable level from a <see cref="LevelData"/>: background, layout elements (or level prefab),
    /// placement zones and exit. Every restart rebuilds the level from data, which guarantees an identical
    /// initial state (deterministic restart) and automatically restores crystals, bombs, doors...
    /// </summary>
    public static class LevelBuilder
    {
        public static WorldTheme ThemeOf(WorldData world)
        {
            if (world == null) return WorldTheme.Default;
            return new WorldTheme(world.backgroundTop, world.backgroundBottom, world.blockColor, world.accentColor);
        }

        public static LevelContext Build(LevelData level, WorldTheme theme, Transform parent, ItemCatalog catalog)
        {
            var root = new GameObject($"Level_{level.levelId:000}");
            root.transform.SetParent(parent, false);
            var context = root.AddComponent<LevelContext>();
            LevelLayout layout = level.layout ?? new LevelLayout();
            context.Initialize(level, theme, layout.bounds);

            BuildBackground(root.transform, layout.bounds, theme);

            Transform elements = CreateChild(root.transform, "Elements");
            Transform zones = CreateChild(root.transform, "Zones");
            Transform placed = CreateChild(root.transform, "Placed");
            context.ElementsRoot = elements;
            context.PlacedRoot = placed;

            if (level.levelPrefab != null)
            {
                GameObject instance = Object.Instantiate(level.levelPrefab, elements);
                instance.name = level.levelPrefab.name;
                var spawn = instance.GetComponentInChildren<LevelSpawnPoint>();
                context.SpawnPosition = spawn != null ? (Vector2)spawn.transform.position : layout.spawn;
                context.Goal = instance.GetComponentInChildren<GoalZone>();
                if (context.Goal == null) context.Goal = GoalZone.Create(elements, layout.goal, layout.goalRadius, theme);
            }
            else
            {
                foreach (ElementData element in layout.elements)
                {
                    if (element != null) ElementFactory.Create(element, elements, theme);
                }

                for (int i = 0; i < layout.zones.Count; i++) PlacementZone.Create(zones, layout.zones[i], i, catalog);
                context.Goal = GoalZone.Create(elements, layout.goal, layout.goalRadius, theme);
                context.SpawnPosition = layout.spawn;
            }

            BuildSpawnMarker(root.transform, context.SpawnPosition, theme);
            context.CollectStaticElements();
            return context;
        }

        private static Transform CreateChild(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static void BuildBackground(Transform root, Rect bounds, WorldTheme theme)
        {
            var background = new GameObject("Background");
            background.transform.SetParent(root, false);
            var renderer = background.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderArt.CreateGradient(theme.BackgroundBottom, theme.BackgroundTop);
            renderer.sortingOrder = SortingOrders.Background;
            background.transform.position = bounds.center;
            // The sprite is 1x1 unit: scale it well beyond the bounds so every aspect ratio is covered.
            background.transform.localScale = new Vector3(bounds.width + 40f, bounds.height + 40f, 1f);

            // Soft decorative circles (world identity, very low contrast to keep the level readable).
            var random = new System.Random(Mathf.RoundToInt(bounds.width * 13f + bounds.height * 7f));
            for (int i = 0; i < 6; i++)
            {
                float x = bounds.xMin + (float)random.NextDouble() * bounds.width;
                float y = bounds.yMin + (float)random.NextDouble() * bounds.height;
                float size = 2f + (float)random.NextDouble() * 4f;
                VisualFactory.Sprite(background.transform.parent, "Decor" + i, ArtId.Circle, theme.BackgroundTop.Lighten(0.35f).WithAlpha(0.35f),
                    SortingOrders.BackgroundDecor, new Vector2(x, y), new Vector2(size, size));
            }
        }

        private static void BuildSpawnMarker(Transform root, Vector2 position, WorldTheme theme)
        {
            SpriteRenderer marker = VisualFactory.Sprite(root, "SpawnMarker", ArtId.DashedRing, theme.Accent.WithAlpha(0.6f), SortingOrders.Zones,
                position, new Vector2(1f, 1f));
            marker.transform.localPosition = position;
        }
    }
}
