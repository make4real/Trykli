using Trykli.Audio;
using Trykli.Data;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Optional collectible (max 3 per level). Disappears when touched, counted by the run tracker.
    /// Crystals are restored automatically because every restart rebuilds the level.
    /// </summary>
    public sealed class Crystal : MonoBehaviour, IConfigurableElement, ILevelElement, ISimulationElement
    {
        public const float PickupRadius = 0.32f;

        [SerializeField] private string crystalId = "";

        private LevelContext _context;
        private Transform _visual;
        private float _collectedTime = -1f;
        private Vector3 _baseScale;

        public string CrystalId => crystalId;
        public bool Collected { get; private set; }

        public void Configure(ElementData data, WorldTheme theme)
        {
            crystalId = string.IsNullOrEmpty(data.id) ? "c" + data.position.GetHashCode() : data.id;
            Transform root = VisualFactory.GetOrCreateVisualRoot(transform, out bool created);
            if (created)
            {
                Color color = new Color(0.4f, 0.9f, 1f);
                VisualFactory.Sprite(root, "Glow", ArtId.SoftGlow, color.WithAlpha(0.5f), SortingOrders.Crystals - 1, Vector2.zero, new Vector2(1.2f, 1.2f));
                VisualFactory.Sprite(root, "Gem", ArtId.Crystal, color, SortingOrders.Crystals, Vector2.zero, new Vector2(0.5f, 0.62f));
            }

            _visual = root;
            _baseScale = root.localScale;
        }

        public void Bind(LevelContext context)
        {
            _context = context;
        }

        public void OnSimulationStart() { }

        public void OnSimulationStep(float deltaTime, float simulationTime)
        {
            if (Collected || _context == null) return;
            TrykliController trykli = _context.Trykli;
            if (trykli == null || !trykli.IsAlive) return;
            Vector2 center = transform.position;
            Vector2 closest = MathUtils.ClosestPointOnSegment(center, trykli.PreviousPosition, trykli.Position);
            float reach = PickupRadius + trykli.Radius;
            if ((closest - center).sqrMagnitude <= reach * reach) Collect();
        }

        private void Collect()
        {
            Collected = true;
            _collectedTime = Time.time;
            _context.Tracker.RecordCrystal(crystalId);
            AudioManager.PlaySfx(SfxId.Crystal);
        }

        private void Update()
        {
            if (_visual == null) return;
            if (!Collected)
            {
                float bob = Mathf.Sin(Time.time * 2.5f + transform.position.x) * 0.06f;
                _visual.localPosition = new Vector3(0f, bob, 0f);
                return;
            }

            float t = (Time.time - _collectedTime) / 0.3f;
            if (t >= 1f)
            {
                _visual.gameObject.SetActive(false);
                return;
            }

            float scale = 1f + t * 0.8f;
            _visual.localScale = _baseScale * scale;
            foreach (SpriteRenderer renderer in _visual.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = renderer.color.WithAlpha(1f - t);
            }
        }
    }
}
