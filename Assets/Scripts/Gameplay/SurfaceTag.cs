using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>Identifies the surface category of static geometry (used by contact objectives).</summary>
    public sealed class SurfaceTag : MonoBehaviour
    {
        public Data.SurfaceKind kind = Data.SurfaceKind.Ground;
    }
}
