using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>Zone that strongly slows Trykli down (high linear damping while inside).</summary>
    public sealed class StickyZone : MechanismBase
    {
        [Header("Sticky zone")]
        [SerializeField] private Vector2 size = new Vector2(2f, 1f);
        [Tooltip("Linear damping applied to Trykli inside the zone.")]
        [SerializeField] private float damping = 5f;

        private bool _inside;

        protected override void ApplyData(ElementData data)
        {
            size = OrDefault(data.size, size);
            damping = OrDefault(data.power, damping);
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sliced(root, "Goo", ArtId.RoundedBox, ColorUtils.Hex("#8BC34A").WithAlpha(0.45f), SortingOrders.Effects, Vector2.one);
            for (int i = 0; i < 4; i++)
            {
                VisualFactory.Sprite(root, "Bubble" + i, ArtId.Circle, ColorUtils.Hex("#DCEDC8").WithAlpha(0.6f), SortingOrders.Effects + 1, Vector2.zero, new Vector2(0.14f, 0.14f));
            }
        }

        protected override void ApplyLayout(Transform root)
        {
            VisualFactory.SetSlicedSize(FindRenderer("Goo"), size);
            for (int i = 0; i < 4; i++)
            {
                Transform bubble = FindVisual("Bubble" + i);
                if (bubble != null) bubble.localPosition = new Vector3((i / 3f - 0.5f) * size.x * 0.7f, ((i * 0.61f) % 1f - 0.5f) * size.y * 0.6f, 0f);
            }
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (trykli == null || !trykli.IsAlive) return;
            bool inside = IsActive && ElementGeometry.PointInBox(trykli.Position, transform.position, size * 0.5f, ElementGeometry.RotationOf(transform));
            if (inside == _inside) return;
            _inside = inside;
            if (inside) trykli.EnterSticky(damping);
            else trykli.ExitSticky();
        }
    }
}
