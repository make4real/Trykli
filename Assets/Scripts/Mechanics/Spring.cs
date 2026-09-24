using Trykli.Audio;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Projects Trykli along the spring's up direction with a fixed speed (predictable trajectory).
    /// Power, direction and angle are configurable (angle = object rotation).
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class Spring : MechanismBase
    {
        [Header("Spring")]
        [Tooltip("Speed given to Trykli along the spring direction (m/s).")]
        [SerializeField] private float launchSpeed = 11f;
        [SerializeField] private Vector2 size = new Vector2(1f, 0.5f);
        [SerializeField] private float cooldown = 0.25f;

        private float _lastLaunch = -10f;
        private float _compress;

        public float LaunchSpeed => launchSpeed;

        protected override void ApplyData(ElementData data)
        {
            launchSpeed = OrDefault(data.power, launchSpeed);
            size = OrDefault(data.size, size);
        }

        protected override void BuildVisual(Transform root)
        {
            Color metal = ColorUtils.Hex("#5A6270");
            VisualFactory.Sliced(root, "Base", ArtId.RoundedBox, metal, SortingOrders.Mechanisms, Vector2.one);
            var coil = new GameObject("Coil").transform;
            coil.SetParent(root, false);
            for (int i = 0; i < 3; i++)
            {
                VisualFactory.Sprite(coil, "Loop" + i, ArtId.RoundedBox, metal.Lighten(0.2f), SortingOrders.Mechanisms - 1,
                    new Vector2(0f, i * 0.33f), new Vector2(0.8f, 0.12f), i % 2 == 0 ? 12f : -12f);
            }

            VisualFactory.Sliced(root, "Pad", ArtId.RoundedBox, ColorUtils.Hex("#FFB020"), SortingOrders.Mechanisms + 1, Vector2.one);
            VisualFactory.Sprite(root, "Arrow", ArtId.Arrow, Color.white.WithAlpha(0.55f), SortingOrders.Mechanisms + 2, Vector2.zero, new Vector2(0.4f, 0.4f));
        }

        protected override void ApplyLayout(Transform root)
        {
            var box = GetComponent<BoxCollider2D>();
            box.size = new Vector2(size.x, size.y * 0.7f);
            box.offset = new Vector2(0f, -size.y * 0.15f);
            ApplyCompression(0f);
        }

        private void ApplyCompression(float amount)
        {
            float padHeight = size.y * 0.28f;
            float baseHeight = size.y * 0.22f;
            float top = size.y * 0.5f - amount * size.y * 0.3f;
            VisualFactory.SetSlicedSize(FindRenderer("Base"), new Vector2(size.x, baseHeight));
            Transform baseT = FindVisual("Base");
            if (baseT != null) baseT.localPosition = new Vector3(0f, -size.y * 0.5f + baseHeight * 0.5f, 0f);
            VisualFactory.SetSlicedSize(FindRenderer("Pad"), new Vector2(size.x * 1.04f, padHeight));
            Transform pad = FindVisual("Pad");
            if (pad != null) pad.localPosition = new Vector3(0f, top - padHeight * 0.5f, 0f);
            Transform coil = FindVisual("Coil");
            if (coil != null)
            {
                float bottom = -size.y * 0.5f + baseHeight;
                float coilHeight = Mathf.Max(0.05f, top - padHeight - bottom);
                coil.localPosition = new Vector3(0f, bottom + coilHeight * 0.15f, 0f);
                coil.localScale = new Vector3(size.x, coilHeight, 1f);
            }

            Transform arrow = FindVisual("Arrow");
            if (arrow != null) arrow.localPosition = new Vector3(0f, size.y * 0.5f + 0.35f, 0f);
        }

        public override void OnSimulationStart()
        {
            Transform arrow = FindVisual("Arrow");
            if (arrow != null) arrow.gameObject.SetActive(false);
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (!IsActive || trykli == null || !trykli.IsAlive || trykli.IsCaptured) return;
            if (simulationTime - _lastLaunch < cooldown) return;

            float rotation = ElementGeometry.RotationOf(transform);
            Vector2 local = MathUtils.ToLocal(trykli.Position, transform.position, rotation);
            float top = size.y * 0.5f;
            bool onPad = Mathf.Abs(local.x) <= size.x * 0.5f + trykli.Radius * 0.3f &&
                         local.y >= top - 0.15f && local.y <= top + trykli.Radius + 0.18f;
            if (!onPad) return;

            _lastLaunch = simulationTime;
            _compress = 1f;
            trykli.Launch((Vector2)transform.up * launchSpeed, BounceSource.Spring);
            AudioManager.PlaySfx(SfxId.Spring);
            Haptics.Play(HapticType.Light);
        }

        private void Update()
        {
            if (_compress <= 0f) return;
            _compress = Mathf.Max(0f, _compress - Time.deltaTime * 5f);
            ApplyCompression(Mathf.Sin(_compress * Mathf.PI) * 0.6f);
        }
    }
}
