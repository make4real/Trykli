using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Utilities
{
    /// <summary>
    /// Central access point for every sprite used by the game.
    /// Returns the sprite from <see cref="SpriteLibrary"/> when available, otherwise generates a
    /// procedural placeholder. The game is therefore fully playable without any imported art.
    /// </summary>
    public static class PlaceholderArt
    {
        public readonly struct SpriteSpec
        {
            public readonly int Size;
            public readonly float PixelsPerUnit;
            public readonly Vector4 Border;

            public SpriteSpec(int size, float pixelsPerUnit, Vector4 border)
            {
                Size = size;
                PixelsPerUnit = pixelsPerUnit;
                Border = border;
            }
        }

        private static readonly Dictionary<ArtId, Sprite> Cache = new Dictionary<ArtId, Sprite>();
        private static SpriteLibrary _library;
        private static bool _libraryChecked;

        private static readonly Color White = Color.white;

        public static Sprite Get(ArtId id)
        {
            if (Cache.TryGetValue(id, out Sprite cached) && cached != null) return cached;

            if (!_libraryChecked)
            {
                _libraryChecked = true;
                _library = Resources.Load<SpriteLibrary>(SpriteLibrary.ResourcePath);
            }

            Sprite sprite = null;
            if (_library != null) _library.TryGet(id, out sprite);
            if (sprite == null) sprite = CreateSprite(id, BuildTexture(id));

            Cache[id] = sprite;
            return sprite;
        }

        /// <summary>Clears cached sprites (used by editor tools after baking art).</summary>
        public static void ClearCache()
        {
            Cache.Clear();
            _library = null;
            _libraryChecked = false;
        }

        public static Sprite CreateSprite(ArtId id, Texture2D texture)
        {
            SpriteSpec spec = GetSpec(id);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f),
                spec.PixelsPerUnit, 0, SpriteMeshType.FullRect, spec.Border);
            sprite.name = id.ToString();
            return sprite;
        }

        /// <summary>Creates a vertical gradient sprite (not cached, one per world theme).</summary>
        public static Sprite CreateGradient(Color bottom, Color top)
        {
            var painter = new IconPainter(64);
            painter.VerticalGradient(bottom, top);
            Texture2D texture = painter.ToTexture("Gradient");
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
        }

        public static SpriteSpec GetSpec(ArtId id)
        {
            switch (id)
            {
                case ArtId.Square: return new SpriteSpec(16, 16f, Vector4.zero);
                case ArtId.RoundedBox: return new SpriteSpec(64, 64f, new Vector4(16, 16, 16, 16));
                case ArtId.ZoneOutline: return new SpriteSpec(64, 64f, new Vector4(20, 20, 20, 20));
                case ArtId.UIPanel:
                case ArtId.UIPanelOutline: return new SpriteSpec(128, 100f, new Vector4(48, 48, 48, 48));
                case ArtId.SoftGlow:
                case ArtId.Spike:
                case ArtId.Crystal:
                case ArtId.Arrow: return new SpriteSpec(64, 64f, Vector4.zero);
                default: return new SpriteSpec(128, 128f, Vector4.zero);
            }
        }

        public static Texture2D BuildTexture(ArtId id)
        {
            SpriteSpec spec = GetSpec(id);
            var p = new IconPainter(spec.Size);
            Paint(id, p);
            return p.ToTexture(id.ToString());
        }

        private static Vector2 V(float x, float y) => new Vector2(x, y);

        private static void Paint(ArtId id, IconPainter p)
        {
            Vector2 c = V(0.5f, 0.5f);
            switch (id)
            {
                case ArtId.Square:
                    p.Box(c, V(0.5f, 0.5f), 0f, 0f, White);
                    break;
                case ArtId.Circle:
                    p.Circle(c, 0.48f, White);
                    break;
                case ArtId.Ring:
                    p.Ring(c, 0.43f, 0.08f, White);
                    break;
                case ArtId.DashedRing:
                    for (int i = 0; i < 12; i++) p.Arc(c, 0.44f, 0.05f, i * 30f, i * 30f + 18f, White);
                    break;
                case ArtId.SoftGlow:
                    p.SoftGlow(White);
                    break;
                case ArtId.RoundedBox:
                    p.Box(c, V(0.5f, 0.5f), 0.25f, 0f, White);
                    break;
                case ArtId.ZoneOutline:
                    p.Box(c, V(0.5f, 0.5f), 0.3f, 0f, White.WithAlpha(0.18f));
                    p.BoxOutline(c, V(0.47f, 0.47f), 0.28f, 0.06f, White);
                    break;
                case ArtId.UIPanel:
                    p.Box(c, V(0.5f, 0.5f), 0.34f, 0f, White);
                    break;
                case ArtId.UIPanelOutline:
                    p.BoxOutline(c, V(0.47f, 0.47f), 0.32f, 0.05f, White);
                    break;
                case ArtId.Spike:
                    p.Polygon(new[] { V(0.02f, 0.02f), V(0.98f, 0.02f), V(0.5f, 0.98f) }, White);
                    break;
                case ArtId.Crystal:
                    p.Polygon(new[] { V(0.5f, 0.98f), V(0.9f, 0.6f), V(0.5f, 0.02f), V(0.1f, 0.6f) }, White);
                    p.Polygon(new[] { V(0.5f, 0.9f), V(0.72f, 0.6f), V(0.5f, 0.6f) }, White.WithAlpha(0.6f));
                    break;
                case ArtId.Star:
                    p.Polygon(StarPoints(c, 0.48f, 0.21f), White);
                    break;
                case ArtId.StarEmpty:
                    p.Polygon(StarPoints(c, 0.48f, 0.21f), White.WithAlpha(0.25f));
                    break;
                case ArtId.Arrow:
                    p.Capsule(V(0.5f, 0.1f), V(0.5f, 0.6f), 0.09f, White);
                    p.Polygon(new[] { V(0.18f, 0.55f), V(0.82f, 0.55f), V(0.5f, 0.97f) }, White);
                    break;
                default:
                    PaintIcon(id, p, c);
                    break;
            }
        }

        private static void PaintIcon(ArtId id, IconPainter p, Vector2 c)
        {
            switch (id)
            {
                case ArtId.IconPause:
                    p.Capsule(V(0.36f, 0.27f), V(0.36f, 0.73f), 0.08f, White);
                    p.Capsule(V(0.64f, 0.27f), V(0.64f, 0.73f), 0.08f, White);
                    break;
                case ArtId.IconPlay:
                    p.Polygon(new[] { V(0.3f, 0.18f), V(0.84f, 0.5f), V(0.3f, 0.82f) }, White);
                    break;
                case ArtId.IconRestart:
                    CircularArrow(p, c, clockwise: true);
                    break;
                case ArtId.IconReset:
                    CircularArrow(p, c, clockwise: false);
                    p.Circle(c, 0.07f, White);
                    break;
                case ArtId.IconSettings:
                    for (int i = 0; i < 8; i++)
                    {
                        float a = i * 45f;
                        p.Box(c + MathUtils.FromAngle(a) * 0.3f, V(0.07f, 0.06f), 0.02f, a, White);
                    }
                    p.Ring(c, 0.2f, 0.13f, White);
                    break;
                case ArtId.IconBack:
                    p.Capsule(V(0.62f, 0.22f), V(0.34f, 0.5f), 0.08f, White);
                    p.Capsule(V(0.34f, 0.5f), V(0.62f, 0.78f), 0.08f, White);
                    break;
                case ArtId.IconLock:
                    p.Box(V(0.5f, 0.36f), V(0.26f, 0.21f), 0.06f, 0f, White);
                    p.Arc(V(0.5f, 0.56f), 0.16f, 0.08f, 0f, 180f, White);
                    break;
                case ArtId.IconCheck:
                    p.Capsule(V(0.2f, 0.52f), V(0.42f, 0.28f), 0.08f, White);
                    p.Capsule(V(0.42f, 0.28f), V(0.82f, 0.74f), 0.08f, White);
                    break;
                case ArtId.IconCross:
                    p.Capsule(V(0.26f, 0.26f), V(0.74f, 0.74f), 0.08f, White);
                    p.Capsule(V(0.26f, 0.74f), V(0.74f, 0.26f), 0.08f, White);
                    break;
                case ArtId.IconHint:
                    p.Circle(V(0.5f, 0.58f), 0.25f, White);
                    p.Box(V(0.5f, 0.27f), V(0.12f, 0.08f), 0.03f, 0f, White);
                    p.Capsule(V(0.42f, 0.14f), V(0.58f, 0.14f), 0.035f, White);
                    break;
                case ArtId.IconRotateLeft:
                    RotateArrow(p, c, left: true);
                    break;
                case ArtId.IconRotateRight:
                    RotateArrow(p, c, left: false);
                    break;
                case ArtId.IconTrash:
                    p.Box(V(0.5f, 0.4f), V(0.21f, 0.26f), 0.05f, 0f, White);
                    p.Capsule(V(0.24f, 0.73f), V(0.76f, 0.73f), 0.04f, White);
                    p.Box(V(0.5f, 0.8f), V(0.09f, 0.04f), 0.02f, 0f, White);
                    break;
                case ArtId.IconSpeed:
                    p.Polygon(new[] { V(0.16f, 0.24f), V(0.5f, 0.5f), V(0.16f, 0.76f) }, White);
                    p.Polygon(new[] { V(0.5f, 0.24f), V(0.84f, 0.5f), V(0.5f, 0.76f) }, White);
                    break;
                case ArtId.IconHome:
                    p.Polygon(new[] { V(0.16f, 0.5f), V(0.5f, 0.84f), V(0.84f, 0.5f) }, White);
                    p.Box(V(0.5f, 0.34f), V(0.22f, 0.18f), 0.03f, 0f, White);
                    break;
                case ArtId.IconLevels:
                    p.Box(V(0.32f, 0.32f), V(0.13f, 0.13f), 0.04f, 0f, White);
                    p.Box(V(0.68f, 0.32f), V(0.13f, 0.13f), 0.04f, 0f, White);
                    p.Box(V(0.32f, 0.68f), V(0.13f, 0.13f), 0.04f, 0f, White);
                    p.Box(V(0.68f, 0.68f), V(0.13f, 0.13f), 0.04f, 0f, White);
                    break;
                case ArtId.IconMusic:
                    p.Circle(V(0.36f, 0.28f), 0.13f, White);
                    p.Capsule(V(0.47f, 0.3f), V(0.47f, 0.8f), 0.04f, White);
                    p.Capsule(V(0.47f, 0.8f), V(0.74f, 0.7f), 0.05f, White);
                    break;
                case ArtId.IconSound:
                    p.Box(V(0.26f, 0.5f), V(0.09f, 0.12f), 0.02f, 0f, White);
                    p.Polygon(new[] { V(0.3f, 0.38f), V(0.54f, 0.18f), V(0.54f, 0.82f), V(0.3f, 0.62f) }, White);
                    p.Arc(V(0.56f, 0.5f), 0.14f, 0.05f, -50f, 50f, White);
                    p.Arc(V(0.56f, 0.5f), 0.27f, 0.05f, -50f, 50f, White);
                    break;
                case ArtId.IconVibration:
                    p.Box(c, V(0.15f, 0.27f), 0.05f, 0f, White);
                    p.Capsule(V(0.2f, 0.36f), V(0.2f, 0.64f), 0.03f, White);
                    p.Capsule(V(0.8f, 0.36f), V(0.8f, 0.64f), 0.03f, White);
                    break;
                case ArtId.IconLanguage:
                    p.Ring(c, 0.32f, 0.06f, White);
                    p.Capsule(V(0.2f, 0.5f), V(0.8f, 0.5f), 0.03f, White);
                    p.Arc(V(0.5f, 0.5f), 0.32f, 0.05f, 60f, 120f, White);
                    p.Box(c, V(0.03f, 0.3f), 0.02f, 0f, White);
                    p.Ring(c, 0.16f, 0.05f, White);
                    break;
                case ArtId.IconSkins:
                    p.Circle(c, 0.38f, White);
                    p.Circle(V(0.39f, 0.56f), 0.08f, Color.black.WithAlpha(0.6f));
                    p.Circle(V(0.61f, 0.56f), 0.08f, Color.black.WithAlpha(0.6f));
                    break;
                case ArtId.IconFinger:
                    p.Circle(V(0.5f, 0.7f), 0.14f, White);
                    p.Capsule(V(0.5f, 0.66f), V(0.5f, 0.3f), 0.14f, White);
                    p.Box(V(0.5f, 0.22f), V(0.2f, 0.14f), 0.08f, 0f, White);
                    break;
                default:
                    PaintItem(id, p, c);
                    break;
            }
        }

        private static void PaintItem(ArtId id, IconPainter p, Vector2 c)
        {
            switch (id)
            {
                case ArtId.ItemSpring:
                {
                    Color metal = ColorUtils.Hex("#5A6270");
                    Color orange = ColorUtils.Hex("#FFB020");
                    p.Box(V(0.5f, 0.17f), V(0.34f, 0.07f), 0.03f, 0f, metal);
                    float[] xs = { 0.3f, 0.7f, 0.3f, 0.7f, 0.3f, 0.7f };
                    for (int i = 0; i < xs.Length - 1; i++)
                    {
                        float y0 = 0.25f + i * 0.1f;
                        p.Capsule(V(xs[i], y0), V(xs[i + 1], y0 + 0.1f), 0.04f, metal);
                    }
                    p.Box(V(0.5f, 0.8f), V(0.36f, 0.08f), 0.04f, 0f, orange);
                    break;
                }
                case ArtId.ItemRamp:
                    p.Box(c, V(0.44f, 0.09f), 0.05f, 28f, ColorUtils.Hex("#D9A066"));
                    p.Circle(V(0.5f, 0.5f), 0.05f, ColorUtils.Hex("#8C5A2B"));
                    break;
                case ArtId.ItemFan:
                {
                    Color blue = ColorUtils.Hex("#4FC3F7");
                    p.Ring(c, 0.4f, 0.07f, blue.Darken(0.3f));
                    for (int i = 0; i < 3; i++)
                    {
                        float a = i * 120f + 20f;
                        p.Box(c + MathUtils.FromAngle(a) * 0.17f, V(0.15f, 0.07f), 0.06f, a, blue);
                    }
                    p.Circle(c, 0.08f, blue.Darken(0.4f));
                    break;
                }
                case ArtId.ItemPortal:
                    p.Ring(c, 0.36f, 0.13f, ColorUtils.Hex("#9B59FF"));
                    p.Ring(c, 0.18f, 0.05f, ColorUtils.Hex("#D7B8FF"));
                    break;
                case ArtId.ItemMagnet:
                {
                    Color red = ColorUtils.Hex("#E74C3C");
                    Color tip = ColorUtils.Hex("#D5DBE1");
                    p.Arc(V(0.5f, 0.48f), 0.22f, 0.15f, 180f, 360f, red);
                    p.Box(V(0.28f, 0.62f), V(0.075f, 0.15f), 0f, 0f, red);
                    p.Box(V(0.72f, 0.62f), V(0.075f, 0.15f), 0f, 0f, red);
                    p.Box(V(0.28f, 0.81f), V(0.075f, 0.06f), 0.01f, 0f, tip);
                    p.Box(V(0.72f, 0.81f), V(0.075f, 0.06f), 0.01f, 0f, tip);
                    break;
                }
                case ArtId.ItemBomb:
                    p.Circle(V(0.46f, 0.42f), 0.31f, ColorUtils.Hex("#2C3E50"));
                    p.Circle(V(0.36f, 0.52f), 0.07f, White.WithAlpha(0.35f));
                    p.Capsule(V(0.64f, 0.66f), V(0.74f, 0.8f), 0.045f, ColorUtils.Hex("#8C5A2B"));
                    p.Circle(V(0.77f, 0.85f), 0.07f, ColorUtils.Hex("#FF9F1C"));
                    break;
                case ArtId.ItemBumper:
                    p.Circle(c, 0.42f, ColorUtils.Hex("#FF4FA3"));
                    p.Ring(c, 0.34f, 0.05f, White);
                    p.Circle(c, 0.16f, ColorUtils.Hex("#FF9CCF"));
                    break;
                case ArtId.ItemPlatform:
                    p.Box(c, V(0.44f, 0.12f), 0.08f, 0f, ColorUtils.Hex("#6C7A89"));
                    break;
                case ArtId.ItemGravity:
                    p.Circle(c, 0.44f, ColorUtils.Hex("#2ECC71"));
                    p.Capsule(V(0.5f, 0.3f), V(0.5f, 0.7f), 0.045f, White);
                    p.Polygon(new[] { V(0.34f, 0.66f), V(0.66f, 0.66f), V(0.5f, 0.86f) }, White);
                    p.Polygon(new[] { V(0.34f, 0.34f), V(0.66f, 0.34f), V(0.5f, 0.14f) }, White);
                    break;
                default:
                    p.Circle(c, 0.4f, Color.magenta);
                    break;
            }
        }

        private static void CircularArrow(IconPainter p, Vector2 c, bool clockwise)
        {
            const float radius = 0.27f;
            // Arc drawn counter-clockwise from 120 deg to 60 deg, leaving a gap at the top.
            p.Arc(c, radius, 0.1f, 120f, 60f, White);
            float headAngle = clockwise ? 120f : 60f;
            Vector2 anchor = c + MathUtils.FromAngle(headAngle) * radius;
            Vector2 tangent = MathUtils.FromAngle(headAngle + (clockwise ? -90f : 90f));
            Vector2 radial = MathUtils.FromAngle(headAngle);
            p.Polygon(new[]
            {
                anchor + radial * 0.13f,
                anchor - radial * 0.13f,
                anchor + tangent * 0.16f
            }, White);
        }

        private static void RotateArrow(IconPainter p, Vector2 c, bool left)
        {
            Vector2 center = c + V(0f, -0.06f);
            const float radius = 0.27f;
            p.Arc(center, radius, 0.09f, 25f, 155f, White);
            float headAngle = left ? 155f : 25f;
            Vector2 anchor = center + MathUtils.FromAngle(headAngle) * radius;
            Vector2 tangent = MathUtils.FromAngle(headAngle + (left ? 90f : -90f));
            Vector2 radial = MathUtils.FromAngle(headAngle);
            p.Polygon(new[]
            {
                anchor + radial * 0.13f,
                anchor - radial * 0.13f,
                anchor + tangent * 0.16f
            }, White);
        }

        private static Vector2[] StarPoints(Vector2 center, float outer, float inner)
        {
            var points = new Vector2[10];
            for (int i = 0; i < 10; i++)
            {
                float radius = i % 2 == 0 ? outer : inner;
                points[i] = center + MathUtils.FromAngle(90f + i * 36f) * radius;
            }

            return points;
        }
    }
}
