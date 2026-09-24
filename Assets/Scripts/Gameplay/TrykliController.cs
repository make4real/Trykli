using System;
using Trykli.Audio;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Save;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    public enum TrykliState
    {
        Idle = 0,
        Falling = 1,
        Rolling = 2,
        Bouncing = 3,
        Flying = 4,
        Teleporting = 5,
        Stuck = 6,
        Dead = 7,
        Victory = 8
    }

    /// <summary>
    /// The creature. A dynamic Rigidbody2D moved only by physics and mechanisms: the player never controls it.
    /// Tracks its state, contacts, idle / stuck time and leaving the level.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public sealed class TrykliController : MonoBehaviour, ILevelElement, ISimulationElement
    {
        private const float LaunchStateDuration = 0.3f;
        private const float TeleportStateDuration = 0.15f;
        private const float RollingSpeed = 0.25f;

        private readonly ContactPoint2D[] _contacts = new ContactPoint2D[8];

        private LevelContext _context;
        private GameplayConfig _config;
        private float _stuckTimer;
        private float _launchTimer;
        private float _teleportTimer;
        private int _stickyZones;
        private float _stickyDamping;
        private Vector2 _previousPosition;
        private Vector2 _currentPosition;

        public Rigidbody2D Body { get; private set; }
        public CircleCollider2D Collider { get; private set; }
        public TrykliVisual Visual { get; private set; }
        public TrykliState State { get; private set; } = TrykliState.Idle;
        public bool IsAlive => State != TrykliState.Dead && State != TrykliState.Victory;
        public bool IsCaptured { get; private set; }
        public bool GravityInverted { get; private set; }
        public float Radius => Collider != null ? Collider.radius : 0.35f;
        public Vector2 Position => Body.position;
        public Vector2 Velocity => Body.GetVelocity();
        public float Speed => Body.GetVelocity().magnitude;
        public int ContactCount { get; private set; }
        public float StuckProgress => _config != null && _config.stuckDuration > 0f ? _stuckTimer / _config.stuckDuration : 0f;

        /// <summary>Position before / after the last physics step (interpolation and swept detection).</summary>
        public Vector2 PreviousPosition => _previousPosition;
        public Vector2 CurrentPosition => _currentPosition;

        public event Action<TrykliState> StateChanged;

        public static TrykliController FromCollider(Collider2D collider)
        {
            if (collider == null) return null;
            Rigidbody2D body = collider.attachedRigidbody;
            if (body != null && body.TryGetComponent(out TrykliController trykli)) return trykli;
            return collider.GetComponentInParent<TrykliController>();
        }

        public static TrykliController Create(Transform parent, Vector2 position, GameplayConfig config, SkinDefinition skin)
        {
            var go = new GameObject("Trykli");
            go.transform.SetParent(parent, false);
            go.transform.position = position;

            var body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.mass = config.trykliMass;
            body.gravityScale = 1f;
            body.SetLinearDamping(0f);
            body.SetAngularDamping(0.05f);
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.None;
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;

            var collider = go.AddComponent<CircleCollider2D>();
            collider.radius = config.trykliRadius;
            collider.sharedMaterial = PhysicsMaterials.ForTrykli(config.trykliFriction, config.trykliBounciness);

            var controller = go.AddComponent<TrykliController>();
            controller._config = config;
            controller.Body = body;
            controller.Collider = collider;
            controller._previousPosition = position;
            controller._currentPosition = position;
            controller.Visual = TrykliVisual.Create(go.transform, controller, config.trykliRadius, skin);
            return controller;
        }

        private void Awake()
        {
            if (Body == null) Body = GetComponent<Rigidbody2D>();
            if (Collider == null) Collider = GetComponent<CircleCollider2D>();
            if (_config == null) _config = GameplayConfig.Instance;
        }

        public void Bind(LevelContext context)
        {
            _context = context;
            context.Trykli = this;
            context.Gravity.Changed += OnGravityChanged;
        }

        public void OnSimulationStart()
        {
            Body.simulated = true;
            Body.bodyType = RigidbodyType2D.Dynamic;
            Body.gravityScale = GravityInverted ? -1f : 1f;
            _previousPosition = Body.position;
            _currentPosition = Body.position;
            SetState(TrykliState.Falling);
        }

        /// <summary>Called by the SimulationRunner before the physics step (mechanisms already stepped).</summary>
        public void OnSimulationStep(float deltaTime, float simulationTime)
        {
            if (!IsAlive || _context == null) return;

            _launchTimer = Mathf.Max(0f, _launchTimer - deltaTime);
            _teleportTimer = Mathf.Max(0f, _teleportTimer - deltaTime);

            if (IsCaptured)
            {
                _stuckTimer = 0f;
                return;
            }

            Vector2 velocity = Body.GetVelocity();
            if (velocity.sqrMagnitude > _config.maxSpeed * _config.maxSpeed)
            {
                velocity = velocity.normalized * _config.maxSpeed;
                Body.SetVelocity(velocity);
            }

            float speed = velocity.magnitude;
            ContactCount = Body.GetContacts(_contacts);
            _context.Tracker.Tick(deltaTime, speed, _config.idleThresholdForObjectives);
            UpdateState(velocity, speed);

            if (!_context.KillBounds(_config.outOfBoundsMargin).Contains(Body.position))
            {
                Kill(FailureReason.OutOfBounds, playEffects: false);
                return;
            }

            if (speed < _config.stuckSpeedThreshold)
            {
                _stuckTimer += deltaTime;
                if (_stuckTimer >= _config.stuckDuration)
                {
                    SetState(TrykliState.Stuck);
                    _context.RequestFailure(FailureReason.Stuck);
                }
            }
            else
            {
                _stuckTimer = 0f;
            }
        }

        public void BeforePhysicsStep()
        {
            _previousPosition = Body.position;
        }

        public void AfterPhysicsStep()
        {
            _currentPosition = Body.position;
        }

        public Vector2 GetInterpolatedPosition(float alpha)
        {
            return Vector2.Lerp(_previousPosition, _currentPosition, Mathf.Clamp01(alpha));
        }

        /// <summary>Sets the velocity (springs, bumpers, cannons: predictable launches).</summary>
        public void Launch(Vector2 velocity, BounceSource source)
        {
            if (!IsAlive || IsCaptured) return;
            Body.SetVelocity(velocity);
            Body.angularVelocity = 0f;
            _launchTimer = LaunchStateDuration;
            _stuckTimer = 0f;
            if (source != BounceSource.Any && _context != null) _context.Tracker.RecordBounce(source);
            SetState(TrykliState.Bouncing);
            if (Visual != null) Visual.OnLaunched(velocity);
        }

        /// <summary>Adds an instantaneous velocity change (explosions).</summary>
        public void AddVelocity(Vector2 deltaVelocity)
        {
            if (!IsAlive || IsCaptured) return;
            Body.SetVelocity(Body.GetVelocity() + deltaVelocity);
            _launchTimer = LaunchStateDuration;
            _stuckTimer = 0f;
            SetState(TrykliState.Flying);
            if (Visual != null) Visual.OnLaunched(deltaVelocity);
        }

        /// <summary>Continuous acceleration for this physics step (fans, magnets).</summary>
        public void ApplyAcceleration(Vector2 acceleration)
        {
            if (!IsAlive || IsCaptured) return;
            Body.AddForce(acceleration * Body.mass, ForceMode2D.Force);
        }

        public void TeleportTo(Vector2 position, Vector2 velocity)
        {
            if (!IsAlive) return;
            Body.position = position;
            transform.position = position;
            Body.SetVelocity(velocity);
            _previousPosition = position;
            _currentPosition = position;
            _teleportTimer = TeleportStateDuration;
            _stuckTimer = 0f;
            SetState(TrykliState.Teleporting);
            if (Visual != null) Visual.OnTeleported();
        }

        /// <summary>Holds Trykli in place (cannon).</summary>
        public void Capture(Vector2 position)
        {
            if (!IsAlive) return;
            IsCaptured = true;
            Body.SetVelocity(Vector2.zero);
            Body.angularVelocity = 0f;
            Body.bodyType = RigidbodyType2D.Kinematic;
            Body.position = position;
            transform.position = position;
            _previousPosition = position;
            _currentPosition = position;
            if (Visual != null) Visual.SetHidden(true);
        }

        public void ReleaseFromCapture(Vector2 position, Vector2 velocity)
        {
            if (!IsCaptured) return;
            IsCaptured = false;
            Body.bodyType = RigidbodyType2D.Dynamic;
            Body.position = position;
            transform.position = position;
            _previousPosition = position;
            _currentPosition = position;
            if (Visual != null) Visual.SetHidden(false);
            Launch(velocity, BounceSource.Any);
        }

        public void EnterSticky(float damping)
        {
            _stickyZones++;
            _stickyDamping = Mathf.Max(_stickyDamping, damping);
            Body.SetLinearDamping(_stickyDamping);
        }

        public void ExitSticky()
        {
            _stickyZones = Mathf.Max(0, _stickyZones - 1);
            if (_stickyZones == 0)
            {
                _stickyDamping = 0f;
                Body.SetLinearDamping(0f);
            }
        }

        public void SetGravityInverted(bool inverted)
        {
            GravityInverted = inverted;
            Body.gravityScale = inverted ? -1f : 1f;
            if (Visual != null) Visual.SetUpsideDown(inverted);
        }

        public void Kill(FailureReason reason, bool playEffects = true)
        {
            if (!IsAlive) return;
            SetState(TrykliState.Dead);
            Body.SetVelocity(Vector2.zero);
            Body.angularVelocity = 0f;
            Body.simulated = false;
            if (playEffects)
            {
                AudioManager.PlaySfx(SfxId.Failure);
                Haptics.Play(HapticType.Heavy);
            }

            _context?.RequestFailure(reason);
        }

        public void Win(Vector2 goalCenter)
        {
            if (!IsAlive) return;
            SetState(TrykliState.Victory);
            Body.SetVelocity(Vector2.zero);
            Body.simulated = false;
            if (Visual != null) Visual.PlayVictory(goalCenter);
        }

        private void UpdateState(Vector2 velocity, float speed)
        {
            TrykliState next;
            if (_teleportTimer > 0f) next = TrykliState.Teleporting;
            else if (_launchTimer > 0f) next = TrykliState.Bouncing;
            else if (ContactCount > 0) next = speed > RollingSpeed ? TrykliState.Rolling : TrykliState.Idle;
            else
            {
                Vector2 down = _context != null ? _context.Gravity.Down : Vector2.down;
                next = Vector2.Dot(velocity, down) > 0f ? TrykliState.Falling : TrykliState.Flying;
            }

            SetState(next);
        }

        private void SetState(TrykliState state)
        {
            if (State == state) return;
            State = state;
            StateChanged?.Invoke(state);
        }

        private void OnGravityChanged(bool inverted)
        {
            SetGravityInverted(inverted);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_context == null || !_context.IsSimulating) return;
            SurfaceTag tag = collision.collider.GetComponentInParent<SurfaceTag>();
            if (tag != null) _context.Tracker.RecordContact(tag.kind);

            float impact = collision.relativeVelocity.magnitude;
            if (impact > 2.5f)
            {
                AudioManager.PlaySfx(SfxId.Collision, Mathf.Clamp01(impact / 12f));
                if (Visual != null && collision.contactCount > 0) Visual.OnImpact(collision.GetContact(0).normal, impact);
            }

            if (impact > 9f) Haptics.Play(HapticType.Heavy);
        }

        private void OnDestroy()
        {
            if (_context != null) _context.Gravity.Changed -= OnGravityChanged;
        }
    }
}
