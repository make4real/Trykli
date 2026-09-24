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
    public sealed class MainMenuController : MonoBehaviour
    {
        private void Start()
        {
            GameManager game = GameManager.EnsureExists();
            game.State.TrySet(GameState.Menu);
            CameraController.EnsureMainCamera(UITheme.MenuBottom);
            MainMenuScreen.Create();
            game.Audio.PlayMusic(MusicTrack.Menu);
        }
    }
}
