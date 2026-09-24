using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Deterministic overlap tests used by mechanisms instead of trigger callbacks. Swept versions sample
    /// Trykli's movement during the last physics step so fast trajectories never tunnel through thin zones.
    /// </summary>
    public static class ElementGeometry
    {
        public static bool CircleOverlapsBox(Vector2 center, float radius, Vector2 boxCenter, Vector2 halfSize, float rotation)
        {
            Vector2 local = MathUtils.ToLocal(center, boxCenter, rotation);
            float dx = Mathf.Max(Mathf.Abs(local.x) - halfSize.x, 0f);
            float dy = Mathf.Max(Mathf.Abs(local.y) - halfSize.y, 0f);
            return dx * dx + dy * dy <= radius * radius;
        }

        public static bool SweptCircleOverlapsBox(Vector2 from, Vector2 to, float radius, Vector2 boxCenter, Vector2 halfSize, float rotation)
        {
            float distance = (to - from).magnitude;
            int samples = Mathf.Clamp(Mathf.CeilToInt(distance / Mathf.Max(0.05f, radius * 0.5f)), 1, 32);
            for (int i = 0; i <= samples; i++)
            {
                Vector2 point = Vector2.Lerp(from, to, i / (float)samples);
                if (CircleOverlapsBox(point, radius, boxCenter, halfSize, rotation)) return true;
            }

            return false;
        }

        public static bool SweptCircleOverlapsCircle(Vector2 from, Vector2 to, float radius, Vector2 center, float otherRadius)
        {
            Vector2 closest = MathUtils.ClosestPointOnSegment(center, from, to);
            float reach = radius + otherRadius;
            return (closest - center).sqrMagnitude <= reach * reach;
        }

        public static bool PointInBox(Vector2 point, Vector2 boxCenter, Vector2 halfSize, float rotation)
        {
            Vector2 local = MathUtils.ToLocal(point, boxCenter, rotation);
            return Mathf.Abs(local.x) <= halfSize.x && Mathf.Abs(local.y) <= halfSize.y;
        }

        public static float RotationOf(Transform transform) => transform.eulerAngles.z;
    }
}
