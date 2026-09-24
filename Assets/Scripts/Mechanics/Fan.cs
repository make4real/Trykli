using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Creates a constant air stream in front of it (its up direction). Trykli receives a constant
    /// acceleration while its center is inside the stream zone. Direction = rotation (45 deg steps when placed).
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class Fan : MechanismBase
    {
        private const int StreakCount = 6;

        [Header("Fan")]
        [Tooltip("Acceleration applied to Trykli inside the stream (m/s^2). Gravity is 9.81.")]
        [SerializeField] private float acceleration = 16f;
        [SerializeField] private float streamLength = 4f;
        [SerializeField] private float streamWidth = 1.2f;
        [SerializeField] private Vector2 bodySize = new Vector2(0.9f, 0.45f);

        private Transform[] _streaks;
        private float _animation;

        public float StreamLength => streamLength;
        public float StreamWidth => streamWidth;

        protected override void ApplyData(ElementData data)
        {
            acceleration = OrDefault(data.power, acceleration);
            streamLength = OrDefault(data.length, streamLength);
            if (data.size.x > 0f) streamWidth = data.size.x;
        }

        protected override void BuildVisual(Transform root)
        {
            Color blue = ColorUtils.Hex("#4FC3F7");
            VisualFactory.Sliced(root, "Stream", ArtId.RoundedBox, blue.WithAlpha(0.14f), SortingOrders.Effects, Vector2.one);
            var streaks = new GameObject("Streaks").transform;
            streaks.SetParent(root, false);
            for (int i = 0; i < StreakCount; i++)
            {
                VisualFactory.Sprite(streaks, "Streak" + i, ArtId.RoundedBox, Color.white.WithAlpha(0.5f), SortingOrders.Effects + 1,
                    Vector2.zero, new Vector2(0.06f, 0.35f));
            }

            VisualFactory.Sliced(root, "Body", ArtId.RoundedBox, ColorUtils.Hex("#37474F"), SortingOrders.Mechanisms, bodySize);
            VisualFactory.Sprite(root, "Blades", ArtId.ItemFan, Color.white, SortingOrders.Mechanisms + 1, Vector2.zero, new Vector2(0.55f, 0.55f));
        }

        protected override void ApplyLayout(Transform root)
        {
            var box = GetComponent<BoxCollider2D>();
            box.size = bodySize;
            VisualFactory.SetSlicedSize(FindRenderer("Body"), bodySize);
            SpriteRenderer stream = FindRenderer("Stream");
            VisualFactory.SetSlicedSize(stream, new Vector2(streamWidth, streamLength));
            if (stream != null) stream.transform.localPosition = new Vector3(0f, bodySize.y * 0.5f + streamLength * 0.5f, 0f);

            Transform streaks = FindVisual("Streaks");
            if (streaks != null)
            {
                _streaks = new Transform[streaks.childCount];
                for (int i = 0; i < streaks.childCount; i++) _streaks[i] = streaks.GetChild(i);
            }
        }

        protected override void RefreshActiveVisual()
        {
            SpriteRenderer stream = FindRenderer("Stream");
            if (stream != null) stream.color = ColorUtils.Hex("#4FC3F7").WithAlpha(IsActive ? 0.16f : 0.04f);
            Transform streaks = FindVisual("Streaks");
            if (streaks != null) streaks.gameObject.SetActive(IsActive);
        }

        public bool IsInStream(Vector2 point)
        {
            Vector2 local = MathUtils.ToLocal(point, transform.position, ElementGeometry.RotationOf(transform));
            float start = bodySize.y * 0.5f - 0.1f;
            return local.y >= start && local.y <= start + streamLength && Mathf.Abs(local.x) <= streamWidth * 0.5f;
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (!IsActive || trykli == null || !trykli.IsAlive) return;
            if (IsInStream(trykli.Position)) trykli.ApplyAcceleration((Vector2)transform.up * acceleration);
        }

        private void Update()
        {
            if (_streaks == null || !IsActive) return;
            _animation += Time.deltaTime * 1.6f;
            Transform blades = FindVisual("Blades");
            if (blades != null) blades.Rotate(0f, 0f, 540f * Time.deltaTime);
            float start = bodySize.y * 0.5f;
            for (int i = 0; i < _streaks.Length; i++)
            {
                float t = Mathf.Repeat(_animation + i / (float)_streaks.Length, 1f);
                float x = ((i * 0.37f) % 1f - 0.5f) * streamWidth * 0.8f;
                _streaks[i].localPosition = new Vector3(x, start + t * streamLength, 0f);
                var renderer = _streaks[i].GetComponent<SpriteRenderer>();
                renderer.color = Color.white.WithAlpha(0.55f * Mathf.Sin(t * Mathf.PI));
            }
        }
    }
}
