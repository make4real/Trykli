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
    /// <summary>Boot scene: initializes the game, shows the studio / TRYKLI logos, then opens the main menu.</summary>
    public sealed class BootController : MonoBehaviour
    {
        private const float StudioDuration = 0.9f;
        private const float LogoDuration = 1.1f;

        private IEnumerator Start()
        {
            GameManager game = GameManager.EnsureExists();
            CameraController.EnsureMainCamera(UITheme.MenuBottom);
            int testLevel = AppInfo.ConsumeEditorTestLevel();
            if (testLevel > 0)
            {
                game.PlayLevel(testLevel);
                yield break;
            }

            Canvas canvas = UIFactory.CreateCanvas("BootCanvas", 10);
            UIFactory.Background(canvas.transform, UITheme.MenuBottom, UITheme.MenuTop);
            RectTransform safe = UIFactory.CreateSafeArea(canvas.transform);
            TextMeshProUGUI studio = UIFactory.LocText(safe, "Studio", "splash.studio", UITheme.HeadingSize, UITheme.TextLight);
            UIFactory.Anchor(studio.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(900f, 200f), Vector2.zero);
            UITween.PopIn(studio);
            yield return WaitOrTap(StudioDuration);

            studio.gameObject.SetActive(false);
            TextMeshProUGUI logo = UIFactory.Text(safe, "Logo", "TRYKLI", 200f, UITheme.Primary, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.Anchor(logo.rectTransform, new Vector2(0.5f, 0.55f), new Vector2(1000f, 260f), Vector2.zero);
            UITween.PopIn(logo);
            RectTransform trykli = UITrykli.Create(safe, 200f, new Color(1f, 0.62f, 0.2f));
            UIFactory.Anchor(trykli, new Vector2(0.5f, 0.38f), new Vector2(200f, 200f), Vector2.zero);
            UITween.PopIn(trykli, 0.2f);
            yield return WaitOrTap(LogoDuration);
            game.GoToMainMenu();
        }

        private static IEnumerator WaitOrTap(float duration)
        {
            float time = 0f;
            yield return null;
            while (time < duration && !Input.GetMouseButtonDown(0) && Input.touchCount == 0)
            {
                time += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
