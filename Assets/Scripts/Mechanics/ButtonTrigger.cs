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
    /// Floor button pressed when Trykli rolls / lands on it. Emits its channel on the level's switch bus:
    /// doors, lasers, magnets, fans, platforms and bombs listening to that channel react.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class ButtonTrigger : MechanismBase
    {
        [Header("Button")]
        [SerializeField] private Vector2 size = new Vector2(0.9f, 0.2f);
        [SerializeField] private int emitChannel;
        [Tooltip("Stays pressed after the first activation.")]
        [SerializeField] private bool oneShot = true;

        private bool _pressed;
        private bool _trykliOnButton;

        public bool Pressed => _pressed;
        public int EmitChannel => emitChannel;

        protected override void ApplyData(ElementData data)
        {
            size = OrDefault(data.size, size);
            emitChannel = data.emitChannel;
            oneShot = data.oneShot;
            // A button never listens to signals itself.
            activation = ActivationMode.AlwaysActive;
            listenChannel = -1;
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sliced(root, "Base", ArtId.RoundedBox, ColorUtils.Hex("#455A64"), SortingOrders.Mechanisms, Vector2.one);
            VisualFactory.Sliced(root, "Cap", ArtId.RoundedBox, ColorUtils.Hex("#FF5252"), SortingOrders.Mechanisms + 1, Vector2.one);
        }

        protected override void ApplyLayout(Transform root)
        {
            var box = GetComponent<BoxCollider2D>();
            box.size = new Vector2(size.x, size.y * 0.5f);
            box.offset = new Vector2(0f, -size.y * 0.25f);
            VisualFactory.SetSlicedSize(FindRenderer("Base"), new Vector2(size.x * 1.15f, size.y * 0.5f));
            Transform baseT = FindVisual("Base");
            if (baseT != null) baseT.localPosition = new Vector3(0f, -size.y * 0.25f, 0f);
            UpdateCap();
        }

        private void UpdateCap()
        {
            SpriteRenderer cap = FindRenderer("Cap");
            if (cap == null) return;
            VisualFactory.SetSlicedSize(cap, new Vector2(size.x * 0.8f, size.y * 0.6f));
            cap.transform.localPosition = new Vector3(0f, _pressed ? size.y * 0.05f : size.y * 0.3f, 0f);
            cap.color = _pressed ? ColorUtils.Hex("#2ECC71") : ColorUtils.Hex("#FF5252");
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (trykli == null || !trykli.IsAlive) return;
            float rotation = ElementGeometry.RotationOf(transform);
            Vector2 local = MathUtils.ToLocal(trykli.Position, transform.position, rotation);
            bool touching = Mathf.Abs(local.x) <= size.x * 0.5f + trykli.Radius * 0.3f &&
                            local.y >= -size.y * 0.5f && local.y <= size.y * 0.5f + trykli.Radius + 0.1f;

            if (touching && !_trykliOnButton && (!_pressed || !oneShot)) Press();
            _trykliOnButton = touching;
        }

        private void Press()
        {
            _pressed = !oneShot ? !_pressed : true;
            UpdateCap();
            Context.Tracker.RecordCounter(LevelRunStats.Counters.ButtonPressed);
            if (!string.IsNullOrEmpty(elementId)) Context.Tracker.RecordCounter(LevelRunStats.Counters.Button(elementId));
            AudioManager.PlaySfx(SfxId.Button);
            Haptics.Play(HapticType.Light);
            Context.Switches.Emit(emitChannel);
        }
    }
}
