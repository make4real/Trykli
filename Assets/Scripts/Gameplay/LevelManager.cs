using System;
using System.Collections;
using System.Collections.Generic;
using Trykli.Audio;
using Trykli.Core;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Objectives;
using Trykli.Placement;
using Trykli.Save;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>Per-level session counters (reset when another level is loaded).</summary>
    public sealed class LevelSession
    {
        public LevelSession(int levelId)
        {
            LevelId = levelId;
        }

        public int LevelId { get; }
        public int Attempts { get; set; }
        public int Failures { get; set; }
        public bool Completed { get; set; }
    }

    /// <summary>
    /// Orchestrates a level (GDD 47): loads LevelData, builds the level, spawns Trykli, prepares the placement
    /// zones, starts the simulation (GO), detects victory / failure, Restart (same placement), Modify (back to
    /// placement), Reset (everything back to the inventory), computes stars, saves progression, loads the next level.
    /// </summary>
    public sealed class LevelManager : MonoBehaviour
    {
        private const float VictoryPanelDelay = 0.65f;
        private const float FailurePanelDelay = 0.45f;

        private readonly List<PlacementRecord> _snapshot = new List<PlacementRecord>();
        private Camera _camera;
        private Coroutine _pendingPanel;

        public static LevelManager Current { get; private set; }

        public LevelData Level { get; private set; }
        public WorldData World { get; private set; }
        public WorldTheme Theme { get; private set; } = WorldTheme.Default;
        public LevelContext Context { get; private set; }
        public PlacementManager Placement { get; private set; }
        public SimulationRunner Runner { get; private set; }
        public CameraController CameraRig { get; private set; }
        public HintController Hints { get; private set; }
        public LevelSession Session { get; private set; }

        public FailureReason LastFailure { get; private set; }
        public LevelResult LastResult { get; private set; }
        public ProgressionService.RecordOutcome LastOutcome { get; private set; }
        public LevelRunStats LastStats { get; private set; }

        public GameState State => GameManager.Instance.State.Current;
        public bool HasSnapshot => _snapshot.Count > 0;

        public event Action<LevelData> LevelLoaded;
        public event Action<GameState> StateChanged;
        public event Action SimulationStarted;
        public event Action VictoryReady;
        public event Action FailureReady;
        public event Action LevelRebuilt;

        public void Initialize(CameraController cameraRig)
        {
            Current = this;
            CameraRig = cameraRig;
            _camera = cameraRig.Camera;
            Placement = gameObject.AddComponent<PlacementManager>();
            Runner = gameObject.AddComponent<SimulationRunner>();
            Hints = gameObject.AddComponent<HintController>();
            GameplayConfig config = GameplayConfig.Instance;
            Runner.Configure(config.physicsStep, config.maxStepsPerFrame);
            Runner.StepCompleted += OnStepCompleted;
            Runner.SpeedMultiplier = GameManager.Instance.Settings.fastSimulation ? 2f : 1f;
        }

        public void LoadLevel(int levelId)
        {
            GameManager game = GameManager.Instance;
            LevelData level = game.Database.GetLevel(levelId);
            if (level == null)
            {
                Debug.LogError($"[TRYKLI] Level {levelId} not found.");
                return;
            }

            StopPendingPanel();
            Level = level;
            World = game.Database.GetWorld(level.worldId);
            Theme = LevelBuilder.ThemeOf(World);
            if (_camera != null) _camera.backgroundColor = Theme.BackgroundBottom;
            Session = new LevelSession(levelId) { Completed = game.Progression.IsLevelCompleted(levelId) };
            _snapshot.Clear();
            Hints.ResetFor(level);
            game.SetPendingLevel(levelId);

            Rebuild(null);
            SetState(GameState.Placement);
            Placement.SetInputEnabled(true);
            game.Audio.PlayMusic(MusicTrack.Gameplay);
            GameEvents.RaiseLevelStarted(levelId);
            LevelLoaded?.Invoke(level);
        }

        private void Rebuild(List<PlacementRecord> placements)
        {
            Runner.Stop();
            if (Context != null)
            {
                Context.gameObject.SetActive(false);
                Destroy(Context.gameObject);
            }

            GameManager game = GameManager.Instance;
            Context = LevelBuilder.Build(Level, Theme, transform, game.Database.Items);
            Context.GoalReached += OnGoalReached;
            Context.FailureRequested += OnFailureRequested;

            SkinDefinition skin = SkinCatalog.Get(game.SaveSystem.Data.selectedSkin);
            TrykliController trykli = TrykliController.Create(Context.transform, Context.SpawnPosition, game.Config, skin);
            Context.Trykli = trykli;

            Placement.Setup(Context, Level.availableItems, _camera);
            if (placements != null) Placement.Restore(placements);

            CameraRig.Configure(Level.layout.bounds, Level.cameraSettings, trykli.transform);
            Hints.Apply(Context);
            LevelRebuilt?.Invoke();
        }

        // ------------------------------------------------------------------ Player actions

        /// <summary>GO: locks the placement and starts the simulation.</summary>
        public void Go()
        {
            if (State != GameState.Placement || Placement.IsDragging) return;
            _snapshot.Clear();
            _snapshot.AddRange(Placement.CaptureRecords());
            StartSimulation();
        }

        /// <summary>RÉESSAYER / restart: same placement, simulation restarted immediately.</summary>
        public void Restart()
        {
            if (State != GameState.Simulation && State != GameState.Failure && State != GameState.Pause) return;
            StopPendingPanel();
            if (State == GameState.Pause) GameManager.Instance.State.Resume();
            Rebuild(_snapshot);
            StartSimulation();
        }

        /// <summary>MODIFIER: back to the placement phase with the previous placements.</summary>
        public void Modify()
        {
            StopPendingPanel();
            Rebuild(_snapshot);
            SetState(GameState.Placement);
            Placement.SetInputEnabled(true);
        }

        /// <summary>RESET: level back to its initial state, every object back to the inventory.</summary>
        public void ResetLevel()
        {
            StopPendingPanel();
            _snapshot.Clear();
            Rebuild(null);
            SetState(GameState.Placement);
            Placement.SetInputEnabled(true);
        }

        public void Pause()
        {
            if (State != GameState.Placement && State != GameState.Simulation && State != GameState.Failure) return;
            Runner.Paused = true;
            Placement.SetInputEnabled(false);
            SetState(GameState.Pause);
        }

        public void Resume()
        {
            if (State != GameState.Pause) return;
            GameState resume = GameManager.Instance.State.ResumeState;
            if (!GameManager.Instance.State.Resume()) return;
            Runner.Paused = false;
            if (resume == GameState.Placement) Placement.SetInputEnabled(true);
            StateChanged?.Invoke(State);
        }

        public void SetFastSimulation(bool fast)
        {
            Runner.SpeedMultiplier = fast ? 2f : 1f;
            GameManager.Instance.SetFastSimulation(fast);
        }

        public bool IsFastSimulation => Runner.SpeedMultiplier > 1f;

        public void RequestHint()
        {
            if (!Hints.CanRequestHint(Session.Failures)) return;
            Hints.RequestNextHint(Context);
        }

        public bool HasNextLevel => Level != null && GameManager.Instance.Database.GetNextLevelId(Level.levelId) > 0;

        public void NextLevel()
        {
            GameManager game = GameManager.Instance;
            int next = game.Database.GetNextLevelId(Level.levelId);
            if (next <= 0 || !game.Progression.IsLevelUnlocked(next))
            {
                game.GoToWorldSelect();
                return;
            }

            LoadLevel(next);
        }

        public void QuitToLevelSelect()
        {
            StopPendingPanel();
            Runner.Stop();
            if (Session != null && !Session.Completed) GameEvents.RaiseLevelAbandoned(Level.levelId);
            GameManager.Instance.GoToLevelSelect(Level != null ? Level.worldId : 1);
        }

        // ------------------------------------------------------------------ Simulation

        private void StartSimulation()
        {
            GameManager game = GameManager.Instance;
            Session.Attempts++;
            game.RecordAttempt(Level.levelId);
            Placement.SetInputEnabled(false);
            Context.Tracker.Begin(Session.Attempts, Context.Crystals.Count, Placement.Inventory.GetUnitsUsed());
            Context.StartSimulation();
            Runner.Paused = false;
            Runner.Begin(Context);
            SetState(GameState.Simulation);
            AudioManager.PlaySfx(SfxId.Go);
            Haptics.Play(HapticType.Medium);
            GameEvents.RaiseAttemptStarted(Level.levelId, Session.Attempts);
            SimulationStarted?.Invoke();
        }

        private void OnStepCompleted()
        {
            if (State != GameState.Simulation || Context == null || !Context.IsSimulating) return;
            float limit = Level.simulationTimeLimit > 0f ? Level.simulationTimeLimit : GameplayConfig.Instance.defaultTimeLimit;
            if (Context.SimulationTime >= limit) OnFailureRequested(FailureReason.Timeout);
        }

        private void OnGoalReached()
        {
            if (State != GameState.Simulation) return;
            Context.StopSimulation();
            Runner.Stop();

            LevelRunStats stats = Context.Tracker.Stats;
            stats.ReachedGoal = true;
            stats.ElapsedTime = Context.SimulationTime;
            LastStats = stats;
            Context.Trykli.Win(Context.Goal != null ? Context.Goal.Center : Context.Trykli.Position);

            LastResult = StarEvaluator.Evaluate(Level.levelId, Level.starObjectives, stats);
            LastOutcome = GameManager.Instance.RecordVictory(LastResult);
            Session.Completed = true;
            SetState(GameState.Victory);
            AudioManager.PlaySfx(SfxId.Victory);
            Haptics.Play(HapticType.Success);
            _pendingPanel = StartCoroutine(RaiseAfter(VictoryPanelDelay, () => VictoryReady?.Invoke()));
        }

        private void OnFailureRequested(FailureReason reason)
        {
            if (State != GameState.Simulation) return;
            Context.StopSimulation();
            Runner.Stop();
            LastFailure = reason;
            LastStats = Context.Tracker.Stats;
            Session.Failures++;
            SetState(GameState.Failure);
            if (reason != FailureReason.Hazard && reason != FailureReason.Explosion) AudioManager.PlaySfx(SfxId.Failure);
            GameEvents.RaiseLevelFailed(Level.levelId, reason.ToString());
            _pendingPanel = StartCoroutine(RaiseAfter(FailurePanelDelay, () => FailureReady?.Invoke()));
        }

        private IEnumerator RaiseAfter(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            _pendingPanel = null;
            action();
        }

        private void StopPendingPanel()
        {
            if (_pendingPanel == null) return;
            StopCoroutine(_pendingPanel);
            _pendingPanel = null;
        }

        private void SetState(GameState state)
        {
            if (!GameManager.Instance.State.TrySet(state))
            {
                Debug.LogWarning($"[TRYKLI] Invalid state transition {State} -> {state}.");
                return;
            }

            StateChanged?.Invoke(state);
        }

        private void OnDestroy()
        {
            if (Current == this) Current = null;
        }
    }
}
