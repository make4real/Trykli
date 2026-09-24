using Trykli.Data;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Holds the layout data of an element while a level is edited in a scene. Position and rotation come from
    /// the transform; every other parameter is edited here and applied with "Rebuild" in the Level Editor.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class ElementAuthoring : MonoBehaviour
    {
        public ElementData data = new ElementData();

        /// <summary>Layout data including the current transform.</summary>
        public ElementData Capture()
        {
            ElementData copy = data.Clone();
            copy.position = RoundVector(transform.position);
            copy.rotation = Mathf.Round(Utilities.MathUtils.NormalizeAngle(transform.eulerAngles.z) * 10f) / 10f;
            return copy;
        }

        public static Vector2 RoundVector(Vector3 value)
        {
            return new Vector2(Mathf.Round(value.x * 100f) / 100f, Mathf.Round(value.y * 100f) / 100f);
        }
    }
}
