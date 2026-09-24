using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Level exit. Victory when the center of Trykli enters the zone (GDD 8). The test is done every physics
    /// step against the swept segment of Trykli's movement, so fast trajectories never skip the exit.
    /// </summary>
    public sealed class GoalZone : MonoBehaviour, ILevelElement, ISimulationElement
    {
        [SerializeField] private float radius = 0.6f;

        private LevelContext _context;
        private Transform _ring;
        private Transform _innerRing;

        public float Radius => radius;
        public Vector2 Center => transform.position;

        public static GoalZone Create(Transform parent, Vector2 position, float radius, WorldTheme theme)
        {
            var go = new GameObject("Goal");
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            var goal = go.AddComponent<GoalZone>();
            goal.radius = radius > 0f ? radius : 0.6f;
            goal.BuildVisual(theme);
            return goal;
        }

        private void BuildVisual(WorldTheme theme)
        {
            Transform root = VisualFactory.GetOrCreateVisualRoot(transform, out bool created);
            if (!created) return;
            float d = radius * 2f;
            Color glow = new Color(0.35f, 1f, 0.75f);
            VisualFactory.Sprite(root, "Glow", ArtId.SoftGlow, glow.WithAlpha(0.55f), SortingOrders.Goal, Vector2.zero, new Vector2(d * 2.2f, d * 2.2f));
            VisualFactory.Sprite(root, "Core", ArtId.Circle, new Color(0.12f, 0.2f, 0.28f, 0.85f), SortingOrders.Goal + 1, Vector2.zero, new Vector2(d, d));
            _ring = VisualFactory.Sprite(root, "Ring", ArtId.DashedRing, glow, SortingOrders.Goal + 2, Vector2.zero, new Vector2(d * 1.15f, d * 1.15f)).transform;
            _innerRing = VisualFactory.Sprite(root, "Inner", ArtId.Ring, glow.WithAlpha(0.8f), SortingOrders.Goal + 2, Vector2.zero, new Vector2(d * 0.6f, d * 0.6f)).transform;
        }

        public void Bind(LevelContext context)
        {
            _context = context;
            context.Goal = this;
        }

        public void OnSimulationStart() { }

        public void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = _context != null ? _context.Trykli : null;
            if (trykli == null || !trykli.IsAlive) return;
            Vector2 closest = MathUtils.ClosestPointOnSegment(Center, trykli.PreviousPosition, trykli.Position);
            if ((closest - Center).sqrMagnitude <= radius * radius) _context.NotifyGoalReached();
        }

        private void Update()
        {
            if (_ring != null) _ring.Rotate(0f, 0f, -60f * Time.deltaTime);
            if (_innerRing != null)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 4f) * 0.08f;
                float d = radius * 2f * 0.6f * pulse;
                _innerRing.localScale = new Vector3(d, d, 1f);
            }
        }
    }
}
