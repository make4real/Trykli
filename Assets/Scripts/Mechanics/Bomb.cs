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
    /// Explodes on contact with Trykli, after an optional fuse delay, or when its switch channel is signalled.
    /// The explosion adds a velocity away from the bomb (stronger near the center) and triggers nearby bombs.
    /// Not lethal unless <see cref="lethal"/> is set (GDD 12.6).
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class Bomb : MechanismBase
    {
        [Header("Bomb")]
        [SerializeField] private float blastRadius = 2.5f;
        [Tooltip("Velocity added to Trykli at the center of the blast (m/s). Halved at the edge.")]
        [SerializeField] private float impulse = 11f;
        [Tooltip("Explodes automatically this many seconds after GO (0 = contact / signal only).")]
        [SerializeField] private float fuseDelay;
        [SerializeField] private bool lethal;
        [SerializeField] private float bodyRadius = 0.35f;
        [SerializeField] private float chainDelay = 0.12f;

        private bool _exploded;
        private float _scheduledTime = -1f;
        private float _fxTime = -1f;
        private Transform _fx;

        public bool Exploded => _exploded;
        public float BlastRadius => blastRadius;

        protected override void ApplyData(ElementData data)
        {
            blastRadius = OrDefault(data.radius, blastRadius);
            impulse = OrDefault(data.power, impulse);
            fuseDelay = Mathf.Max(0f, data.delay);
            lethal = data.lethal;
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sprite(root, "Range", ArtId.DashedRing, new Color(1f, 0.5f, 0.1f, 0.3f), SortingOrders.Effects, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Body", ArtId.ItemBomb, Color.white, SortingOrders.Mechanisms, Vector2.zero, new Vector2(0.95f, 0.95f));
            VisualFactory.Sprite(root, "Blast", ArtId.SoftGlow, new Color(1f, 0.6f, 0.15f, 0f), SortingOrders.Overlay, Vector2.zero, Vector2.one);
        }

        protected override void ApplyLayout(Transform root)
        {
            GetComponent<CircleCollider2D>().radius = bodyRadius;
            Transform range = FindVisual("Range");
            if (range != null) range.localScale = new Vector3(blastRadius * 2f, blastRadius * 2f, 1f);
            _fx = FindVisual("Blast");
        }

        protected override void OnActiveChanged(bool active)
        {
            // A bomb waiting for a signal explodes when the signal arrives.
            if (active && activation == ActivationMode.ActivatedBySignal && Context != null) Schedule(Context.SimulationTime);
        }

        public override void OnSimulationStart()
        {
            Transform range = FindVisual("Range");
            if (range != null) range.gameObject.SetActive(false);
        }

        public void Schedule(float time)
        {
            if (_exploded) return;
            if (_scheduledTime < 0f || time < _scheduledTime) _scheduledTime = time;
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            if (_exploded) return;
            if (_scheduledTime >= 0f && simulationTime >= _scheduledTime)
            {
                Explode(simulationTime);
                return;
            }

            if (fuseDelay > 0f && activation != ActivationMode.ActivatedBySignal && simulationTime >= fuseDelay)
            {
                Explode(simulationTime);
                return;
            }

            TrykliController trykli = Trykli;
            if (trykli == null || !trykli.IsAlive) return;
            if (ElementGeometry.SweptCircleOverlapsCircle(trykli.PreviousPosition, trykli.Position, trykli.Radius, transform.position, bodyRadius + 0.05f))
            {
                Explode(simulationTime);
            }
        }

        private void Explode(float simulationTime)
        {
            _exploded = true;
            GetComponent<CircleCollider2D>().enabled = false;
            Transform body = FindVisual("Body");
            if (body != null) body.gameObject.SetActive(false);
            _fxTime = Time.time;

            Context.Tracker.RecordCounter(LevelRunStats.Counters.Explosion);
            AudioManager.PlaySfx(SfxId.Explosion);
            Haptics.Play(HapticType.Heavy);

            Vector2 center = transform.position;
            TrykliController trykli = Trykli;
            if (trykli != null && trykli.IsAlive)
            {
                Vector2 offset = trykli.Position - center;
                float distance = offset.magnitude;
                if (distance <= blastRadius)
                {
                    if (lethal && distance < blastRadius * 0.5f)
                    {
                        Context.Tracker.RecordHazard(HazardCategory.Explosion);
                        trykli.Kill(FailureReason.Explosion);
                    }
                    else
                    {
                        Vector2 direction = distance > 0.01f ? offset / distance : (Vector2)transform.up;
                        float falloff = 1f - 0.5f * (distance / blastRadius);
                        trykli.AddVelocity(direction * impulse * falloff);
                    }
                }
            }

            foreach (Bomb other in Context.GetComponentsInChildren<Bomb>())
            {
                if (other == this || other._exploded) continue;
                if (((Vector2)other.transform.position - center).magnitude <= blastRadius) other.Schedule(simulationTime + chainDelay);
            }
        }

        private void Update()
        {
            if (_fxTime < 0f || _fx == null) return;
            float t = (Time.time - _fxTime) / 0.4f;
            if (t >= 1f)
            {
                _fx.gameObject.SetActive(false);
                _fxTime = -1f;
                return;
            }

            float d = blastRadius * 2f * MathUtils.EaseOutCubic(t);
            _fx.localScale = new Vector3(d, d, 1f);
            _fx.GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.15f, 0.8f * (1f - t));
        }
    }
}
