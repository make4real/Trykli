using System;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>
    /// Base class of every popup (pause, victory, failure, settings...). Dims the screen, blocks the input
    /// below and shows a rounded card with a vertical layout.
    /// </summary>
    public abstract class ModalPanel : MonoBehaviour
    {
        private bool _closing;
        private float _closeTime;
        private CanvasGroup _group;

        protected RectTransform Card { get; private set; }
        protected RectTransform Content { get; private set; }

        public event Action Closed;

        /// <summary>Bottom sheet (e.g. failure panel) instead of a centered card.</summary>
        protected virtual bool IsBottomSheet => false;
        protected virtual Color CardColor => UITheme.Panel;
        protected virtual bool CloseOnDimmerTap => false;

        public static T Open<T>(Transform parent) where T : ModalPanel
        {
            RectTransform root = UIFactory.CreateRect(typeof(T).Name, parent);
            UIFactory.Stretch(root);
            var panel = root.gameObject.AddComponent<T>();
            panel.Setup();
            return panel;
        }

        private void Setup()
        {
            _group = gameObject.AddComponent<CanvasGroup>();
            Image dimmer = UIFactory.Dimmer(transform);
            if (CloseOnDimmerTap)
            {
                var button = dimmer.gameObject.AddComponent<Button>();
                button.transition = Selectable.Transition.None;
                button.onClick.AddListener(Close);
            }

            Image card = UIFactory.Panel(transform, "Card", CardColor);
            Card = card.rectTransform;
            if (IsBottomSheet)
            {
                Card.anchorMin = new Vector2(0f, 0f);
                Card.anchorMax = new Vector2(1f, 0f);
                Card.pivot = new Vector2(0.5f, 0f);
                Card.offsetMin = new Vector2(30f, 30f);
                Card.offsetMax = new Vector2(-30f, 30f);
            }
            else
            {
                Card.anchorMin = new Vector2(0f, 0.5f);
                Card.anchorMax = new Vector2(1f, 0.5f);
                Card.pivot = new Vector2(0.5f, 0.5f);
                Card.offsetMin = new Vector2(60f, 0f);
                Card.offsetMax = new Vector2(-60f, 0f);
            }

            UIFactory.Vertical(Card.gameObject, 28f, new RectOffset(56, 56, 56, 56));
            UIFactory.FitVertical(Card.gameObject);
            Content = Card;
            Build();
            UITween.PopIn(Card, 0f, 0.28f);
        }

        /// <summary>Creates the panel content inside <see cref="Content"/>.</summary>
        protected abstract void Build();

        protected TextMeshProUGUI AddTitle(string key, float size = UITheme.HeadingSize, params object[] args)
        {
            TextMeshProUGUI title = UIFactory.LocText(Content, "Title", key, size, UITheme.TextDark, TextAlignmentOptions.Center, FontStyles.Bold, args);
            UIFactory.SetPreferredSize(title.gameObject, -1f, size * 1.4f);
            return title;
        }

        protected TextMeshProUGUI AddText(string key, float size = UITheme.BodySize, Color? color = null, params object[] args)
        {
            TextMeshProUGUI text = UIFactory.LocText(Content, "Text", key, size, color ?? UITheme.TextDark, TextAlignmentOptions.Center, FontStyles.Normal, args);
            return text;
        }

        protected TextMeshProUGUI AddRawText(string text, float size = UITheme.BodySize, Color? color = null)
        {
            return UIFactory.Text(Content, "Text", text, size, color ?? UITheme.TextDark);
        }

        protected UIButton AddButton(string key, Color color, Action action, ArtId? icon = null, Transform parent = null)
        {
            UIButton button = UIFactory.Button(parent != null ? parent : Content, key, key, color, action, icon);
            UIFactory.SetPreferredSize(button.gameObject, -1f, UITheme.ButtonHeight, 1f);
            return button;
        }

        protected RectTransform AddRow(float height, float spacing = 24f)
        {
            RectTransform row = UIFactory.CreateRect("Row", Content);
            UIFactory.Horizontal(row.gameObject, spacing).childForceExpandWidth = true;
            UIFactory.SetPreferredSize(row.gameObject, -1f, height);
            return row;
        }

        public void Close()
        {
            if (_closing) return;
            _closing = true;
            _closeTime = 0f;
            _group.blocksRaycasts = false;
        }

        protected virtual void Update()
        {
            if (!_closing) return;
            _closeTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_closeTime / 0.15f);
            _group.alpha = 1f - t;
            if (t < 1f) return;
            Closed?.Invoke();
            Destroy(gameObject);
        }
    }

}
