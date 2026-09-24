using System.Collections.Generic;
using Trykli.Core;
using Trykli.Localization;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.UI
{
    /// <summary>Settings: music, sounds, vibrations, language, reset progression, credits, privacy policy.</summary>
    public sealed class SettingsPanel : ModalPanel
    {
        private UIButton _music;
        private UIButton _sounds;
        private UIButton _vibrations;
        private UIButton _language;

        /// <summary>Called after the progression has been reset (menus refresh their content).</summary>
        public System.Action ProgressReset;

        protected override void Build()
        {
            GameManager game = GameManager.Instance;
            AddTitle("settings.title");
            _music = AddButton("settings.music", UITheme.Secondary, () =>
            {
                game.SetMusicEnabled(!game.Settings.musicOn);
                Refresh();
            }, ArtId.IconMusic);
            _sounds = AddButton("settings.sounds", UITheme.Secondary, () =>
            {
                game.SetSfxEnabled(!game.Settings.sfxOn);
                Refresh();
            }, ArtId.IconSound);
            _vibrations = AddButton("settings.vibrations", UITheme.Secondary, () =>
            {
                game.SetVibrationEnabled(!game.Settings.vibrationOn);
                Refresh();
            }, ArtId.IconVibration);
            _language = AddButton("settings.language", UITheme.Neutral, CycleLanguage, ArtId.IconLanguage);

            RectTransform row = AddRow(UITheme.ButtonHeight);
            AddButton("settings.credits", UITheme.Neutral, () => Open<CreditsPanel>(transform.parent), null, row);
            AddButton("settings.privacy", UITheme.Neutral, OpenPrivacyPolicy, null, row);

            AddButton("settings.reset", UITheme.Danger, () => ConfirmDialog.Show(transform.parent, "settings.reset_confirm", () =>
            {
                game.ResetProgress();
                Toast.Show(transform.parent, Loc.Get("settings.reset_done"));
                ProgressReset?.Invoke();
            }), ArtId.IconReset);

            AddButton("common.close", UITheme.Primary, Close, ArtId.IconCheck);
            Refresh();
            Loc.LanguageChanged += Refresh;
        }

        private void OnDestroy()
        {
            Loc.LanguageChanged -= Refresh;
        }

        private void Refresh()
        {
            GameManager game = GameManager.Instance;
            SetToggle(_music, "settings.music", game.Settings.musicOn);
            SetToggle(_sounds, "settings.sounds", game.Settings.sfxOn);
            SetToggle(_vibrations, "settings.vibrations", game.Settings.vibrationOn);
            string languageName = Loc.CurrentLanguage;
            foreach (Loc.LanguageInfo language in Loc.AvailableLanguages)
            {
                if (language.Code == Loc.CurrentLanguage) languageName = language.DisplayName;
            }

            _language.SetLabelKey("settings.language", languageName);
        }

        private static void SetToggle(UIButton button, string key, bool on)
        {
            button.SetLabelKey(key, Loc.Get(on ? "common.on" : "common.off"));
            button.SetColor(on ? UITheme.Secondary : UITheme.Neutral);
        }

        private void CycleLanguage()
        {
            IReadOnlyList<Loc.LanguageInfo> languages = Loc.AvailableLanguages;
            if (languages.Count == 0) return;
            int index = 0;
            for (int i = 0; i < languages.Count; i++)
            {
                if (languages[i].Code == Loc.CurrentLanguage) index = i;
            }

            GameManager.Instance.SetLanguage(languages[(index + 1) % languages.Count].Code);
        }

        private void OpenPrivacyPolicy()
        {
            if (string.IsNullOrEmpty(AppLinks.PrivacyPolicyUrl)) Toast.Show(transform.parent, Loc.Get("common.coming_soon"));
            else Application.OpenURL(AppLinks.PrivacyPolicyUrl);
        }
    }

}
