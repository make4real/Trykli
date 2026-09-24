using UnityEngine;

namespace Trykli.Utilities
{
    public static class ColorUtils
    {
        /// <summary>Parses "#RRGGBB" or "#RRGGBBAA". Returns <paramref name="fallback"/> when invalid.</summary>
        public static Color Hex(string hex, Color fallback)
        {
            if (!string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out Color color)) return color;
            return fallback;
        }

        public static Color Hex(string hex)
        {
            return Hex(hex, Color.magenta);
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color Darken(this Color color, float amount)
        {
            return Color.Lerp(color, Color.black, amount).WithAlpha(color.a);
        }

        public static Color Lighten(this Color color, float amount)
        {
            return Color.Lerp(color, Color.white, amount).WithAlpha(color.a);
        }
    }
}
