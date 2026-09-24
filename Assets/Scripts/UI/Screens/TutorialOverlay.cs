using Trykli.Core;
using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Localization;
using Trykli.Placement;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>
    /// Visual tutorial (GDD 52): an animated finger shows how to drag the first object and press GO,
    /// how to rotate an object, and what RETRY / MODIFY do. No mandatory text tutorial.
    /// </summary>
    public sealed class TutorialOverlay : MonoBehaviour
    {
        private LevelManager _level;
        private GameplayHUD _hud;
        private RectTransform _hand;
        private RectTransform _bubble;
        private TextMeshProUGUI _bubbleText;
        private float _time;
        private string _currentKey;
        private bool _restartShown;

        public static TutorialOverlay Create(RectTransform parent, GameplayHUD hud, LevelManager level)
        {
            RectTransform root = UIFactory.CreateRect("Tutorial", parent);
            UIFactory.Stretch(root);
            var overlay = root.gameObject.AddComponent<TutorialOverlay>();
            overlay._level = level;
            overlay._hud = hud;

            Image bubble = UIFactory.Panel(root, "Bubble", UITheme.Panel, false);
            overlay._bubble = bubble.rectTransform;
            UIFactory.Anchor(overlay._bubble, new Vector2(0.5f, 0.5f), new Vector2(900f, 150f), new Vector2(0f, 250f));
            overlay._bubbleText = UIFactory.Text(bubble.transform, "Text", string.Empty, UITheme.BodySize, UITheme.TextDark);
            UIFactory.Stretch(overlay._bubbleText.rectTransform, 30f, 10f, 30f, 10f);
            UIFactory.AutoSize(overlay._bubbleText, 28f, UITheme.BodySize);

            Image hand = UIFactory.Image(root, "Hand", ArtId.IconFinger, Color.white);
            overlay._hand = hand.rectTransform;
            overlay._hand.sizeDelta = new Vector2(150f, 150f);
            overlay._hand.pivot = new Vector2(0.5f, 0.85f);
            var outline = hand.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.6f);
            outline.effectDistance = new Vector2(4f, -4f);
            overlay.Hide();
            return overlay;
        }

        public void OnFailureShown()
        {
            if (_restartShown || _level.Level == null || _level.Level.tutorial != TutorialKind.Restart) return;
            _restartShown = true;
            Toast.Show(_hud.SafeRoot, Loc.Get("tuto.restart"), 4.5f);
        }

        private void Update()
        {
            if (_level == null || _level.Level == null || _level.Session == null)
            {
                Hide();
                return;
            }

            _time += Time.unscaledDeltaTime;
            switch (_level.Level.tutorial)
            {
                case TutorialKind.DragAndGo:
                    UpdateDragAndGo();
                    break;
                case TutorialKind.Rotate:
                    UpdateRotate();
                    break;
                default:
                    Hide();
                    break;
            }
        }

        private void UpdateDragAndGo()
        {
            if (_level.State != GameState.Placement || _level.Session.Attempts > 0 || _level.Placement.IsDragging)
            {
                Hide();
                return;
            }

            if (_level.Placement.PlacedItems.Count == 0)
            {
                Vector2 from = _hud.GetSlotScreenPosition(0);
                Vector2 to = ZoneScreenPosition();
                float t = Mathf.Repeat(_time / 1.8f, 1f);
                float move = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((t - 0.15f) / 0.6f));
                ShowHand(Vector2.Lerp(from, to, move), t < 0.9f);
                ShowBubble("tuto.drag");
            }
            else
            {
                Vector2 go = RectTransformUtility.WorldToScreenPoint(null, _hud.GoButtonRect.position);
                ShowHand(go + new Vector2(0f, Mathf.Abs(Mathf.Sin(_time * 4f)) * 30f), true);
                ShowBubble("tuto.go");
            }
        }

        private void UpdateRotate()
        {
            bool show = _level.State == GameState.Placement && _level.Session.Attempts < 2 && _level.Placement.Selected != null &&
                        _level.Placement.CanRotateSelected() && !_level.Placement.IsDragging;
            if (!show)
            {
                if (_level.State == GameState.Placement && _level.Session.Attempts == 0 && _level.Placement.PlacedItems.Count == 0)
                {
                    _hand.gameObject.SetActive(false);
                    ShowBubble("tuto.rotate_intro");
                }
                else
                {
                    Hide();
                }

                return;
            }

            Vector2 rotate = RectTransformUtility.WorldToScreenPoint(null, _hud.RotateButtonRect.position);
            ShowHand(rotate + new Vector2(0f, Mathf.Abs(Mathf.Sin(_time * 4f)) * 30f), true);
            ShowBubble("tuto.rotate");
        }

        private Vector2 ZoneScreenPosition()
        {
            Camera camera = _level.CameraRig != null ? _level.CameraRig.Camera : Camera.main;
            LevelContext context = _level.Context;
            if (camera == null || context == null || context.Zones.Count == 0) return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            int zoneIndex = _level.Level.solution.Count > 0 ? _level.Level.solution[0].zoneIndex : 0;
            PlacementZone zone = context.Zones[Mathf.Clamp(zoneIndex, 0, context.Zones.Count - 1)];
            return camera.WorldToScreenPoint(zone.Position);
        }

        private void ShowHand(Vector2 screenPosition, bool visible)
        {
            _hand.gameObject.SetActive(visible);
            _hand.position = screenPosition;
        }

        private void ShowBubble(string key)
        {
            _bubble.gameObject.SetActive(true);
            if (_currentKey == key) return;
            _currentKey = key;
            _bubbleText.text = Loc.Get(key);
            UITween.PopIn(_bubble, 0f, 0.25f);
        }

        private void Hide()
        {
            if (_hand != null) _hand.gameObject.SetActive(false);
            if (_bubble != null) _bubble.gameObject.SetActive(false);
            _currentKey = null;
        }
    }
}
