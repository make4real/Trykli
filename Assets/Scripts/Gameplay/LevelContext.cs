using System;
using System.Collections.Generic;
using Trykli.Data;
using Trykli.Mechanics;
using Trykli.Placement;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Runtime state shared by every element of the loaded level: services (switches, gravity, run statistics),
    /// references (Trykli, goal, zones, crystals, portals) and the simulation clock.
    /// Lives on the level root; a new context is created for every (re)build of the level.
    /// </summary>
    public sealed class LevelContext : MonoBehaviour
    {
        private readonly List<ISimulationElement> _simulationElements = new List<ISimulationElement>();
        private readonly HashSet<ILevelElement> _boundElements = new HashSet<ILevelElement>();

        public LevelData Level { get; private set; }
        public WorldTheme Theme { get; private set; }
        public Rect Bounds { get; private set; }
        public Vector2 SpawnPosition { get; set; }

        public SwitchBus Switches { get; } = new SwitchBus();
        public GravityService Gravity { get; } = new GravityService();
        public RunTracker Tracker { get; } = new RunTracker();

        public TrykliController Trykli { get; set; }
        public GoalZone Goal { get; set; }
        public Transform ElementsRoot { get; set; }
        public Transform PlacedRoot { get; set; }

        public List<PlacementZone> Zones { get; } = new List<PlacementZone>();
        public List<Crystal> Crystals { get; } = new List<Crystal>();
        public List<Portal> Portals { get; } = new List<Portal>();

        /// <summary>Portals ignore Trykli until this simulation time (prevents chained instant teleports).</summary>
        public float NextTeleportTime { get; set; }

        public float SimulationTime { get; private set; }
        public bool IsSimulating { get; private set; }

        public event Action GoalReached;
        public event Action<FailureReason> FailureRequested;

        public void Initialize(LevelData level, WorldTheme theme, Rect bounds)
        {
            Level = level;
            Theme = theme;
            Bounds = bounds;
        }

        /// <summary>Registers zones and crystals once the level is built (needed before the simulation).</summary>
        public void CollectStaticElements()
        {
            Zones.Clear();
            Zones.AddRange(GetComponentsInChildren<PlacementZone>(true));
            Zones.Sort((a, b) => a.Index.CompareTo(b.Index));
            Crystals.Clear();
            Crystals.AddRange(GetComponentsInChildren<Crystal>(true));
        }

        /// <summary>Binds every element currently in the level and starts the simulation clock.</summary>
        public void StartSimulation()
        {
            _simulationElements.Clear();
            Portals.Clear();
            foreach (MonoBehaviour behaviour in GetComponentsInChildren<MonoBehaviour>(false))
            {
                if (behaviour is ILevelElement element && _boundElements.Add(element)) element.Bind(this);
                if (behaviour is ISimulationElement simulationElement) _simulationElements.Add(simulationElement);
                if (behaviour is Portal portal) Portals.Add(portal);
            }

            CollectStaticElements();
            SimulationTime = 0f;
            NextTeleportTime = 0f;
            IsSimulating = true;
            foreach (ISimulationElement element in _simulationElements) element.OnSimulationStart();
        }

        public void StepElements(float deltaTime)
        {
            if (!IsSimulating) return;
            for (int i = 0; i < _simulationElements.Count; i++)
            {
                if (!IsSimulating) break;
                ISimulationElement element = _simulationElements[i];
                if (element is MonoBehaviour behaviour && behaviour == null) continue;
                element.OnSimulationStep(deltaTime, SimulationTime);
            }

            SimulationTime += deltaTime;
        }

        public void StopSimulation()
        {
            IsSimulating = false;
        }

        public void RequestFailure(FailureReason reason)
        {
            if (!IsSimulating) return;
            FailureRequested?.Invoke(reason);
        }

        public void NotifyGoalReached()
        {
            if (!IsSimulating) return;
            GoalReached?.Invoke();
        }

        /// <summary>Other endpoint of a portal pair (null when the pair is incomplete).</summary>
        public Portal FindPartner(Portal portal)
        {
            foreach (Portal other in Portals)
            {
                if (other != null && other != portal && other.Channel == portal.Channel && other.isActiveAndEnabled) return other;
            }

            return null;
        }

        public Rect KillBounds(float margin)
        {
            return new Rect(Bounds.xMin - margin, Bounds.yMin - margin, Bounds.width + margin * 2f, Bounds.height + margin * 2f);
        }
    }
}
