using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>Kinematic plank rotating continuously around its center, or when activated by a button.</summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class RotatingPlatform : MechanismBase
    {
        [Header("Rotating platform")]
        [SerializeField] private Vector2 size = new Vector2(2.4f, 0.3f);
        [Tooltip("Degrees per second (negative = clockwise).")]
        [SerializeField] private float angularSpeed = 45f;
        [Tooltip("If > 0, oscillates between -range and +range degrees instead of turning continuously.")]
        [SerializeField] private float oscillationRange;

        private Rigidbody2D _body;
        private float _startRotation;
        private float _activeTime;

        protected override void ApplyData(ElementData data)
        {
            size = OrDefault(data.size, size);
            if (Mathf.Abs(data.speed) > 0f) angularSpeed = data.speed;
            oscillationRange = Mathf.Max(0f, data.length);
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sliced(root, "Body", ArtId.RoundedBox, ColorUtils.Hex("#26A69A"), SortingOrders.Blocks + 1, size);
            VisualFactory.Sprite(root, "Pivot", ArtId.Circle, ColorUtils.Hex("#004D40"), SortingOrders.Blocks + 2, Vector2.zero, new Vector2(0.2f, 0.2f));
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
        }

        public override void Bind(LevelContext context)
        {
            base.Bind(context);
            _startRotation = transform.eulerAngles.z;
            _activeTime = 0f;
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            if (!IsActive || _body == null) return;
            _activeTime += deltaTime;
            float angle = oscillationRange > 0f
                ? _startRotation + Mathf.Sin(_activeTime * angularSpeed * Mathf.Deg2Rad) * oscillationRange
                : _startRotation + angularSpeed * _activeTime;
            _body.MoveRotation(angle);
        }
    }
}
