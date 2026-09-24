using UnityEngine;

namespace Trykli.Utilities
{
    public static class MathUtils
    {
        /// <summary>Unit vector pointing along the given angle (degrees, 0 = right, counter-clockwise).</summary>
        public static Vector2 FromAngle(float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        /// <summary>Returns the "up" direction of an object rotated by <paramref name="degrees"/> around Z.</summary>
        public static Vector2 UpFromRotation(float degrees)
        {
            return FromAngle(degrees + 90f);
        }

        public static float NormalizeAngle(float degrees)
        {
            degrees %= 360f;
            if (degrees > 180f) degrees -= 360f;
            if (degrees <= -180f) degrees += 360f;
            return degrees;
        }

        public static float SnapAngle(float degrees, float step)
        {
            if (step <= 0f) return NormalizeAngle(degrees);
            return NormalizeAngle(Mathf.Round(degrees / step) * step);
        }

        public static float Snap(float value, float step)
        {
            if (step <= 0f) return value;
            return Mathf.Round(value / step) * step;
        }

        /// <summary>Closest point to <paramref name="p"/> on segment [a, b].</summary>
        public static Vector2 ClosestPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float lengthSq = ab.sqrMagnitude;
            if (lengthSq < 1e-6f) return a;
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / lengthSq);
            return a + ab * t;
        }

        /// <summary>Converts a world point into the local frame of a box rotated by <paramref name="rotation"/> degrees.</summary>
        public static Vector2 ToLocal(Vector2 point, Vector2 origin, float rotation)
        {
            Vector2 d = point - origin;
            float rad = -rotation * Mathf.Deg2Rad;
            float c = Mathf.Cos(rad);
            float s = Mathf.Sin(rad);
            return new Vector2(d.x * c - d.y * s, d.x * s + d.y * c);
        }

        public static Vector2 ToWorld(Vector2 local, Vector2 origin, float rotation)
        {
            float rad = rotation * Mathf.Deg2Rad;
            float c = Mathf.Cos(rad);
            float s = Mathf.Sin(rad);
            return origin + new Vector2(local.x * c - local.y * s, local.x * s + local.y * c);
        }

        public static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            t -= 1f;
            return 1f + c3 * t * t * t + c1 * t * t;
        }

        public static float EaseOutCubic(float t)
        {
            t = 1f - Mathf.Clamp01(t);
            return 1f - t * t * t;
        }
    }
}
