using Trykli.Data;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Root of a level opened in a scene by the Level Editor ("Edit layout in scene"). Designers move the
    /// elements, zones, spawn and goal with the regular Unity tools, then apply the scene back to the LevelData.
    /// Never used at runtime.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class LevelAuthoringRoot : MonoBehaviour
    {
        public LevelData level;
        public Transform spawnMarker;
        public Transform goalMarker;

        private void OnDrawGizmos()
        {
            if (level == null || level.layout == null) return;
            Rect bounds = level.layout.bounds;
            Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.9f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            if (goalMarker != null)
            {
                Gizmos.color = new Color(0.3f, 1f, 0.6f);
                Gizmos.DrawWireSphere(goalMarker.position, level.layout.goalRadius);
            }
        }
    }
}
