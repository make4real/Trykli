using Trykli.Audio;
using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Objectives;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Periodic hazard (GDD 12.14): a beam emitted along the laser's up direction. Can be permanent or
    /// intermittent (on / off durations with a phase) and disabled by a button. Lethal by default; a non
    /// lethal laser only pushes Trykli back and counts as a hazard contact for the star objectives.
    /// </summary>
    public sealed class Laser : MechanismBase
    {
        [Header("Laser")]
        [SerializeField] private float beamLength = 4f;
        [SerializeField] private float beamThickness = 0.14f;
        [Tooltip("Seconds on / off. Both 0 = permanent beam.")]
        [SerializeField] private float onDuration;
        [SerializeField] private float offDuration;
        [SerializeField] private float phase;
        [SerializeField] private bool lethal = true;
        [SerializeField] private float knockbackSpeed = 5f;

        private float _lastHit = -10f;
        private bool _beamOn = true;
        private float _time;

        public bool BeamOn => _beamOn;

        protected override void ApplyData(ElementData data)
        {
            beamLength = OrDefault(data.length, beamLength);
            onDuration = Mathf.Max(0f, data.onDuration);
            offDuration = Mathf.Max(0f, data.offDuration);
            phase = data.phase;
            lethal = data.lethal;
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sprite(root, "Glow", ArtId.Square, new Color(1f, 0.2f, 0.3f, 0.25f), SortingOrders.Hazards - 1, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Beam", ArtId.Square, new Color(1f, 0.25f, 0.3f, 0.95f), SortingOrders.Hazards, Vector2.zero, Vector2.one);
            VisualFactory.Sliced(root, "Emitter", ArtId.RoundedBox, ColorUtils.Hex("#263238"), SortingOrders.Hazards + 1, new Vector2(0.5f, 0.4f));
            VisualFactory.Sprite(root, "Lens", ArtId.Circle, new Color(1f, 0.3f, 0.35f), SortingOrders.Hazards + 2, new Vector2(0f, 0.08f), new Vector2(0.18f, 0.18f));
        }

        protected override void ApplyLayout(Transform root)
        {
            Transform beam = FindVisual("Beam");
            if (beam != null)
            {
                beam.localPosition = new Vector3(0f, 0.2f + beamLength * 0.5f, 0f);
                beam.localScale = new Vector3(beamThickness, beamLength, 1f);
            }

            Transform glow = FindVisual("Glow");
            if (glow != null)
            {
                glow.localPosition = new Vector3(0f, 0.2f + beamLength * 0.5f, 0f);
                glow.localScale = new Vector3(beamThickness * 3.5f, beamLength, 1f);
            }

            _beamOn = ComputeBeam(0f);
            UpdateBeamVisual();
        }

        protected override void OnActiveChanged(bool active)
        {
            if (!active && Context != null)
            {
                Context.Tracker.RecordCounter(LevelRunStats.Counters.LaserDisabled);
                AudioManager.PlaySfx(SfxId.Laser);
            }
        }

        protected override void RefreshActiveVisual()
        {
            _beamOn = ComputeBeam(_time);
            UpdateBeamVisual();
        }

        private bool ComputeBeam(float time)
        {
            if (!IsActive) return false;
            if (onDuration <= 0f || offDuration <= 0f) return true;
            float cycle = onDuration + offDuration;
            return Mathf.Repeat(time + phase, cycle) < onDuration;
        }

        private void UpdateBeamVisual()
        {
            SpriteRenderer beam = FindRenderer("Beam");
            if (beam != null) beam.color = _beamOn ? new Color(1f, 0.25f, 0.3f, 0.95f) : new Color(1f, 0.25f, 0.3f, 0.12f);
            Transform glow = FindVisual("Glow");
            if (glow != null) glow.gameObject.SetActive(_beamOn);
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            _time = simulationTime;
            bool beamOn = ComputeBeam(simulationTime);
            if (beamOn != _beamOn)
            {
                _beamOn = beamOn;
                UpdateBeamVisual();
            }

            TrykliController trykli = Trykli;
            if (!_beamOn || trykli == null || !trykli.IsAlive) return;

            float rotation = ElementGeometry.RotationOf(transform);
            Vector2 beamCenter = (Vector2)transform.position + (Vector2)transform.up * (0.2f + beamLength * 0.5f);
            Vector2 half = new Vector2(beamThickness * 0.5f, beamLength * 0.5f);
            if (!ElementGeometry.SweptCircleOverlapsBox(trykli.PreviousPosition, trykli.Position, trykli.Radius * 0.9f, beamCenter, half, rotation)) return;

            if (lethal)
            {
                Context.Tracker.RecordHazard(HazardCategory.Laser);
                trykli.Kill(FailureReason.Hazard);
                return;
            }

            if (simulationTime - _lastHit < 0.3f) return;
            _lastHit = simulationTime;
            Context.Tracker.RecordHazard(HazardCategory.Laser);
            Vector2 local = MathUtils.ToLocal(trykli.Position, beamCenter, rotation);
            Vector2 away = (Vector2)transform.right * (local.x >= 0f ? 1f : -1f);
            trykli.Launch(away * knockbackSpeed, BounceSource.Any);
            AudioManager.PlaySfx(SfxId.Laser);
        }

        private void Update()
        {
            if (!_beamOn) return;
            SpriteRenderer glow = FindRenderer("Glow");
            if (glow != null) glow.color = new Color(1f, 0.2f, 0.3f, 0.18f + Mathf.Sin(Time.time * 20f) * 0.07f);
        }
    }
}
