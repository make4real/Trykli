using System.Collections.Generic;
using Trykli.Audio;
using Trykli.Data;
using Trykli.Localization;
using Trykli.Placement;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Anti-frustration hints (GDD 16). After 3 failures a short tip is available, after 5 failures the
    /// "INDICE" button reveals progressively: 1) the important zone, 2) the recommended object, 3) its direction.
    /// Only the first step of the solution is ever revealed: the complete solution is never shown.
    /// </summary>
    public sealed class HintController : MonoBehaviour
    {
        public const int MaxHintLevel = 3;

        private readonly List<GameObject> _overlays = new List<GameObject>();
        private LevelData _level;

        public int HintLevel { get; private set; }

        public void ResetFor(LevelData level)
        {
            _level = level;
            HintLevel = 0;
            ClearOverlays();
        }

        public bool CanShowTip(int failures) => failures >= GameplayConfig.Instance.failuresBeforeTip;

        public bool CanRequestHint(int failures) =>
            failures >= GameplayConfig.Instance.failuresBeforeHint && HintLevel < MaxHintLevel && ResolveHint() != null;

        public string GetTipText()
        {
            if (_level == null) return string.Empty;
            if (!string.IsNullOrEmpty(_level.tipKey) && Loc.Has(_level.tipKey)) return Loc.Get(_level.tipKey);
            HintData hint = ResolveHint();
            if (hint != null && !string.IsNullOrEmpty(hint.itemId)) return Loc.Get("tip.item." + hint.itemId);
            return Loc.Get("tip.generic");
        }

        public void RequestNextHint(LevelContext context)
        {
            if (HintLevel >= MaxHintLevel) return;
            HintLevel++;
            AudioManager.PlaySfx(SfxId.Hint);
            Apply(context);
        }

        /// <summary>(Re)creates the hint visuals for the current hint level (called after every level rebuild).</summary>
        public void Apply(LevelContext context)
        {
            ClearOverlays();
            if (context == null || HintLevel <= 0) return;
            HintData hint = ResolveHint();
            if (hint == null || hint.zoneIndex < 0) return;

            PlacementZone zone = null;
            foreach (PlacementZone candidate in context.Zones)
            {
                if (candidate != null && candidate.Index == hint.zoneIndex) zone = candidate;
            }

            if (zone == null) return;
            zone.SetHinted(true);
            Vector2 position = zone.Position;

            if (HintLevel >= 2 && !string.IsNullOrEmpty(hint.itemId))
            {
                ItemDefinition item = context.Level != null ? FindItem(context.Level, hint.itemId) : null;
                if (item != null)
                {
                    SpriteRenderer ghost = VisualFactory.Sprite(context.transform, "HintItem", ArtId.Circle, Color.white.WithAlpha(0.55f),
                        SortingOrders.Hints, position + new Vector2(0f, 1.1f), new Vector2(0.8f, 0.8f));
                    ghost.sprite = item.GetIcon();
                    _overlays.Add(ghost.gameObject);
                }
            }

            if (HintLevel >= 3 && hint.showDirection)
            {
                SpriteRenderer arrow = VisualFactory.Sprite(context.transform, "HintArrow", ArtId.Arrow, new Color(1f, 0.85f, 0.2f, 0.9f),
                    SortingOrders.Hints, position, new Vector2(0.9f, 1.4f), hint.rotation);
                arrow.transform.position = position + MathUtils.UpFromRotation(hint.rotation) * 0.9f;
                _overlays.Add(arrow.gameObject);
            }
        }

        private HintData ResolveHint()
        {
            if (_level == null) return null;
            if (_level.hintData != null && _level.hintData.Count > 0) return _level.hintData[0];
            if (_level.solution == null || _level.solution.Count == 0) return null;
            SolutionStep step = _level.solution[0];
            ItemDefinition item = FindItem(_level, step.itemId);
            return new HintData
            {
                zoneIndex = step.zoneIndex,
                itemId = step.itemId,
                rotation = step.rotation,
                showDirection = item != null && item.rotatable
            };
        }

        private static ItemDefinition FindItem(LevelData level, string itemId)
        {
            foreach (ItemStack stack in level.availableItems)
            {
                if (stack.item != null && stack.item.itemId == itemId) return stack.item;
            }

            return null;
        }

        private void ClearOverlays()
        {
            foreach (GameObject overlay in _overlays)
            {
                if (overlay != null) Destroy(overlay);
            }

            _overlays.Clear();
        }
    }
}
