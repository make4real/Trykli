using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Static plank used as a ramp (trajectory change, adjustable angle) or as a mini platform.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class Ramp : MonoBehaviour, IConfigurableElement
    {
        public static readonly Vector2 RampSize = new Vector2(2.4f, 0.22f);
        public static readonly Vector2 MiniPlatformSize = new Vector2(1.4f, 0.25f);

        [SerializeField] private Vector2 size = RampSize;
        [SerializeField] private bool miniPlatform;

        public void Configure(ElementData data, WorldTheme theme)
        {
            miniPlatform = data.type == ElementType.MiniPlatform;
            Vector2 fallback = miniPlatform ? MiniPlatformSize : RampSize;
            size = data.size.x > 0f && data.size.y > 0f ? data.size : fallback;

            var box = GetComponent<BoxCollider2D>();
            box.size = size;
            box.sharedMaterial = PhysicsMaterials.Default;
            gameObject.GetOrAddComponent<SurfaceTag>().kind = SurfaceKind.Platform;

            Transform root = VisualFactory.GetOrCreateVisualRoot(transform, out bool created);
            Color color = miniPlatform ? ColorUtils.Hex("#6C7A89") : ColorUtils.Hex("#D9A066");
            if (created)
            {
                VisualFactory.Sliced(root, "Plank", ArtId.RoundedBox, color, SortingOrders.Mechanisms, size);
                VisualFactory.Sprite(root, "Pivot", ArtId.Circle, color.Darken(0.35f), SortingOrders.Mechanisms + 1, Vector2.zero, new Vector2(0.12f, 0.12f));
            }

            Transform plank = root.Find("Plank");
            if (plank != null)
            {
                var renderer = plank.GetComponent<SpriteRenderer>();
                VisualFactory.SetSlicedSize(renderer, size);
                renderer.color = color;
            }
        }
    }
}
