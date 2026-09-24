using System.Collections.Generic;
using Trykli.Core;
using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Localization;
using Trykli.Objectives;
using Trykli.Placement;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>
    /// Gameplay interface (GDD 34): top = pause, level number, objectives / stars; bottom = inventory,
    /// GO, reset, restart, hint, x1/x2 speed; selection tools (rotate / delete) and the inventory bar
    /// acting as a trash while an object is dragged.
    /// </summary>
    public sealed class GameplayHUD : MonoBehaviour
    {
        private const float TopBarHeight = 150f;
        private const float BottomHeight = 400f;

        private sealed class InventorySlot
        {
            public string ItemId;
            public UIButton Button;
            public TextMeshProUGUI Count;
        }

        private readonly List<InventorySlot> _slots = new List<InventorySlot>();
        private readonly List<TextMeshProUGUI> _objectiveTexts = new List<TextMeshProUGUI>();

        private LevelManager _level;
        private RectTransform _safe;
        private RectTransform _topBar;
        private RectTransform _objectivesCard;
        private RectTransform _objectivesList;
        private RectTransform _bottom;
        private RectTransform _inventoryRow;
        private Image _inventoryBackground;
        private RectTransform _trashOverlay;
        private RectTransform _selectionTools;
        private UIButton _rotateLeft;
        private UIButton _rotateRight;
        private TextMeshProUGUI _levelLabel;
        private UIButton _go;
        private UIButton _reset;
        private UIButton _restart;
        private UIButton _hint;
        private UIButton _speed;
        private TextMeshProUGUI _tipLabel;
        private TutorialOverlay _tutorial;
        private bool _objectivesCollapsed;
        private float _insetTimer;
        private Vector2 _lastInsets = new Vector2(-1f, -1f);

        public RectTransform SafeRoot => _safe;
        public RectTransform GoButtonRect => (RectTransform)_go.transform;
        public RectTransform RotateButtonRect => (RectTransform)_rotateRight.transform;

        public static GameplayHUD Create(LevelManager level)
        {
            Canvas canvas = UIFactory.CreateCanvas("GameplayCanvas", 20);
            var hud = canvas.gameObject.AddComponent<GameplayHUD>();
            hud.Build(canvas, level);
            return hud;
        }

        private void Build(Canvas canvas, LevelManager level)
        {
            _level = level;
            _safe = UIFactory.CreateSafeArea(canvas.transform);
            BuildTopBar();
            BuildObjectives();
            BuildBottom();
            BuildSelectionTools();

            _tipLabel = UIFactory.Text(_safe, "Tip", string.Empty, UITheme.BodySize, UITheme.TextLight);
            UIFactory.Anchor(_tipLabel.rectTransform, new Vector2(0.5f, 0f), new Vector2(980f, 110f), new Vector2(0f, BottomHeight + 150f), new Vector2(0.5f, 0f));
            _tipLabel.outlineWidth = 0.2f;
            _tipLabel.outlineColor = new Color32(20, 25, 40, 255);

            _tutorial = TutorialOverlay.Create(_safe, this, level);

            level.LevelLoaded += OnLevelLoaded;
            level.StateChanged += OnStateChanged;
            level.VictoryReady += OnVictoryReady;
            level.FailureReady += OnFailureReady;
            level.LevelRebuilt += RefreshAll;
            level.Placement.InventoryChanged += RefreshInventoryCounts;
            level.Placement.SelectionChanged += _ => RefreshSelectionTools();
            level.Placement.DragStateChanged += OnDragStateChanged;
            level.Placement.IsOverTrash = IsOverInventoryBar;
            Loc.LanguageChanged += RefreshAll;
        }

        private void OnDestroy()
        {
            Loc.LanguageChanged -= RefreshAll;
        }

        // ------------------------------------------------------------------ Build

        private void BuildTopBar()
        {
            _topBar = UIFactory.CreateRect("TopBar", _safe);
            UIFactory.Band(_topBar, true, TopBarHeight);
            UIButton pause = UIFactory.IconButton(_topBar, "Pause", ArtId.IconPause, UITheme.Neutral, OpenPause, 120f);
            UIFactory.Anchor((RectTransform)pause.transform, new Vector2(0f, 0.5f), new Vector2(120f, 120f), new Vector2(24f, 0f), new Vector2(0f, 0.5f));

            Image labelBackground = UIFactory.Panel(_topBar, "LevelLabel", UITheme.PanelDark, false);
            UIFactory.Anchor(labelBackground.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(700f, 110f), new Vector2(40f, 0f));
            _levelLabel = UIFactory.Text(labelBackground.transform, "Text", string.Empty, UITheme.SubheadingSize * 0.8f, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.Stretch(_levelLabel.rectTransform, 20f, 6f, 20f, 6f);
            UIFactory.AutoSize(_levelLabel, 26f, UITheme.SubheadingSize * 0.8f);
        }

        private void BuildObjectives()
        {
            Image card = UIFactory.Panel(_safe, "Objectives", UITheme.PanelDark, true);
            _objectivesCard = card.rectTransform;
            _objectivesCard.anchorMin = new Vector2(0f, 1f);
            _objectivesCard.anchorMax = new Vector2(1f, 1f);
            _objectivesCard.pivot = new Vector2(0.5f, 1f);
            _objectivesCard.offsetMin = new Vector2(24f, 0f);
            _objectivesCard.offsetMax = new Vector2(-24f, 0f);
            _objectivesCard.anchoredPosition = new Vector2(0f, -TopBarHeight - 6f);
            UIFactory.Vertical(card.gameObject, 4f, new RectOffset(26, 26, 14, 14), TextAnchor.UpperLeft);
            UIFactory.FitVertical(card.gameObject);
            var toggle = card.gameObject.AddComponent<Button>();
            toggle.transition = Selectable.Transition.None;
            toggle.onClick.AddListener(ToggleObjectives);
            _objectivesList = _objectivesCard;
        }

        private void BuildBottom()
        {
            _bottom = UIFactory.CreateRect("Bottom", _safe);
            UIFactory.Band(_bottom, false, BottomHeight);

            // Inventory bar (also the trash area while dragging a placed object).
            _inventoryBackground = UIFactory.Panel(_bottom, "InventoryBar", UITheme.PanelDark, true);
            RectTransform bar = _inventoryBackground.rectTransform;
            bar.anchorMin = new Vector2(0f, 0f);
            bar.anchorMax = new Vector2(1f, 0f);
            bar.pivot = new Vector2(0.5f, 0f);
            bar.offsetMin = new Vector2(16f, 12f);
            bar.offsetMax = new Vector2(-16f, 12f);
            bar.sizeDelta = new Vector2(-32f, 210f);

            _inventoryRow = UIFactory.CreateRect("Items", bar);
            UIFactory.Stretch(_inventoryRow, 20f, 18f, 20f, 18f);
            UIFactory.Horizontal(_inventoryRow.gameObject, 22f, null, TextAnchor.MiddleCenter);

            _trashOverlay = UIFactory.CreateRect("Trash", bar);
            UIFactory.Stretch(_trashOverlay);
            Image trashBackground = _trashOverlay.gameObject.AddComponent<Image>();
            trashBackground.sprite = PlaceholderArt.Get(ArtId.UIPanel);
            trashBackground.type = Image.Type.Sliced;
            trashBackground.color = UITheme.Danger.WithAlpha(0.85f);
            trashBackground.raycastTarget = false;
            RectTransform trashRow = UIFactory.CreateRect("Row", _trashOverlay);
            UIFactory.Stretch(trashRow);
            UIFactory.Horizontal(trashRow.gameObject, 20f);
            Image trashIcon = UIFactory.Image(trashRow, "Icon", ArtId.IconTrash, UITheme.TextLight);
            UIFactory.SetPreferredSize(trashIcon.gameObject, 100f, 100f);
            TextMeshProUGUI trashText = UIFactory.LocText(trashRow, "Text", "hud.trash", UITheme.BodySize, UITheme.TextLight, TextAlignmentOptions.Left, FontStyles.Bold);
            UIFactory.SetPreferredSize(trashText.gameObject, 600f, 100f);
            _trashOverlay.gameObject.SetActive(false);

            // Action row: reset, hint | GO | speed, restart.
            RectTransform actions = UIFactory.CreateRect("Actions", _bottom);
            actions.anchorMin = new Vector2(0f, 0f);
            actions.anchorMax = new Vector2(1f, 0f);
            actions.pivot = new Vector2(0.5f, 0f);
            actions.sizeDelta = new Vector2(0f, 170f);
            actions.anchoredPosition = new Vector2(0f, 230f);

            _reset = UIFactory.IconButton(actions, "Reset", ArtId.IconReset, UITheme.Neutral, () => _level.ResetLevel(), 130f);
            UIFactory.Anchor((RectTransform)_reset.transform, new Vector2(0f, 0.5f), new Vector2(130f, 130f), new Vector2(30f, 0f), new Vector2(0f, 0.5f));
            _hint = UIFactory.IconButton(actions, "Hint", ArtId.IconHint, UITheme.Star, () => _level.RequestHint(), 130f);
            UIFactory.Anchor((RectTransform)_hint.transform, new Vector2(0f, 0.5f), new Vector2(130f, 130f), new Vector2(180f, 0f), new Vector2(0f, 0.5f));
            UITween.Pulse(_hint, 0.05f);

            _go = UIFactory.Button(actions, "GO", "hud.go", UITheme.Primary, OnGoPressed, null, 80f);
            UIFactory.Anchor((RectTransform)_go.transform, new Vector2(0.5f, 0.5f), new Vector2(300f, 170f), new Vector2(0f, 10f));

            _restart = UIFactory.IconButton(actions, "Restart", ArtId.IconRestart, UITheme.Secondary, () => _level.Restart(), 130f);
            UIFactory.Anchor((RectTransform)_restart.transform, new Vector2(1f, 0.5f), new Vector2(130f, 130f), new Vector2(-30f, 0f), new Vector2(1f, 0.5f));
            _speed = UIFactory.Button(actions, "Speed", null, UITheme.Neutral, ToggleSpeed, null, UITheme.BodySize);
            UIFactory.Anchor((RectTransform)_speed.transform, new Vector2(1f, 0.5f), new Vector2(130f, 130f), new Vector2(-180f, 0f), new Vector2(1f, 0.5f));
            TextMeshProUGUI speedText = UIFactory.Text(_speed.transform, "SpeedText", "x1", UITheme.SubheadingSize, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.Stretch(speedText.rectTransform);
            _speed.Label = speedText;
        }

        private void BuildSelectionTools()
        {
            _selectionTools = UIFactory.CreateRect("SelectionTools", _safe);
            UIFactory.Anchor(_selectionTools, new Vector2(0.5f, 0f), new Vector2(520f, 140f), new Vector2(0f, BottomHeight + 10f), new Vector2(0.5f, 0f));
            UIFactory.Horizontal(_selectionTools.gameObject, 30f);
            _rotateLeft = UIFactory.IconButton(_selectionTools, "RotateLeft", ArtId.IconRotateLeft, UITheme.Secondary, () => _level.Placement.RotateSelected(1), 130f);
            _rotateRight = UIFactory.IconButton(_selectionTools, "RotateRight", ArtId.IconRotateRight, UITheme.Secondary, () => _level.Placement.RotateSelected(-1), 130f);
            UIFactory.IconButton(_selectionTools, "Delete", ArtId.IconTrash, UITheme.Danger, () => _level.Placement.DeleteSelected(), 130f);
            _selectionTools.gameObject.SetActive(false);
        }

        // ------------------------------------------------------------------ Refresh

        private void OnLevelLoaded(LevelData level)
        {
            _objectivesCollapsed = false;
            RefreshAll();
        }

        private void RefreshAll()
        {
            if (_level == null || _level.Level == null) return;
            LevelData level = _level.Level;
            _levelLabel.text = Loc.Format("hud.level", level.Code, level.DisplayName);
            RebuildObjectives();
            RebuildInventory();
            OnStateChanged(_level.State);
        }

        private void RebuildObjectives()
        {
            foreach (Transform child in _objectivesList) Destroy(child.gameObject);
            _objectiveTexts.Clear();
            IReadOnlyList<StarObjective> objectives = _level.Level.starObjectives;
            int best = GameManager.Instance.Progression.GetStars(_level.Level.levelId);
            for (int i = 0; i < objectives.Count && i < 3; i++)
            {
                RectTransform row = UIFactory.CreateRect("Objective" + i, _objectivesList);
                UIFactory.Horizontal(row.gameObject, 14f, null, TextAnchor.MiddleLeft);
                UIFactory.SetPreferredSize(row.gameObject, -1f, _objectivesCollapsed ? 0f : 52f);
                Image star = UIFactory.Image(row, "Star", i < best ? ArtId.Star : ArtId.StarEmpty, i < best ? UITheme.Star : UITheme.StarEmpty);
                UIFactory.SetPreferredSize(star.gameObject, 46f, 46f);
                TextMeshProUGUI text = UIFactory.Text(row, "Text", objectives[i] != null ? objectives[i].GetDescription() : "-", 34f, UITheme.TextLight, TextAlignmentOptions.Left);
                UIFactory.SetPreferredSize(text.gameObject, -1f, 50f, 1f);
                UIFactory.AutoSize(text, 22f, 34f);
                _objectiveTexts.Add(text);
                row.gameObject.SetActive(!_objectivesCollapsed || i == 0);
                if (_objectivesCollapsed) text.text = Loc.Get("hud.objectives_collapsed");
            }
        }

        private void ToggleObjectives()
        {
            _objectivesCollapsed = !_objectivesCollapsed;
            RebuildObjectives();
            _insetTimer = 0f;
        }

        private void RebuildInventory()
        {
            foreach (InventorySlot slot in _slots)
            {
                if (slot.Button != null) Destroy(slot.Button.gameObject);
            }

            _slots.Clear();
            foreach (ItemStack stack in _level.Level.availableItems)
            {
                if (stack.item == null) continue;
                ItemDefinition item = stack.item;
                UIButton button = UIFactory.Button(_inventoryRow, "Slot_" + item.itemId, null, item.color.Darken(0.45f), null);
                button.PlayClickSound = false;
                UIFactory.SetPreferredSize(button.gameObject, 170f, 170f);
                Transform content = button.transform.Find("Content");
                content.GetComponent<HorizontalLayoutGroup>().enabled = false;
                Image icon = UIFactory.Image(content, "Icon", item.icon, Color.white);
                icon.sprite = item.GetIcon();
                UIFactory.Stretch(icon.rectTransform, 10f, 0f, 10f, 30f);
                TextMeshProUGUI count = UIFactory.Text(content, "Count", string.Empty, 34f, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold);
                UIFactory.Anchor(count.rectTransform, new Vector2(0.5f, 0f), new Vector2(150f, 40f), new Vector2(0f, -4f), new Vector2(0.5f, 0f));
                button.PointerDown += () => _level.Placement.BeginInventoryDrag(item);
                _slots.Add(new InventorySlot { ItemId = item.itemId, Button = button, Count = count });
            }

            RefreshInventoryCounts();
        }

        private void RefreshInventoryCounts()
        {
            InventoryModel inventory = _level.Placement.Inventory;
            foreach (InventorySlot slot in _slots)
            {
                ItemDefinition item = _level.Placement.GetDefinition(slot.ItemId);
                int remaining = inventory.RemainingPieces(slot.ItemId);
                if (item != null && item.kind == ItemKind.Portal)
                {
                    string next = remaining == 0 ? "-" : remaining % 2 == 0 ? "A" : "B";
                    slot.Count.text = Loc.Format("hud.portal_count", next, remaining);
                }
                else
                {
                    slot.Count.text = "x" + remaining;
                }

                slot.Button.Interactable = remaining > 0 && _level.State == GameState.Placement;
            }
        }

        private void OnStateChanged(GameState state)
        {
            bool placement = state == GameState.Placement;
            bool simulation = state == GameState.Simulation;
            _go.gameObject.SetActive(placement || simulation);
            _go.SetLabelKey(simulation ? "hud.stop" : "hud.go");
            _go.SetColor(simulation ? UITheme.Neutral : UITheme.Primary);
            _restart.gameObject.SetActive(simulation);
            _speed.gameObject.SetActive(simulation || placement);
            _speed.Label.text = _level.IsFastSimulation ? "x2" : "x1";
            _reset.gameObject.SetActive(placement || simulation);
            _hint.gameObject.SetActive(placement && _level.Session != null && _level.Hints.CanRequestHint(_level.Session.Failures));
            _inventoryRow.gameObject.SetActive(true);
            RefreshInventoryCounts();
            RefreshSelectionTools();
            _tipLabel.text = placement && _level.Session != null && _level.Hints.CanShowTip(_level.Session.Failures) ? _level.Hints.GetTipText() : string.Empty;
        }

        private void RefreshSelectionTools()
        {
            PlacedItem selected = _level.Placement.Selected;
            bool visible = selected != null && _level.State == GameState.Placement && !_level.Placement.IsDragging;
            _selectionTools.gameObject.SetActive(visible);
            if (!visible) return;
            bool rotatable = _level.Placement.CanRotateSelected();
            _rotateLeft.gameObject.SetActive(rotatable);
            _rotateRight.gameObject.SetActive(rotatable);
        }

        private void OnDragStateChanged(bool dragging)
        {
            RefreshSelectionTools();
            _trashOverlay.gameObject.SetActive(false);
        }

        // ------------------------------------------------------------------ Actions

        private void OnGoPressed()
        {
            if (_level.State == GameState.Placement) _level.Go();
            else if (_level.State == GameState.Simulation) _level.Modify();
        }

        private void ToggleSpeed()
        {
            _level.SetFastSimulation(!_level.IsFastSimulation);
            _speed.Label.text = _level.IsFastSimulation ? "x2" : "x1";
        }

        private void OpenPause()
        {
            if (_level.State == GameState.Victory || _level.State == GameState.Pause) return;
            _level.Pause();
            var panel = ModalPanel.Open<PausePanel>(_safe);
            panel.Level = _level;
        }

        private void OnVictoryReady()
        {
            var panel = ModalPanel.Open<VictoryPanel>(_safe);
            panel.Level = _level;
            panel.MasterRequested = () => ModalPanel.Open<MasterPanel>(_safe);
            panel.Populate();
        }

        private void OnFailureReady()
        {
            var panel = ModalPanel.Open<FailurePanel>(_safe);
            panel.Level = _level;
            panel.HintRequested = () =>
            {
                _level.Modify();
                _level.RequestHint();
                Toast.Show(_safe, Loc.Get("hint.shown"));
            };
            panel.Populate();
            _tutorial.OnFailureShown();
        }

        private bool IsOverInventoryBar(Vector2 screenPosition)
        {
            if (_inventoryBackground == null) return false;
            bool over = RectTransformUtility.RectangleContainsScreenPoint(_inventoryBackground.rectTransform, screenPosition, null);
            // The trash overlay is shown only when an already placed object is dragged over the bar.
            _trashOverlay.gameObject.SetActive(over && _level.Placement.IsDragging);
            return over;
        }

        public Vector2 GetSlotScreenPosition(int index)
        {
            if (index < 0 || index >= _slots.Count) return Vector2.zero;
            return RectTransformUtility.WorldToScreenPoint(null, _slots[index].Button.transform.position);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OpenPause();

            _insetTimer -= Time.unscaledDeltaTime;
            if (_insetTimer > 0f) return;
            _insetTimer = 0.25f;
            UpdateCameraInsets();
        }

        private void UpdateCameraInsets()
        {
            if (_level == null || _level.CameraRig == null) return;
            var corners = new Vector3[4];
            _objectivesCard.GetWorldCorners(corners);
            float top = Screen.height - corners[0].y;
            _bottom.GetWorldCorners(corners);
            float bottom = corners[1].y - 60f * _safe.lossyScale.y;
            var insets = new Vector2(top, Mathf.Max(0f, bottom));
            if ((insets - _lastInsets).sqrMagnitude < 1f) return;
            _lastInsets = insets;
            _level.CameraRig.SetScreenInsets(insets.x, insets.y);
        }
    }
}
