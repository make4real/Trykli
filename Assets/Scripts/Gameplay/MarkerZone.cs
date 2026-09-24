using Trykli.Data;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Invisible (dashed in the editor) zone recording "zone:&lt;id&gt;" when Trykli passes through it.
    /// Used by objectives such as "passer par la plateforme gauche" or "prendre le chemin haut".
    /// </summary>
    public sealed class MarkerZone : MonoBehaviour, IConfigurableElement, ILevelElement, ISimulationElement
    {
        [SerializeField] private string markerId = "";
        [SerializeField] private Vector2 size = new Vector2(1f, 1f);

        private LevelContext _context;
        private bool _reached;

        public string MarkerId => markerId;

        public void Configure(ElementData data, WorldTheme theme)
        {
            markerId = data.id ?? string.Empty;
            if (data.size.x > 0f && data.size.y > 0f) size = data.size;
            Transform root = VisualFactory.GetOrCreateVisualRoot(transform, out bool created);
            if (created) VisualFactory.Sliced(root, "Area", ArtId.ZoneOutline, theme.Accent.WithAlpha(0.18f), SortingOrders.Zones, size);
            VisualFactory.SetSlicedSize(root.Find("Area")?.GetComponent<SpriteRenderer>(), size);
        }

        public void Bind(LevelContext context)
        {
            _context = context;
        }

        public void OnSimulationStart()
        {
            _reached = false;
        }

        public void OnSimulationStep(float deltaTime, float simulationTime)
        {
            if (_reached || _context == null) return;
            TrykliController trykli = _context.Trykli;
            if (trykli == null || !trykli.IsAlive) return;
            Vector2 local = MathUtils.ToLocal(trykli.Position, transform.position, transform.eulerAngles.z);
            if (Mathf.Abs(local.x) > size.x * 0.5f || Mathf.Abs(local.y) > size.y * 0.5f) return;
            _reached = true;
            _context.Tracker.RecordCounter(Objectives.LevelRunStats.Counters.Zone(markerId));
        }
    }
}
