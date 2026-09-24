using UnityEngine;

namespace Trykli.Utilities
{
    /// <summary>
    /// Tiny anti-aliased shape rasterizer based on signed distance functions.
    /// Used to build every placeholder sprite of the game at runtime (or baked to PNG by the editor tools),
    /// so the project never depends on external art to be playable.
    /// Coordinates are normalized: (0,0) bottom-left, (1,1) top-right.
    /// </summary>
    public sealed class IconPainter
    {
        private readonly int _size;
        private readonly Color[] _pixels;

        public IconPainter(int size)
        {
            _size = Mathf.Max(8, size);
            _pixels = new Color[_size * _size];
        }

        public int Size => _size;

        public void Circle(Vector2 center, float radius, Color color)
        {
            Paint(color, p => (p - center).magnitude - radius);
        }

        public void Ring(Vector2 center, float radius, float thickness, Color color)
        {
            float half = thickness * 0.5f;
            Paint(color, p => Mathf.Abs((p - center).magnitude - radius) - half);
        }

        public void Capsule(Vector2 a, Vector2 b, float radius, Color color)
        {
            Paint(color, p => (p - MathUtils.ClosestPointOnSegment(p, a, b)).magnitude - radius);
        }

        public void Box(Vector2 center, Vector2 halfSize, float cornerRadius, float rotationDegrees, Color color)
        {
            Paint(color, p =>
            {
                Vector2 local = MathUtils.ToLocal(p, center, rotationDegrees);
                Vector2 q = new Vector2(Mathf.Abs(local.x), Mathf.Abs(local.y)) - halfSize + new Vector2(cornerRadius, cornerRadius);
                Vector2 outside = new Vector2(Mathf.Max(q.x, 0f), Mathf.Max(q.y, 0f));
                return outside.magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - cornerRadius;
            });
        }

        public void BoxOutline(Vector2 center, Vector2 halfSize, float cornerRadius, float thickness, Color color)
        {
            float half = thickness * 0.5f;
            Paint(color, p =>
            {
                Vector2 local = p - center;
                Vector2 q = new Vector2(Mathf.Abs(local.x), Mathf.Abs(local.y)) - halfSize + new Vector2(cornerRadius, cornerRadius);
                Vector2 outside = new Vector2(Mathf.Max(q.x, 0f), Mathf.Max(q.y, 0f));
                float d = outside.magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - cornerRadius;
                return Mathf.Abs(d) - half;
            });
        }

        public void Polygon(Vector2[] points, Color color)
        {
            Paint(color, p => PolygonDistance(p, points));
        }

        /// <summary>Thick arc between two angles (degrees, counter-clockwise, 0 = right).</summary>
        public void Arc(Vector2 center, float radius, float thickness, float startDegrees, float endDegrees, Color color)
        {
            float half = thickness * 0.5f;
            Vector2 startPoint = center + MathUtils.FromAngle(startDegrees) * radius;
            Vector2 endPoint = center + MathUtils.FromAngle(endDegrees) * radius;
            float span = Mathf.Repeat(endDegrees - startDegrees, 360f);
            Paint(color, p =>
            {
                Vector2 d = p - center;
                float angle = Mathf.Repeat(Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg - startDegrees, 360f);
                if (angle <= span) return Mathf.Abs(d.magnitude - radius) - half;
                return Mathf.Min((p - startPoint).magnitude, (p - endPoint).magnitude) - half;
            });
        }

        /// <summary>Vertical gradient filling the whole texture.</summary>
        public void VerticalGradient(Color bottom, Color top)
        {
            for (int y = 0; y < _size; y++)
            {
                Color c = Color.Lerp(bottom, top, y / (float)(_size - 1));
                for (int x = 0; x < _size; x++) _pixels[y * _size + x] = c;
            }
        }

        /// <summary>Radial soft glow (alpha falls off towards the edge).</summary>
        public void SoftGlow(Color color)
        {
            Vector2 center = new Vector2(0.5f, 0.5f);
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    Vector2 p = new Vector2((x + 0.5f) / _size, (y + 0.5f) / _size);
                    float t = Mathf.Clamp01(1f - (p - center).magnitude * 2f);
                    Blend(x, y, color, t * t);
                }
            }
        }

        public Texture2D ToTexture(string name)
        {
            var texture = new Texture2D(_size, _size, TextureFormat.RGBA32, false)
            {
                name = name,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            texture.SetPixels(_pixels);
            texture.Apply(false, false);
            return texture;
        }

        private delegate float DistanceFunction(Vector2 p);

        private void Paint(Color color, DistanceFunction distance)
        {
            float pixel = 1f / _size;
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    Vector2 p = new Vector2((x + 0.5f) * pixel, (y + 0.5f) * pixel);
                    float d = distance(p);
                    float coverage = Mathf.Clamp01(0.5f - d / pixel);
                    if (coverage > 0f) Blend(x, y, color, coverage);
                }
            }
        }

        private void Blend(int x, int y, Color color, float coverage)
        {
            int index = y * _size + x;
            Color dst = _pixels[index];
            float srcA = color.a * coverage;
            float outA = srcA + dst.a * (1f - srcA);
            if (outA <= 0f)
            {
                _pixels[index] = Color.clear;
                return;
            }

            Color result = (color * srcA + dst * dst.a * (1f - srcA)) / outA;
            result.a = outA;
            _pixels[index] = result;
        }

        private static float PolygonDistance(Vector2 p, Vector2[] points)
        {
            float minDistance = float.MaxValue;
            bool inside = false;
            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                Vector2 a = points[j];
                Vector2 b = points[i];
                float d = (p - MathUtils.ClosestPointOnSegment(p, a, b)).magnitude;
                if (d < minDistance) minDistance = d;
                bool crosses = (b.y > p.y) != (a.y > p.y) &&
                               p.x < (a.x - b.x) * (p.y - b.y) / (a.y - b.y) + b.x;
                if (crosses) inside = !inside;
            }

            return inside ? -minDistance : minDistance;
        }
    }
}
