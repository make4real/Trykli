using Trykli.Audio;
using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Objectives;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Blocks a passage until opened by a button (active = open). The collider is disabled as soon as the
    /// door opens so the result never depends on the animation.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class Door : MechanismBase
    {
        [Header("Door")]
        [SerializeField] private Vector2 size = new Vector2(0.35f, 2.4f);
        [SerializeField] private float animationDuration = 0.35f;

        private float _openAmount;

        protected override void ApplyData(ElementData data)
        {
            size = OrDefault(data.size, size);
            // Doors are closed until a signal opens them unless configured otherwise.
            if (data.activation == ActivationMode.AlwaysActive) activation = ActivationMode.ActivatedBySignal;
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sliced(root, "Frame", ArtId.RoundedBox, ColorUtils.Hex("#37474F").WithAlpha(0.35f), SortingOrders.Blocks - 1, Vector2.one);
            VisualFactory.Sliced(root, "Panel", ArtId.RoundedBox, ColorUtils.Hex("#FFB300"), SortingOrders.Blocks + 1, Vector2.one);
            VisualFactory.Sprite(root, "Lock", ArtId.IconLock, ColorUtils.Hex("#5D4037"), SortingOrders.Blocks + 2, Vector2.zero, new Vector2(0.3f, 0.3f));
        }

        protected override void ApplyLayout(Transform root)
        {
            var box = GetComponent<BoxCollider2D>();
            box.size = size;
            gameObject.GetOrAddComponent<SurfaceTag>().kind = SurfaceKind.Obstacle;
            VisualFactory.SetSlicedSize(FindRenderer("Frame"), size);
            VisualFactory.SetSlicedSize(FindRenderer("Panel"), size);
            _openAmount = IsActive ? 1f : 0f;
            UpdatePanel();
        }

        protected override void RefreshActiveVisual()
        {
            GetComponent<BoxCollider2D>().enabled = !IsActive;
            if (Context == null) _openAmount = IsActive ? 1f : 0f;
            UpdatePanel();
        }

        protected override void OnActiveChanged(bool active)
        {
            if (Context == null) return;
            if (active) Context.Tracker.RecordCounter(LevelRunStats.Counters.DoorOpened);
            AudioManager.PlaySfx(SfxId.Door);
        }

        private void UpdatePanel()
        {
            SpriteRenderer panel = FindRenderer("Panel");
            if (panel == null) return;
            float visibleLength = size.y * (1f - _openAmount * 0.92f);
            VisualFactory.SetSlicedSize(panel, new Vector2(size.x, Mathf.Max(0.05f, visibleLength)));
            panel.transform.localPosition = new Vector3(0f, (size.y - visibleLength) * 0.5f, 0f);
            Transform lockIcon = FindVisual("Lock");
            if (lockIcon != null) lockIcon.gameObject.SetActive(_openAmount < 0.5f);
        }

        private void Update()
        {
            float target = IsActive ? 1f : 0f;
            if (Mathf.Approximately(_openAmount, target)) return;
            _openAmount = Mathf.MoveTowards(_openAmount, target, Time.deltaTime / Mathf.Max(0.01f, animationDuration));
            UpdatePanel();
        }
    }
}
