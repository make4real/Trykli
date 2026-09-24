using Trykli.Data;
using Trykli.Placement;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>Spawn position of Trykli in a prefab based level.</summary>
    public sealed class LevelSpawnPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.6f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, 0.35f);
        }
    }
}
