using System.Collections.Generic;
using Trykli.Core;
using Trykli.Data;
using Trykli.Localization;
using Trykli.Save;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>Home screen: TRYKLI logo, JOUER, NIVEAUX, PARAMÈTRES, SKINS, version.</summary>
    public sealed class MainMenuScreen : MonoBehaviour
    {
        private RectTransform _safe;
        private TextMeshProUGUI _progress;

        public static MainMenuScreen Create()
        {
            Canvas canvas = UIFactory.CreateCanvas("MainMenuCanvas", 10);
            UIFactory.Background(canvas.transform, UITheme.MenuBottom, UITheme.MenuTop);
            var screen = canvas.gameObject.AddComponent<MainMenuScreen>();
            screen.Build(canvas);
            return screen;
        }

        private void Build(Canvas canvas)
        {
            GameManager game = GameManager.Instance;
            _safe = UIFactory.CreateSafeArea(canvas.transform);

            TextMeshProUGUI logo = UIFactory.Text(_safe, "Logo", "TRYKLI", 190f, UITheme.Primary, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.Anchor(logo.rectTransform, new Vector2(0.5f, 0.84f), new Vector2(1000f, 240f), Vector2.zero);
            logo.outlineWidth = 0.18f;
            logo.outlineColor = new Color32(40, 30, 60, 255);
            UITween.Pulse(logo, 0.02f);

            TextMeshProUGUI tagline = UIFactory.LocText(_safe, "Tagline", "menu.tagline", UITheme.BodySize, UITheme.TextLight);
            UIFactory.Anchor(tagline.rectTransform, new Vector2(0.5f, 0.76f), new Vector2(960f, 80f), Vector2.zero);

            SkinDefinition skin = SkinCatalog.Get(game.SaveSystem.Data.selectedSkin);
            RectTransform trykli = UITrykli.Create(_safe, 260f, skin.BodyColor);
            UIFactory.Anchor(trykli, new Vector2(0.5f, 0.6f), new Vector2(260f, 260f), Vector2.zero);
            UITween.Float(trykli, 20f);

            RectTransform buttons = UIFactory.CreateRect("Buttons", _safe);
            UIFactory.Anchor(buttons, new Vector2(0.5f, 0.27f), new Vector2(760f, 700f), Vector2.zero);
            UIFactory.Vertical(buttons.gameObject, 30f, null, TextAnchor.MiddleCenter);

            UIButton play = UIFactory.Button(buttons, "Play", "menu.play", UITheme.Primary, game.PlayContinue, ArtId.IconPlay, 72f);
            UIFactory.SetPreferredSize(play.gameObject, -1f, 190f);
            UITween.Pulse(play, 0.025f);
            UIButton levels = UIFactory.Button(buttons, "Levels", "menu.levels", UITheme.Secondary, game.GoToWorldSelect, ArtId.IconLevels);
            UIFactory.SetPreferredSize(levels.gameObject, -1f, UITheme.ButtonHeight);
            UIButton settings = UIFactory.Button(buttons, "Settings", "menu.settings", UITheme.Neutral,
                () => ModalPanel.Open<SettingsPanel>(_safe).ProgressReset = RefreshProgress, ArtId.IconSettings);
            UIFactory.SetPreferredSize(settings.gameObject, -1f, UITheme.ButtonHeight);
            UIButton skins = UIFactory.Button(buttons, "Skins", "menu.skins", UITheme.Neutral, () => ModalPanel.Open<SkinsPanel>(_safe), ArtId.IconSkins);
            UIFactory.SetPreferredSize(skins.gameObject, -1f, UITheme.ButtonHeight);

            _progress = UIFactory.Text(_safe, "Progress", string.Empty, UITheme.SmallSize, UITheme.TextLight);
            UIFactory.Anchor(_progress.rectTransform, new Vector2(0.5f, 0.06f), new Vector2(900f, 60f), Vector2.zero);
            TextMeshProUGUI version = UIFactory.Text(_safe, "Version", AppInfo.VersionLabel, 30f, UITheme.TextLight.WithAlpha(0.6f));
            UIFactory.Anchor(version.rectTransform, new Vector2(0.5f, 0.02f), new Vector2(600f, 50f), Vector2.zero);
            RefreshProgress();
            Loc.LanguageChanged += RefreshProgress;
        }

        private void OnDestroy()
        {
            Loc.LanguageChanged -= RefreshProgress;
        }

        private void RefreshProgress()
        {
            ProgressionService progression = GameManager.Instance.Progression;
            _progress.text = Loc.Format("menu.progress", progression.TotalStars, progression.TotalLevels * 3, progression.TotalCrystals);
        }
    }
}
