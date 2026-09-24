using System.Collections;
using Trykli.Audio;
using Trykli.Core;
using Trykli.Gameplay;
using Trykli.Localization;
using Trykli.Utilities;
using TMPro;
using UnityEngine;

namespace Trykli.UI
{
    /// <summary>
    /// Gameplay scene: a single scene for the 100 levels. Builds the camera, the LevelManager and the HUD,
    /// then loads the pending level (or the continue level when the scene is played directly).
    /// </summary>
    public sealed class GameplayController : MonoBehaviour
    {
        private void Start()
        {
            GameManager game = GameManager.EnsureExists();
            CameraController cameraRig = CameraController.EnsureMainCamera(UITheme.MenuBottom);

            var levelRoot = new GameObject("LevelManager");
            var level = levelRoot.AddComponent<LevelManager>();
            level.Initialize(cameraRig);
            GameplayHUD.Create(level);

            int levelId = game.PendingLevelId;
            if (levelId <= 0) levelId = AppInfo.ConsumeEditorTestLevel();
            if (levelId <= 0) levelId = game.Progression.GetContinueLevelId();
            if (game.Database.GetLevel(levelId) == null)
            {
                Debug.LogError("[TRYKLI] No level to load. " + Loc.Get("error.no_levels"));
                return;
            }

            level.LoadLevel(levelId);
        }
    }
}
