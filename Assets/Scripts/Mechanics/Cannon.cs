using Trykli.Audio;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Gameplay;
using Trykli.Objectives;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>Captures Trykli, waits a short delay, then fires it along the barrel direction (GDD 12.15).</summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class Cannon : MechanismBase
    {
        private enum CannonState
        {
            Ready,
            Loaded,
            Cooldown
        }

        [Header("Cannon")]
        [SerializeField] private float shotSpeed = 12f;
        [SerializeField] private float fireDelay = 0.6f;
        [SerializeField] private float captureRadius = 0.5f;

        private CannonState _state = CannonState.Ready;
        private float _loadedTime;
        private float _recoil;

        protected override void ApplyData(ElementData data)
        {
            shotSpeed = OrDefault(data.power, shotSpeed);
            fireDelay = OrDefault(data.delay, fireDelay);
            captureRadius = OrDefault(data.radius, captureRadius);
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sliced(root, "Barrel", ArtId.RoundedBox, ColorUtils.Hex("#37474F"), SortingOrders.Mechanisms, new Vector2(0.7f, 1.2f), new Vector2(0f, 0.35f));
            VisualFactory.Sprite(root, "Mouth", ArtId.Ring, ColorUtils.Hex("#FFB020"), SortingOrders.Mechanisms + 1, new Vector2(0f, 0.9f), new Vector2(0.6f, 0.3f));
            VisualFactory.Sprite(root, "Base", ArtId.Circle, ColorUtils.Hex("#263238"), SortingOrders.Mechanisms + 1, Vector2.zero, new Vector2(0.9f, 0.9f));
        }

        protected override void ApplyLayout(Transform root)
        {
            var circle = GetComponent<CircleCollider2D>();
            circle.radius = captureRadius;
            circle.isTrigger = true;
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (!IsActive || trykli == null || !trykli.IsAlive) return;
            Vector2 center = transform.position;
            switch (_state)
            {
                case CannonState.Ready:
                    if (ElementGeometry.SweptCircleOverlapsCircle(trykli.PreviousPosition, trykli.Position, trykli.Radius * 0.5f, center, captureRadius))
                    {
                        trykli.Capture(center);
                        _loadedTime = simulationTime;
                        _state = CannonState.Loaded;
                        Context.Tracker.RecordCounter(LevelRunStats.Counters.CannonShot);
                    }

                    break;
                case CannonState.Loaded:
                    if (simulationTime - _loadedTime >= fireDelay)
                    {
                        Vector2 direction = transform.up;
                        trykli.ReleaseFromCapture(center + direction * (captureRadius + trykli.Radius + 0.35f), direction * shotSpeed);
                        _state = CannonState.Cooldown;
                        _recoil = 1f;
                        AudioManager.PlaySfx(SfxId.Cannon);
                        Haptics.Play(HapticType.Heavy);
                    }

                    break;
                case CannonState.Cooldown:
                    if ((trykli.Position - center).magnitude > captureRadius + trykli.Radius + 0.5f) _state = CannonState.Ready;
                    break;
            }
        }

        private void Update()
        {
            if (_recoil <= 0f) return;
            _recoil = Mathf.Max(0f, _recoil - Time.deltaTime * 4f);
            Transform barrel = FindVisual("Barrel");
            if (barrel != null) barrel.localPosition = new Vector3(0f, 0.35f - 0.15f * _recoil, 0f);
        }
    }
}
