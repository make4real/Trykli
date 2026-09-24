using UnityEngine;

namespace Trykli.Utilities
{
    public static class ComponentExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            return component != null ? component : go.AddComponent<T>();
        }

        public static GameObject CreateChild(this Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform) child.gameObject.SetLayerRecursively(layer);
        }
    }
}
