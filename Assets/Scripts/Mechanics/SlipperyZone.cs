using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>Frictionless (icy) surface: Trykli slides without losing speed.</summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class SlipperyZone : MonoBehaviour, IConfigurableElement
    {
        [SerializeField] private Vector2 size = new Vector2(3f, 0.4f);
        [SerializeField] private SurfaceKind surface = SurfaceKind.Ground;

        public void Configure(ElementData data, WorldTheme theme)
        {
            if (data.size.x > 0f && data.size.y > 0f) size = data.size;
            surface = data.surface;
            var box = GetComponent<BoxCollider2D>();
            box.size = size;
            box.sharedMaterial = PhysicsMaterials.Frictionless;
            gameObject.GetOrAddComponent<SurfaceTag>().kind = surface;

            Transform root = VisualFactory.GetOrCreateVisualRoot(transform, out bool created);
            if (created)
            {
                VisualFactory.Sliced(root, "Ice", ArtId.RoundedBox, ColorUtils.Hex("#B3E5FC"), SortingOrders.Blocks, size);
                VisualFactory.Sprite(root, "Shine", ArtId.RoundedBox, Color.white.WithAlpha(0.6f), SortingOrders.Blocks + 1, Vector2.zero, new Vector2(0.5f, 0.06f), 0f);
            }

            VisualFactory.SetSlicedSize(root.Find("Ice")?.GetComponent<SpriteRenderer>(), size);
            Transform shine = root.Find("Shine");
            if (shine != null)
            {
                shine.localPosition = new Vector3(-size.x * 0.2f, size.y * 0.25f, 0f);
                shine.localScale = new Vector3(size.x * 0.35f, 0.06f, 1f);
            }
        }
    }
}
