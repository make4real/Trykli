using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>Helpers creating sprite based visuals for gameplay elements.</summary>
    public static class VisualFactory
    {
        public const string VisualRootName = "Visual";

        /// <summary>Returns the "Visual" child, creating it when missing. <paramref name="created"/> tells whether it is new.</summary>
        public static Transform GetOrCreateVisualRoot(Transform owner, out bool created)
        {
            Transform existing = owner.Find(VisualRootName);
            created = existing == null;
            if (existing != null) return existing;
            var go = new GameObject(VisualRootName);
            go.transform.SetParent(owner, false);
            return go.transform;
        }

        public static SpriteRenderer Sprite(Transform parent, string name, ArtId art, Color color, int order,
            Vector2 localPosition = default, Vector2? scale = null, float rotation = 0f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            Vector2 s = scale ?? Vector2.one;
            go.transform.localScale = new Vector3(s.x, s.y, 1f);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderArt.Get(art);
            renderer.color = color;
            renderer.sortingOrder = order;
            return renderer;
        }

        /// <summary>9-sliced sprite sized in world units (blocks, platforms, zones).</summary>
        public static SpriteRenderer Sliced(Transform parent, string name, ArtId art, Color color, int order, Vector2 size, Vector2 localPosition = default)
        {
            SpriteRenderer renderer = Sprite(parent, name, art, color, order, localPosition);
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = size;
            return renderer;
        }

        public static void SetSlicedSize(SpriteRenderer renderer, Vector2 size)
        {
            if (renderer == null) return;
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = size;
        }
    }

    /// <summary>Shared physics materials created once at runtime.</summary>
    public static class PhysicsMaterials
    {
        private static PhysicsMaterial2D _default;
        private static PhysicsMaterial2D _frictionless;
        private static PhysicsMaterial2D _trykli;

        public static PhysicsMaterial2D Default => _default != null ? _default : (_default = Create("TrykliDefault", 0.5f, 0.05f));
        public static PhysicsMaterial2D Frictionless => _frictionless != null ? _frictionless : (_frictionless = Create("TrykliSlippery", 0f, 0f));

        public static PhysicsMaterial2D ForTrykli(float friction, float bounciness)
        {
            if (_trykli == null) _trykli = Create("TrykliBody", friction, bounciness);
            _trykli.friction = friction;
            _trykli.bounciness = bounciness;
            return _trykli;
        }

        private static PhysicsMaterial2D Create(string name, float friction, float bounciness)
        {
            return new PhysicsMaterial2D(name) { friction = friction, bounciness = bounciness };
        }
    }
}
