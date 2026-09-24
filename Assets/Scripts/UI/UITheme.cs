using Trykli.Utilities;
using UnityEngine;

namespace Trykli.UI
{
    /// <summary>Temporary but consistent visual identity of the interface (easy to replace).</summary>
    public static class UITheme
    {
        public static readonly Vector2 ReferenceResolution = new Vector2(1080f, 1920f);

        public static readonly Color MenuTop = ColorUtils.Hex("#3D5A98");
        public static readonly Color MenuBottom = ColorUtils.Hex("#1E2A4A");
        public static readonly Color Primary = ColorUtils.Hex("#FF9F1C");
        public static readonly Color Secondary = ColorUtils.Hex("#2EC4B6");
        public static readonly Color Neutral = ColorUtils.Hex("#5C6B8A");
        public static readonly Color Danger = ColorUtils.Hex("#E63946");
        public static readonly Color Success = ColorUtils.Hex("#2ECC71");
        public static readonly Color Panel = ColorUtils.Hex("#FFFFFF");
        public static readonly Color PanelDark = ColorUtils.Hex("#1B2238").WithAlpha(0.88f);
        public static readonly Color TextDark = ColorUtils.Hex("#1B2238");
        public static readonly Color TextLight = Color.white;
        public static readonly Color TextMuted = ColorUtils.Hex("#8A94A8");
        public static readonly Color Star = ColorUtils.Hex("#FFC93C");
        public static readonly Color StarEmpty = ColorUtils.Hex("#C9CED8");
        public static readonly Color Dim = new Color(0.05f, 0.07f, 0.12f, 0.6f);
        public static readonly Color Crystal = ColorUtils.Hex("#66E3FF");

        public const float TitleSize = 120f;
        public const float HeadingSize = 68f;
        public const float SubheadingSize = 52f;
        public const float BodySize = 44f;
        public const float SmallSize = 34f;

        /// <summary>Minimum touch target (GDD 92: 80-100 px on the reference resolution).</summary>
        public const float MinTouchSize = 120f;
        public const float ButtonHeight = 150f;
    }
}
