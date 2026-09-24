using Trykli.Audio;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>Circular object that strongly repels Trykli with a fixed speed (flash feedback).</summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class Bumper : MechanismBase
    {
        [Header("Bumper")]
        [SerializeField] private float radius = 0.45f;
        [Tooltip("Speed given to Trykli away from the bumper center (m/s).")]
        [SerializeField] private float bounceSpeed = 10f;
        [SerializeField] private float cooldown = 0.15f;

        private float _lastHit = -10f;
        private float _flash;

        protected override void ApplyData(ElementData data)
        {
            radius = OrDefault(data.radius, radius);
            bounceSpeed = OrDefault(data.power, bounceSpeed);
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sprite(root, "Flash", ArtId.SoftGlow, new Color(1f, 0.4f, 0.7f, 0f), SortingOrders.Effects, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Body", ArtId.ItemBumper, Color.white, SortingOrders.Mechanisms, Vector2.zero, Vector2.one);
        }

        protected override void ApplyLayout(Transform root)
        {
            GetComponent<CircleCollider2D>().radius = radius;
            Transform body = FindVisual("Body");
            if (body != null) body.localScale = new Vector3(radius * 2.1f, radius * 2.1f, 1f);
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (!IsActive || trykli == null || !trykli.IsAlive) return;
            if (simulationTime - _lastHit < cooldown) return;
            Vector2 center = transform.position;
            if (!ElementGeometry.SweptCircleOverlapsCircle(trykli.PreviousPosition, trykli.Position, trykli.Radius, center, radius + 0.04f)) return;

            Vector2 direction = trykli.Position - center;
            direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : (Vector2)transform.up;
            _lastHit = simulationTime;
            _flash = 1f;
            trykli.Launch(direction * bounceSpeed, BounceSource.Bumper);
            AudioManager.PlaySfx(SfxId.Bumper);
            Haptics.Play(HapticType.Medium);
        }

        private void Update()
        {
            if (_flash <= 0f) return;
            _flash = Mathf.Max(0f, _flash - Time.deltaTime * 4f);
            SpriteRenderer flash = FindRenderer("Flash");
            if (flash != null)
            {
                float d = radius * 2f * (1.6f + (1f - _flash));
                flash.transform.localScale = new Vector3(d, d, 1f);
                flash.color = new Color(1f, 0.4f, 0.7f, _flash * 0.8f);
            }

            Transform body = FindVisual("Body");
            if (body != null)
            {
                float s = radius * 2.1f * (1f + 0.15f * _flash);
                body.localScale = new Vector3(s, s, 1f);
            }
        }
    }
}
