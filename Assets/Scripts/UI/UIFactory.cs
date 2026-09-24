using System;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>
    /// Builds every UI element in code (Canvas, panels, TextMeshPro texts, large buttons, layouts).
    /// No UI prefab is required, and every text goes through the localization system.
    /// </summary>
    public static class UIFactory
    {
        private static bool _fontWarningShown;

        public static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        public static Canvas CreateCanvas(string name, int sortingOrder)
        {
            EnsureEventSystem();
            var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = UITheme.ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        /// <summary>Full screen child constrained to the device safe area.</summary>
        public static RectTransform CreateSafeArea(Transform canvas)
        {
            RectTransform rect = CreateRect("SafeArea", canvas);
            Stretch(rect);
            rect.gameObject.AddComponent<SafeArea>();
            return rect;
        }

        public static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static RectTransform Stretch(RectTransform rect, float left = 0f, float top = 0f, float right = 0f, float bottom = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
            return rect;
        }

        /// <summary>Anchors a rect to a point of its parent (x, y in 0..1) with a fixed size.</summary>
        public static RectTransform Anchor(RectTransform rect, Vector2 anchor, Vector2 size, Vector2 offset, Vector2? pivot = null)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot ?? anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = offset;
            return rect;
        }

        /// <summary>Horizontal band stretched across the parent width, anchored at the top (y=1) or bottom (y=0).</summary>
        public static RectTransform Band(RectTransform rect, bool top, float height, float offset = 0f)
        {
            float y = top ? 1f : 0f;
            rect.anchorMin = new Vector2(0f, y);
            rect.anchorMax = new Vector2(1f, y);
            rect.pivot = new Vector2(0.5f, y);
            rect.sizeDelta = new Vector2(0f, height);
            rect.anchoredPosition = new Vector2(0f, top ? -offset : offset);
            return rect;
        }

        public static Image Image(Transform parent, string name, ArtId art, Color color, bool raycast = false)
        {
            RectTransform rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = PlaceholderArt.Get(art);
            image.color = color;
            image.raycastTarget = raycast;
            PlaceholderArt.SpriteSpec spec = PlaceholderArt.GetSpec(art);
            if (spec.Border != Vector4.zero)
            {
                image.type = UnityEngine.UI.Image.Type.Sliced;
                image.pixelsPerUnitMultiplier = 1f;
            }
            else
            {
                image.preserveAspect = true;
            }

            return image;
        }

        public static Image Panel(Transform parent, string name, Color color, bool raycast = true)
        {
            return Image(parent, name, ArtId.UIPanel, color, raycast);
        }

        /// <summary>Full-screen dimmer that blocks the input below modal panels.</summary>
        public static Image Dimmer(Transform parent)
        {
            Image image = Image(parent, "Dimmer", ArtId.Square, UITheme.Dim, true);
            image.preserveAspect = false;
            Stretch(image.rectTransform, -200f, -400f, -200f, -400f);
            return image;
        }

        public static TextMeshProUGUI Text(Transform parent, string name, string text, float size, Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.Center, FontStyles style = FontStyles.Normal)
        {
            RectTransform rect = CreateRect(name, parent);
            var tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
            if (tmp.font == null)
            {
                TMP_FontAsset font = UIFonts.Default;
                if (font != null) tmp.font = font;
            }

            tmp.text = text ?? string.Empty;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.fontStyle = style;
            tmp.raycastTarget = false;
#if UNITY_6000_0_OR_NEWER
            tmp.textWrappingMode = TextWrappingModes.Normal;
#else
            tmp.enableWordWrapping = true;
#endif
            tmp.overflowMode = TextOverflowModes.Overflow;
            return tmp;
        }

        public static TextMeshProUGUI LocText(Transform parent, string name, string key, float size, Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.Center, FontStyles style = FontStyles.Normal, params object[] args)
        {
            TextMeshProUGUI tmp = Text(parent, name, string.Empty, size, color, alignment, style);
            tmp.gameObject.AddComponent<LocalizedText>().SetKey(key, args);
            return tmp;
        }

        public static void AutoSize(TMP_Text text, float min, float max)
        {
            text.enableAutoSizing = true;
            text.fontSizeMin = min;
            text.fontSizeMax = max;
        }

        /// <summary>Large rounded button with a localized label and an optional icon.</summary>
        public static UIButton Button(Transform parent, string name, string labelKey, Color color, Action onClick,
            ArtId? icon = null, float fontSize = UITheme.SubheadingSize, Color? textColor = null)
        {
            RectTransform rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = PlaceholderArt.Get(ArtId.UIPanel);
            image.type = UnityEngine.UI.Image.Type.Sliced;
            image.color = color;
            rect.gameObject.AddComponent<Button>();
            var button = rect.gameObject.AddComponent<UIButton>();

            // Subtle bottom shade gives the button some depth.
            Image shade = Image(rect, "Shade", ArtId.UIPanel, new Color(0f, 0f, 0f, 0.18f));
            Stretch(shade.rectTransform, 0f, 12f, 0f, -8f);
            shade.transform.SetAsFirstSibling();

            var content = CreateRect("Content", rect);
            Stretch(content, 24f, 8f, 24f, 8f);
            var layout = content.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 18f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            if (icon.HasValue)
            {
                Image iconImage = Image(content, "Icon", icon.Value, textColor ?? UITheme.TextLight);
                var iconLayout = iconImage.gameObject.AddComponent<LayoutElement>();
                iconLayout.preferredWidth = fontSize * 1.3f;
                iconLayout.preferredHeight = fontSize * 1.3f;
                button.Icon = iconImage;
            }

            if (!string.IsNullOrEmpty(labelKey))
            {
                TextMeshProUGUI label = LocText(content, "Label", labelKey, fontSize, textColor ?? UITheme.TextLight,
                    TextAlignmentOptions.Center, FontStyles.Bold);
                AutoSize(label, fontSize * 0.55f, fontSize);
                var labelLayout = label.gameObject.AddComponent<LayoutElement>();
                labelLayout.flexibleWidth = 1f;
                button.Label = label;
                button.LocalizedLabel = label.GetComponent<LocalizedText>();
            }

            button.SetColor(color);
            button.OnClick(onClick);
            return button;
        }

        /// <summary>Square icon-only button (pause, rotate, delete...).</summary>
        public static UIButton IconButton(Transform parent, string name, ArtId icon, Color color, Action onClick, float size = UITheme.MinTouchSize)
        {
            UIButton button = Button(parent, name, null, color, onClick);
            RectTransform rect = (RectTransform)button.transform;
            rect.sizeDelta = new Vector2(size, size);
            Transform content = rect.Find("Content");
            if (content != null) UnityEngine.Object.Destroy(content.gameObject);
            Image iconImage = Image(rect, "Icon", icon, UITheme.TextLight);
            Stretch(iconImage.rectTransform, size * 0.2f, size * 0.2f, size * 0.2f, size * 0.2f);
            button.Icon = iconImage;
            SetPreferredSize(rect.gameObject, size, size);
            return button;
        }

        public static LayoutElement SetPreferredSize(GameObject go, float width, float height, float flexibleWidth = -1f, float flexibleHeight = -1f)
        {
            var layout = go.GetComponent<LayoutElement>();
            if (layout == null) layout = go.AddComponent<LayoutElement>();
            if (width >= 0f)
            {
                layout.preferredWidth = width;
                layout.minWidth = width;
            }

            if (height >= 0f)
            {
                layout.preferredHeight = height;
                layout.minHeight = height;
            }

            layout.flexibleWidth = flexibleWidth;
            layout.flexibleHeight = flexibleHeight;
            return layout;
        }

        public static VerticalLayoutGroup Vertical(GameObject go, float spacing, RectOffset padding = null, TextAnchor alignment = TextAnchor.UpperCenter,
            bool expandWidth = true)
        {
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(0, 0, 0, 0);
            layout.childAlignment = alignment;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = expandWidth;
            layout.childForceExpandHeight = false;
            return layout;
        }

        public static HorizontalLayoutGroup Horizontal(GameObject go, float spacing, RectOffset padding = null, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(0, 0, 0, 0);
            layout.childAlignment = alignment;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            return layout;
        }

        public static ContentSizeFitter FitVertical(GameObject go)
        {
            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            return fitter;
        }

        /// <summary>Vertical scroll view; returns the content rect (children are laid out vertically).</summary>
        public static RectTransform ScrollView(Transform parent, string name, float spacing, RectOffset padding)
        {
            RectTransform root = CreateRect(name, parent);
            var rootImage = root.gameObject.AddComponent<Image>();
            rootImage.color = new Color(0f, 0f, 0f, 0f);
            var scroll = root.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 30f;

            RectTransform viewport = CreateRect("Viewport", root);
            Stretch(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();
            var viewportImage = viewport.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0f);

            RectTransform content = CreateRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = Vector2.zero;
            Vertical(content.gameObject, spacing, padding);
            FitVertical(content.gameObject);

            scroll.viewport = viewport;
            scroll.content = content;
            return content;
        }

        /// <summary>Row of star icons (filled / empty).</summary>
        public static Image[] Stars(Transform parent, string name, int filled, int total, float size)
        {
            RectTransform row = CreateRect(name, parent);
            Horizontal(row.gameObject, size * 0.12f);
            var images = new Image[total];
            for (int i = 0; i < total; i++)
            {
                images[i] = Image(row, "Star" + i, i < filled ? ArtId.Star : ArtId.StarEmpty, i < filled ? UITheme.Star : UITheme.StarEmpty);
                SetPreferredSize(images[i].gameObject, size, size);
            }

            SetPreferredSize(row.gameObject, total * size + (total - 1) * size * 0.12f, size);
            return images;
        }

        public static void SetStars(Image[] stars, int filled)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                bool on = i < filled;
                stars[i].sprite = PlaceholderArt.Get(on ? ArtId.Star : ArtId.StarEmpty);
                stars[i].color = on ? UITheme.Star : UITheme.StarEmpty;
            }
        }

        /// <summary>Menu background: vertical gradient covering the whole screen.</summary>
        public static Image Background(Transform canvas, Color bottom, Color top)
        {
            RectTransform rect = CreateRect("Background", canvas);
            Stretch(rect);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = PlaceholderArt.CreateGradient(bottom, top);
            image.raycastTarget = false;
            rect.SetAsFirstSibling();
            return image;
        }

        internal static void WarnMissingFont()
        {
            if (_fontWarningShown) return;
            _fontWarningShown = true;
            Debug.LogError("[TRYKLI] No TextMeshPro font found. Import the TMP Essential Resources " +
                           "(Window > TextMeshPro > Import TMP Essential Resources) or run Tools > TRYKLI > Setup Project.");
        }
    }

    /// <summary>Font resolution: optional project font (Resources/Fonts/TrykliFont) then TMP default font.</summary>
    public static class UIFonts
    {
        public const string ResourcePath = "Fonts/TrykliFont";

        private static TMP_FontAsset _font;
        private static bool _resolved;

        public static TMP_FontAsset Default
        {
            get
            {
                if (_resolved) return _font;
                _resolved = true;
                _font = Resources.Load<TMP_FontAsset>(ResourcePath);
                if (_font == null && TMP_Settings.instance != null) _font = TMP_Settings.defaultFontAsset;
                if (_font == null) UIFactory.WarnMissingFont();
                return _font;
            }
        }
    }
}
