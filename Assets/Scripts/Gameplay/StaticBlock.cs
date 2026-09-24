using Trykli.Data;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>Static level geometry (ground, walls, ceiling, obstacles).</summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class StaticBlock : MonoBehaviour, IConfigurableElement
    {
        [SerializeField] private Vector2 size = new Vector2(2f, 0.5f);
        [SerializeField] private SurfaceKind surface = SurfaceKind.Ground;

        public Vector2 Size => size;
        public SurfaceKind Surface => surface;

        public void Configure(ElementData data, WorldTheme theme)
        {
            if (data.size.x > 0f && data.size.y > 0f) size = data.size;
            surface = data.surface;

            var box = GetComponent<BoxCollider2D>();
            box.size = size;
            box.sharedMaterial = PhysicsMaterials.Default;
            gameObject.GetOrAddComponent<SurfaceTag>().kind = surface;

            Transform root = VisualFactory.GetOrCreateVisualRoot(transform, out bool created);
            Color color = ColorFor(surface, theme);
            if (created)
            {
                VisualFactory.Sliced(root, "Body", ArtId.RoundedBox, color, SortingOrders.Blocks, size);
                VisualFactory.Sliced(root, "Top", ArtId.RoundedBox, Color.white.WithAlpha(0.18f), SortingOrders.Blocks + 1, size);
            }

            Transform body = root.Find("Body");
            if (body != null)
            {
                var renderer = body.GetComponent<SpriteRenderer>();
                VisualFactory.SetSlicedSize(renderer, size);
                renderer.color = color;
            }

            Transform top = root.Find("Top");
            if (top != null)
            {
                float h = Mathf.Min(0.12f, size.y * 0.3f);
                VisualFactory.SetSlicedSize(top.GetComponent<SpriteRenderer>(), new Vector2(size.x, h));
                top.localPosition = new Vector3(0f, size.y * 0.5f - h * 0.5f, 0f);
            }
        }

        public static Color ColorFor(SurfaceKind surface, WorldTheme theme)
        {
            switch (surface)
            {
                case SurfaceKind.Wall: return theme.Block.Darken(0.18f);
                case SurfaceKind.Ceiling: return theme.Block.Darken(0.1f);
                case SurfaceKind.Obstacle: return Color.Lerp(theme.Block, theme.Accent, 0.35f);
                case SurfaceKind.Platform: return theme.Block.Lighten(0.2f);
                default: return theme.Block;
            }
        }
    }

}
