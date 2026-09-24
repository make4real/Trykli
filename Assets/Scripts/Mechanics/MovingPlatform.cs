using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Kinematic platform moving back and forth between its start position and start + travel.
    /// Motion depends only on the simulation time (deterministic timing, GDD 15). It can start moving
    /// when a button is pressed (ActivatedBySignal) and carries Trykli through friction.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class MovingPlatform : MechanismBase
    {
        [Header("Moving platform")]
        [SerializeField] private Vector2 size = new Vector2(2f, 0.3f);
        [SerializeField] private Vector2 travel = new Vector2(3f, 0f);
        [Tooltip("Duration of a full back-and-forth cycle (s).")]
        [SerializeField] private float period = 4f;
        [Tooltip("Cycle offset (0..1).")]
        [Range(0f, 1f)] [SerializeField] private float phase;
        [SerializeField] private float startDelay;
        [Tooltip("Moves only once from start to end (elevator).")]
        [SerializeField] private bool oneWay;

        private Rigidbody2D _body;
        private Vector2 _start;
        private float _activeTime;

        protected override void ApplyData(ElementData data)
        {
            size = OrDefault(data.size, size);
            if (data.travel.sqrMagnitude > 0f) travel = data.travel;
            period = OrDefault(data.period, period);
            phase = Mathf.Repeat(data.phase, 1f);
            startDelay = Mathf.Max(0f, data.delay);
            oneWay = !data.loop;
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sprite(root, "Rail", ArtId.Square, Color.white.WithAlpha(0.25f), SortingOrders.Effects, Vector2.zero, Vector2.one);
            VisualFactory.Sliced(root, "Body", ArtId.RoundedBox, ColorUtils.Hex("#7E57C2"), SortingOrders.Blocks + 1, size);
            VisualFactory.Sprite(root, "Stripe", ArtId.RoundedBox, Color.white.WithAlpha(0.35f), SortingOrders.Blocks + 2, Vector2.zero, new Vector2(0.4f, 0.08f));
        }

        protected override void ApplyLayout(Transform root)
        {
            _body = GetComponent<Rigidbody2D>();
            _body.MakeKinematic();
            var box = GetComponent<BoxCollider2D>();
            box.size = size;
            box.sharedMaterial = PhysicsMaterials.Default;
            gameObject.GetOrAddComponent<SurfaceTag>().kind = SurfaceKind.Platform;
            VisualFactory.SetSlicedSize(FindRenderer("Body"), size);

            // Rail preview showing the path (helps anticipating the timing).
            Transform rail = FindVisual("Rail");
            if (rail != null)
            {
                Vector2 localTravel = Quaternion.Inverse(transform.rotation) * (Vector3)travel;
                float length = localTravel.magnitude;
                rail.localPosition = localTravel * 0.5f;
                rail.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(localTravel.y, localTravel.x) * Mathf.Rad2Deg);
                rail.localScale = new Vector3(length, 0.06f, 1f);
                rail.gameObject.SetActive(length > 0.01f);
            }
        }

        public override void Bind(LevelContext context)
        {
            base.Bind(context);
            _start = transform.position;
            _activeTime = 0f;
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            if (!IsActive || _body == null) return;
            _activeTime += deltaTime;
            float t = _activeTime - startDelay;
            if (t < 0f) return;

            float s;
            if (oneWay) s = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / (period * 0.5f)));
            else s = 0.5f - 0.5f * Mathf.Cos(2f * Mathf.PI * (t / period + phase));

            _body.MovePosition(_start + travel * s);
        }
    }
}
