using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>Row of spikes. Contact = failure (GDD 12.13), or a push-back when configured as non lethal.</summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class HazardSpikes : MechanismBase
    {
        [Header("Spikes")]
        [SerializeField] private Vector2 size = new Vector2(1f, 0.4f);
        [SerializeField] private bool lethal = true;
        [SerializeField] private float knockbackSpeed = 6f;

        private float _lastHit = -10f;

        protected override void ApplyData(ElementData data)
        {
            size = OrDefault(data.size, size);
            lethal = data.lethal;
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sliced(root, "Base", ArtId.RoundedBox, ColorUtils.Hex("#546E7A"), SortingOrders.Hazards, Vector2.one);
            var teeth = new GameObject("Teeth").transform;
            teeth.SetParent(root, false);
        }

        protected override void ApplyLayout(Transform root)
        {
            var box = GetComponent<BoxCollider2D>();
            box.size = new Vector2(size.x, size.y * 0.6f);
            box.offset = new Vector2(0f, -size.y * 0.2f);
            VisualFactory.SetSlicedSize(FindRenderer("Base"), new Vector2(size.x, size.y * 0.3f));
            Transform baseT = FindVisual("Base");
            if (baseT != null) baseT.localPosition = new Vector3(0f, -size.y * 0.35f, 0f);

            Transform teeth = FindVisual("Teeth");
            if (teeth == null) return;
            for (int i = teeth.childCount - 1; i >= 0; i--)
            {
                GameObject child = teeth.GetChild(i).gameObject;
                if (Application.isPlaying) Destroy(child);
                else DestroyImmediate(child);
            }

            int count = Mathf.Max(1, Mathf.RoundToInt(size.x / 0.33f));
            float width = size.x / count;
            Color color = lethal ? ColorUtils.Hex("#ECEFF1") : ColorUtils.Hex("#FFCC80");
            for (int i = 0; i < count; i++)
            {
                float x = -size.x * 0.5f + width * (i + 0.5f);
                VisualFactory.Sprite(teeth, "Spike" + i, ArtId.Spike, color, SortingOrders.Hazards + 1, new Vector2(x, size.y * 0.05f),
                    new Vector2(width * 0.95f, size.y * 0.8f));
            }
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (trykli == null || !trykli.IsAlive) return;
            float rotation = ElementGeometry.RotationOf(transform);
            Vector2 half = new Vector2(size.x * 0.5f, size.y * 0.5f);
            if (!ElementGeometry.SweptCircleOverlapsBox(trykli.PreviousPosition, trykli.Position, trykli.Radius + 0.02f, transform.position, half, rotation)) return;

            if (lethal)
            {
                Context.Tracker.RecordHazard(HazardCategory.Spikes);
                trykli.Kill(FailureReason.Hazard);
                return;
            }

            if (simulationTime - _lastHit < 0.3f) return;
            _lastHit = simulationTime;
            Context.Tracker.RecordHazard(HazardCategory.Spikes);
            trykli.Launch((Vector2)transform.up * knockbackSpeed, BounceSource.Any);
        }
    }
}
