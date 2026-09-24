using Trykli.Audio;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Localization;
using Trykli.Objectives;
using Trykli.Save;
using UnityEngine;

namespace Trykli.Core
{
    /// <summary>
    /// Persistent root of the game: initialization, services (save, progression, audio, localization),
    /// navigation between scenes, settings and global state. Created automatically by any scene controller,
    /// so every scene can be played directly from the editor.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null) EnsureExists();
                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public SaveManager SaveSystem { get; private set; }
        public ProgressionService Progression { get; private set; }
        public LevelDatabase Database { get; private set; }
        public AudioManager Audio { get; private set; }
        public GameStateMachine State { get; } = new GameStateMachine();
        public SceneFlow Scenes { get; private set; }
        public GameplayConfig Config => GameplayConfig.Instance;

        public SettingsData Settings => SaveSystem.Data.settings;

        /// <summary>Level to load when the Gameplay scene starts.</summary>
        public int PendingLevelId { get; private set; }

        /// <summary>World displayed by the LevelSelect scene.</summary>
        public int SelectedWorldId { get; set; } = 1;

        public static GameManager EnsureExists()
        {
            if (_instance != null) return _instance;
            var existing = FindFirstObjectByType<GameManager>();
            if (existing != null)
            {
                existing.InitializeIfNeeded();
                return existing;
            }

            var go = new GameObject("[TRYKLI] GameManager");
            var manager = go.AddComponent<GameManager>();
            manager.InitializeIfNeeded();
            return manager;
        }

        private bool _initialized;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeIfNeeded();
        }

        private void InitializeIfNeeded()
        {
            if (_initialized) return;
            _initialized = true;
            _instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = false;

            // Physics is stepped manually by the SimulationRunner (deterministic fixed step).
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.gravity = new Vector2(0f, -Config.gravity);

            SaveSystem = new SaveManager(new FileSaveStorage());
            SaveSystem.Load();

            string language = string.IsNullOrEmpty(Settings.language) ? Loc.DetectSystemLanguage() : Settings.language;
            Loc.Initialize(language);
            Settings.language = Loc.CurrentLanguage;

            Database = LevelDatabase.Load();
            if (Database.LevelCount == 0) Debug.LogError("[TRYKLI] No level found. Check Resources/LevelDefinitions or run Tools > TRYKLI > Generate Levels.");
            Progression = new ProgressionService(SaveSystem.Data, Mathf.Max(1, Database.LevelCount), Database.LevelsPerWorld);

            Audio = gameObject.AddComponent<AudioManager>();
            Audio.Initialize(Settings.musicOn, Settings.sfxOn);
            Haptics.Enabled = Settings.vibrationOn;

            Scenes = gameObject.AddComponent<SceneFlow>();
            Scenes.Initialize();

            State.TrySet(GameState.Menu);
        }

        // ------------------------------------------------------------------ Navigation

        public void GoToMainMenu()
        {
            State.TrySet(GameState.Menu);
            Scenes.Load(SceneNames.MainMenu);
        }

        public void GoToWorldSelect()
        {
            State.TrySet(GameState.Menu);
            Scenes.Load(SceneNames.WorldSelect);
        }

        public void GoToLevelSelect(int worldId)
        {
            SelectedWorldId = Mathf.Clamp(worldId, 1, Mathf.Max(1, Progression.WorldCount));
            State.TrySet(GameState.Menu);
            Scenes.Load(SceneNames.LevelSelect);
        }

        /// <summary>"JOUER": continues with the first unfinished level.</summary>
        public void PlayContinue()
        {
            PlayLevel(Progression.GetContinueLevelId());
        }

        public void PlayLevel(int levelId)
        {
            if (Database.GetLevel(levelId) == null)
            {
                Debug.LogError($"[TRYKLI] Level {levelId} does not exist.");
                return;
            }

            SetPendingLevel(levelId);
            Scenes.Load(SceneNames.Gameplay);
        }

        /// <summary>Selects the level loaded by the Gameplay scene (also used when switching level in place).</summary>
        public void SetPendingLevel(int levelId)
        {
            PendingLevelId = levelId;
            LevelData level = Database.GetLevel(levelId);
            if (level != null) SelectedWorldId = level.worldId;
        }

        // ------------------------------------------------------------------ Progression

        public ProgressionService.RecordOutcome RecordVictory(LevelResult result)
        {
            ProgressionService.RecordOutcome outcome = Progression.RecordResult(result);
            SaveSystem.Save();
            GameEvents.RaiseLevelCompleted(result);
            return outcome;
        }

        public void RecordAttempt(int levelId)
        {
            Progression.RecordAttempt(levelId);
        }

        public void ResetProgress()
        {
            SaveSystem.ResetProgress(keepSettings: true);
            Progression = new ProgressionService(SaveSystem.Data, Mathf.Max(1, Database.LevelCount), Database.LevelsPerWorld);
        }

        // ------------------------------------------------------------------ Settings

        public void SetMusicEnabled(bool enabled)
        {
            Settings.musicOn = enabled;
            Audio.SetMusicEnabled(enabled);
            SaveSystem.Save();
        }

        public void SetSfxEnabled(bool enabled)
        {
            Settings.sfxOn = enabled;
            Audio.SetSfxEnabled(enabled);
            SaveSystem.Save();
        }

        public void SetVibrationEnabled(bool enabled)
        {
            Settings.vibrationOn = enabled;
            Haptics.Enabled = enabled;
            if (enabled) Haptics.Play(HapticType.Medium);
            SaveSystem.Save();
        }

        public void SetLanguage(string code)
        {
            Loc.SetLanguage(code);
            Settings.language = Loc.CurrentLanguage;
            SaveSystem.Save();
        }

        public void SetFastSimulation(bool enabled)
        {
            Settings.fastSimulation = enabled;
        }

        public bool IsSkinUnlocked(string skinId) => SkinCatalog.IsUnlocked(skinId, Progression.TotalCrystals);

        public void SelectSkin(string skinId)
        {
            if (!IsSkinUnlocked(skinId)) return;
            SaveSystem.Data.selectedSkin = skinId;
            SaveSystem.Save();
        }

        // ------------------------------------------------------------------ Lifecycle

        private void OnApplicationPause(bool paused)
        {
            if (paused && SaveSystem != null) SaveSystem.Save();
        }

        private void OnApplicationQuit()
        {
            SaveSystem?.Save();
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }
    }
}
