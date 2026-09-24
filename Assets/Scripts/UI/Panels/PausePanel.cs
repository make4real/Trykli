using System.Collections.Generic;
using Trykli.Core;
using Trykli.Gameplay;
using Trykli.Localization;
using Trykli.Objectives;
using Trykli.Save;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>CONTINUER / RECOMMENCER / PARAMÈTRES / QUITTER LE NIVEAU.</summary>
    public sealed class PausePanel : ModalPanel
    {
        public LevelManager Level;

        protected override void Build()
        {
            AddTitle("pause.title");
            AddButton("pause.continue", UITheme.Primary, () =>
            {
                Close();
                Level.Resume();
            }, ArtId.IconPlay);
            AddButton("pause.restart", UITheme.Secondary, () =>
            {
                Close();
                Level.Resume();
                Level.ResetLevel();
            }, ArtId.IconRestart);
            AddButton("pause.settings", UITheme.Neutral, () => Open<SettingsPanel>(transform.parent), ArtId.IconSettings);
            AddButton("pause.quit", UITheme.Danger, () =>
            {
                Close();
                Level.QuitToLevelSelect();
            }, ArtId.IconHome);
        }
    }
}
