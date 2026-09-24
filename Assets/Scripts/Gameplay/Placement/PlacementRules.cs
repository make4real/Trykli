using System.Collections.Generic;
using Trykli.Data;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Placement
{
    /// <summary>
    /// Pure placement rules: where an item snaps inside a zone, capacity, spacing and rotation limits.
    /// Kept free of MonoBehaviours so it can be unit tested.
    /// </summary>
    public static class PlacementRules
    {
        public struct Settings
        {
            public float PointSnapDistance;
            public float AreaMargin;
            public float RailSnapDistance;
            public float GridStep;
            public float MinSpacing;

            public static Settings Default => new Settings
            {
                PointSnapDistance = 1.1f,
                AreaMargin = 0.45f,
                RailSnapDistance = 0.8f,
                GridStep = 0.25f,
                MinSpacing = 0.7f
            };
        }

        /// <summary>
        /// Resolves the position of <paramref name="itemId"/> dropped at <paramref name="point"/> in <paramref name="zone"/>.
        /// <paramref name="occupied"/> lists the positions of the other items already in that zone.
        /// </summary>
        public static bool TryResolve(PlacementZoneData zone, string itemId, Vector2 point, IReadOnlyList<Vector2> occupied,
            Settings settings, out Vector2 position, out float distance)
        {
            position = zone.position;
            distance = float.MaxValue;
            if (!zone.Accepts(itemId)) return false;
            int count = occupied != null ? occupied.Count : 0;
            if (count >= Mathf.Max(1, zone.capacity)) return false;

            switch (zone.shape)
            {
                case ZoneShape.Point:
                    distance = (point - zone.position).magnitude;
                    if (distance > settings.PointSnapDistance) return false;
                    position = zone.position;
                    break;

                case ZoneShape.Rect:
                {
                    Vector2 half = new Vector2(Mathf.Max(0.01f, zone.size.x * 0.5f), Mathf.Max(0.01f, zone.size.y * 0.5f));
                    Vector2 local = MathUtils.ToLocal(point, zone.position, zone.rotation);
                    float outsideX = Mathf.Max(0f, Mathf.Abs(local.x) - half.x);
                    float outsideY = Mathf.Max(0f, Mathf.Abs(local.y) - half.y);
                    distance = new Vector2(outsideX, outsideY).magnitude;
                    if (distance > settings.AreaMargin) return false;
                    local.x = Mathf.Clamp(MathUtils.Snap(local.x, settings.GridStep), -half.x, half.x);
                    local.y = Mathf.Clamp(MathUtils.Snap(local.y, settings.GridStep), -half.y, half.y);
                    position = MathUtils.ToWorld(local, zone.position, zone.rotation);
                    break;
                }

                case ZoneShape.Rail:
                {
                    float halfLength = Mathf.Max(0.01f, zone.size.x * 0.5f);
                    Vector2 axis = MathUtils.FromAngle(zone.rotation);
                    Vector2 a = zone.position - axis * halfLength;
                    Vector2 b = zone.position + axis * halfLength;
                    Vector2 closest = MathUtils.ClosestPointOnSegment(point, a, b);
                    distance = (point - closest).magnitude;
                    if (distance > settings.RailSnapDistance) return false;
                    float along = Vector2.Dot(closest - zone.position, axis);
                    along = Mathf.Clamp(MathUtils.Snap(along, settings.GridStep), -halfLength, halfLength);
                    position = zone.position + axis * along;
                    break;
                }
            }

            for (int i = 0; i < count; i++)
            {
                if ((occupied[i] - position).magnitude < settings.MinSpacing) return false;
            }

            return true;
        }

        /// <summary>Rotation applied to an item in a zone: snapped to the item step and clamped to the zone limits.</summary>
        public static float ClampRotation(PlacementZoneData zone, float rotation, float step, bool itemRotatable)
        {
            if (zone == null) return MathUtils.SnapAngle(rotation, step);
            if (!zone.allowRotation || !itemRotatable) return MathUtils.NormalizeAngle(zone.defaultRotation);
            float snapped = MathUtils.SnapAngle(rotation, step);
            float min = Mathf.Min(zone.minRotation, zone.maxRotation);
            float max = Mathf.Max(zone.minRotation, zone.maxRotation);
            if (max - min >= 359.9f) return snapped;
            return Mathf.Clamp(snapped, min, max);
        }

        /// <summary>Next rotation when the player presses a rotate button (or double taps).</summary>
        public static float Rotate(PlacementZoneData zone, float current, float step, int direction, bool itemRotatable)
        {
            float next = MathUtils.NormalizeAngle(current + step * Mathf.Sign(direction));
            if (zone != null && zone.allowRotation && itemRotatable)
            {
                float min = Mathf.Min(zone.minRotation, zone.maxRotation);
                float max = Mathf.Max(zone.minRotation, zone.maxRotation);
                if (max - min < 359.9f && (next > max + 0.01f || next < min - 0.01f)) return current;
            }

            return ClampRotation(zone, next, step, itemRotatable);
        }
    }
}
