using Trykli.Audio;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Gameplay;
using Trykli.Objectives;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Inverts (or sets) the gravity of Trykli when it passes through. Springs, portals, fans and platforms
    /// keep working because they act along their own orientation, independently of gravity.
    /// </summary>
    public sealed class GravitySwitch : MechanismBase
    {
        [Header("Gravity switch")]
        [SerializeField] private GravityMode mode = GravityMode.Toggle;
        [SerializeField] private float radius = 0.4f;
        [Tooltip("Works only once per run.")]
        [SerializeField] private bool oneShot;

        private bool _wasInside;
        private bool _used;
        private Transform _arrows;

        protected override void ApplyData(ElementData data)
        {
            mode = data.gravityMode;
            radius = OrDefault(data.radius, radius);
            oneShot = data.oneShot;
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sprite(root, "Glow", ArtId.SoftGlow, ColorUtils.Hex("#2ECC71").WithAlpha(0.5f), SortingOrders.Effects, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Orb", ArtId.ItemGravity, Color.white, SortingOrders.Mechanisms, Vector2.zero, Vector2.one);
        }

        protected override void ApplyLayout(Transform root)
        {
            float d = radius * 2.2f;
            Transform glow = FindVisual("Glow");
            if (glow != null) glow.localScale = new Vector3(d * 2f, d * 2f, 1f);
            _arrows = FindVisual("Orb");
            if (_arrows != null) _arrows.localScale = new Vector3(d, d, 1f);
        }

        public override void Bind(LevelContext context)
        {
            base.Bind(context);
            context.Gravity.Changed += OnGravityChanged;
        }

        private void OnGravityChanged(bool inverted)
        {
            if (_arrows != null) _arrows.localRotation = Quaternion.Euler(0f, 0f, inverted ? 180f : 0f);
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (!IsActive || trykli == null || !trykli.IsAlive) return;
            bool inside = ElementGeometry.SweptCircleOverlapsCircle(trykli.PreviousPosition, trykli.Position, trykli.Radius * 0.6f, transform.position, radius);
            if (inside && !_wasInside && !(oneShot && _used))
            {
                _used = true;
                if (Context.Gravity.Apply(mode))
                {
                    Context.Tracker.RecordCounter(LevelRunStats.Counters.GravityFlip);
                    AudioManager.PlaySfx(SfxId.GravityFlip);
                    Haptics.Play(HapticType.Medium);
                }
            }

            _wasInside = inside;
        }

        private void OnDestroy()
        {
            if (Context != null) Context.Gravity.Changed -= OnGravityChanged;
        }
    }
}
