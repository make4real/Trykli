using Trykli.Save;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Temporary (replaceable) look of Trykli: a round body with two expressive eyes, squash / stretch,
    /// blinking, surprise, victory and death expressions, and cosmetic skin accessories.
    /// The face always stays upright; the physics body may roll freely.
    /// </summary>
    public sealed class TrykliVisual : MonoBehaviour
    {
        private enum Expression
        {
            Normal,
            Surprised,
            Happy,
            Dead
        }

        private TrykliController _owner;
        private Transform _shape;
        private Transform _face;
        private SpriteRenderer _body;
        private SpriteRenderer[] _eyes;
        private SpriteRenderer[] _pupils;
        private Transform _accessoryRoot;
        private float _radius;
        private float _squash;
        private Vector2 _squashAxis = Vector2.up;
        private float _blinkTimer;
        private float _blinkPhase;
        private float _victoryTimer = -1f;
        private Vector2 _victoryTarget;
        private float _teleportFlash;
        private bool _upsideDown;
        private bool _hidden;
        private Expression _expression = Expression.Normal;

        public static TrykliVisual Create(Transform parent, TrykliController owner, float radius, SkinDefinition skin)
        {
            var go = new GameObject(VisualFactory.VisualRootName);
            go.transform.SetParent(parent, false);
            var visual = go.AddComponent<TrykliVisual>();
            visual._owner = owner;
            visual._radius = radius;
            visual.Build(skin);
            return visual;
        }

        private void Build(SkinDefinition skin)
        {
            float diameter = _radius * 2f;
            _shape = new GameObject("Shape").transform;
            _shape.SetParent(transform, false);
            VisualFactory.Sprite(_shape, "Shadow", ArtId.SoftGlow, new Color(0f, 0f, 0f, 0.18f), SortingOrders.Trykli - 1,
                new Vector2(0f, -0.04f), new Vector2(diameter * 1.35f, diameter * 1.35f));
            _body = VisualFactory.Sprite(_shape, "Body", ArtId.Circle, skin.BodyColor, SortingOrders.Trykli, Vector2.zero,
                new Vector2(diameter, diameter));
            VisualFactory.Sprite(_shape, "Highlight", ArtId.Circle, new Color(1f, 1f, 1f, 0.35f), SortingOrders.Trykli + 1,
                new Vector2(-_radius * 0.35f, _radius * 0.4f), new Vector2(diameter * 0.28f, diameter * 0.2f));

            _face = new GameObject("Face").transform;
            _face.SetParent(transform, false);
            _eyes = new SpriteRenderer[2];
            _pupils = new SpriteRenderer[2];
            for (int i = 0; i < 2; i++)
            {
                float side = i == 0 ? -1f : 1f;
                Vector2 eyePosition = new Vector2(side * _radius * 0.36f, _radius * 0.22f);
                _eyes[i] = VisualFactory.Sprite(_face, i == 0 ? "EyeL" : "EyeR", ArtId.Circle, Color.white, SortingOrders.Trykli + 2,
                    eyePosition, new Vector2(_radius * 0.62f, _radius * 0.7f));
                _pupils[i] = VisualFactory.Sprite(_eyes[i].transform, "Pupil", ArtId.Circle, new Color(0.1f, 0.1f, 0.16f), SortingOrders.Trykli + 3,
                    Vector2.zero, new Vector2(0.52f, 0.5f));
            }

            _accessoryRoot = new GameObject("Accessory").transform;
            _accessoryRoot.SetParent(_face, false);
            BuildAccessory(skin);
            _blinkTimer = 2.5f;
        }

        private void BuildAccessory(SkinDefinition skin)
        {
            float r = _radius;
            switch (skin.Accessory)
            {
                case SkinAccessory.Headband:
                    VisualFactory.Sprite(_accessoryRoot, "Band", ArtId.RoundedBox, new Color(0.85f, 0.15f, 0.2f), SortingOrders.Trykli + 4,
                        new Vector2(0f, r * 0.62f), new Vector2(r * 1.9f, r * 0.24f));
                    VisualFactory.Sprite(_accessoryRoot, "Tail", ArtId.RoundedBox, new Color(0.85f, 0.15f, 0.2f), SortingOrders.Trykli - 1,
                        new Vector2(r * 1.05f, r * 0.5f), new Vector2(r * 0.6f, r * 0.14f), -25f);
                    break;
                case SkinAccessory.Antenna:
                    VisualFactory.Sprite(_accessoryRoot, "Stem", ArtId.Square, new Color(0.35f, 0.4f, 0.47f), SortingOrders.Trykli - 1,
                        new Vector2(0f, r * 1.2f), new Vector2(r * 0.1f, r * 0.6f));
                    VisualFactory.Sprite(_accessoryRoot, "Ball", ArtId.Circle, new Color(1f, 0.3f, 0.3f), SortingOrders.Trykli - 1,
                        new Vector2(0f, r * 1.55f), new Vector2(r * 0.35f, r * 0.35f));
                    break;
                case SkinAccessory.Helmet:
                    VisualFactory.Sprite(_accessoryRoot, "Glass", ArtId.Ring, new Color(0.6f, 0.85f, 1f, 0.9f), SortingOrders.Trykli + 4,
                        Vector2.zero, new Vector2(r * 2.5f, r * 2.5f));
                    break;
                case SkinAccessory.EyePatch:
                    VisualFactory.Sprite(_accessoryRoot, "Strap", ArtId.Square, new Color(0.1f, 0.1f, 0.1f), SortingOrders.Trykli + 4,
                        new Vector2(0f, r * 0.45f), new Vector2(r * 2f, r * 0.08f), -18f);
                    VisualFactory.Sprite(_accessoryRoot, "Patch", ArtId.Circle, new Color(0.1f, 0.1f, 0.1f), SortingOrders.Trykli + 5,
                        new Vector2(r * 0.36f, r * 0.22f), new Vector2(r * 0.7f, r * 0.7f));
                    break;
                case SkinAccessory.Drip:
                    VisualFactory.Sprite(_accessoryRoot, "Drip1", ArtId.Circle, skin.BodyColor, SortingOrders.Trykli,
                        new Vector2(-r * 0.45f, -r * 0.85f), new Vector2(r * 0.35f, r * 0.45f));
                    VisualFactory.Sprite(_accessoryRoot, "Drip2", ArtId.Circle, skin.BodyColor, SortingOrders.Trykli,
                        new Vector2(r * 0.3f, -r * 0.92f), new Vector2(r * 0.25f, r * 0.32f));
                    break;
            }
        }

        public void OnLaunched(Vector2 velocity)
        {
            _squash = -0.25f;
            if (velocity.sqrMagnitude > 0.01f) _squashAxis = velocity.normalized;
            _expression = Expression.Surprised;
        }

        public void OnImpact(Vector2 normal, float strength)
        {
            _squash = Mathf.Clamp(strength * 0.03f, 0.08f, 0.3f);
            _squashAxis = normal.sqrMagnitude > 0.01f ? normal.normalized : Vector2.up;
        }

        public void OnTeleported()
        {
            _teleportFlash = 1f;
        }

        public void SetUpsideDown(bool upsideDown)
        {
            _upsideDown = upsideDown;
        }

        public void SetHidden(bool hidden)
        {
            _hidden = hidden;
            _shape.gameObject.SetActive(!hidden);
            _face.gameObject.SetActive(!hidden);
        }

        public void PlayVictory(Vector2 goalCenter)
        {
            _victoryTimer = 0f;
            _victoryTarget = goalCenter;
            _expression = Expression.Happy;
        }

        private void LateUpdate()
        {
            if (_owner == null) return;
            float dt = Time.deltaTime;

            // Interpolated position while simulating; the face never rotates with the rolling body.
            Vector2 position = _owner.GetInterpolatedPosition(SimulationRunner.InterpolationAlpha);
            if (_victoryTimer >= 0f)
            {
                _victoryTimer += dt;
                float t = Mathf.Clamp01(_victoryTimer / 0.45f);
                position = Vector2.Lerp(_owner.CurrentPosition, _victoryTarget, MathUtils.EaseOutCubic(t));
                float scale = 1f - 0.85f * t;
                transform.localScale = new Vector3(scale, scale, 1f);
            }

            transform.position = new Vector3(position.x, position.y, transform.position.z);
            transform.rotation = Quaternion.identity;

            if (_owner.State == TrykliState.Dead) _expression = Expression.Dead;
            else if (_expression == Expression.Surprised && _owner.Speed < 6f && _owner.State != TrykliState.Bouncing) _expression = Expression.Normal;
            else if (_expression == Expression.Normal && _owner.Speed > 11f) _expression = Expression.Surprised;

            UpdateSquash(dt);
            UpdateEyes(dt);

            if (_teleportFlash > 0f)
            {
                _teleportFlash = Mathf.Max(0f, _teleportFlash - dt * 4f);
                float s = 1f - 0.3f * _teleportFlash;
                _shape.localScale = new Vector3(_shape.localScale.x * s, _shape.localScale.y * s, 1f);
            }
        }

        private void UpdateSquash(float dt)
        {
            _squash = Mathf.MoveTowards(_squash, 0f, dt * 1.6f);
            float idle = _owner.State == TrykliState.Idle && !_hidden ? Mathf.Sin(Time.time * 3f) * 0.03f : 0f;
            float amount = _squash + idle;

            // Stretch along the velocity when flying fast.
            if (Mathf.Abs(_squash) < 0.02f && _owner.Speed > 4f && _owner.State != TrykliState.Victory)
            {
                amount = -Mathf.Clamp((_owner.Speed - 4f) * 0.012f, 0f, 0.12f);
                _squashAxis = _owner.Velocity.normalized;
            }

            float angle = Mathf.Atan2(_squashAxis.y, _squashAxis.x) * Mathf.Rad2Deg - 90f;
            _shape.localRotation = Quaternion.Euler(0f, 0f, angle);
            _shape.localScale = new Vector3(1f + amount, 1f - amount, 1f);
        }

        private void UpdateEyes(float dt)
        {
            _blinkTimer -= dt;
            if (_blinkTimer <= 0f)
            {
                _blinkPhase = 0.12f;
                _blinkTimer = Random.Range(2.5f, 4.5f);
            }

            _blinkPhase = Mathf.Max(0f, _blinkPhase - dt);
            float eyeOpen = _blinkPhase > 0f ? 0.15f : 1f;
            float eyeScale = 1f;
            float pupilScale = 1f;
            switch (_expression)
            {
                case Expression.Surprised:
                    eyeScale = 1.25f;
                    pupilScale = 0.6f;
                    break;
                case Expression.Happy:
                    eyeOpen = 0.35f;
                    break;
                case Expression.Dead:
                    eyeOpen = 0.12f;
                    pupilScale = 0f;
                    break;
            }

            Vector2 look = _owner.Velocity;
            look = look.sqrMagnitude > 0.25f ? look.normalized * 0.22f : Vector2.zero;
            _face.localScale = new Vector3(1f, _upsideDown ? -1f : 1f, 1f);
            for (int i = 0; i < 2; i++)
            {
                _eyes[i].transform.localScale = new Vector3(_radius * 0.62f * eyeScale, _radius * 0.7f * eyeScale * eyeOpen, 1f);
                _pupils[i].transform.localPosition = new Vector3(look.x, _upsideDown ? -look.y : look.y, 0f);
                _pupils[i].transform.localScale = new Vector3(0.52f * pupilScale, 0.5f * pupilScale, 1f);
            }
        }
    }
}
