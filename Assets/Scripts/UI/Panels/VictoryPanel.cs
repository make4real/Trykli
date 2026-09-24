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
    /// <summary>NIVEAU TERMINÉ: stars, fulfilled objectives, NIVEAU SUIVANT / REJOUER / NIVEAUX.</summary>
    public sealed class VictoryPanel : ModalPanel
    {
        public LevelManager Level;
        public System.Action MasterRequested;

        protected override void Build() { }

        public void Populate()
        {
            LevelResult result = Level.LastResult;
            ProgressionService.RecordOutcome outcome = Level.LastOutcome;
            AddTitle("victory.title");

            Image[] stars = UIFactory.Stars(Content, "Stars", result.stars, 3, 150f);
            for (int i = 0; i < stars.Length; i++) UITween.PopIn(stars[i], 0.2f + i * 0.18f);

            IReadOnlyList<StarObjective> objectives = Level.Level.starObjectives;
            for (int i = 0; i < objectives.Count && i < 3; i++)
            {
                AddObjectiveRow(objectives[i] != null ? objectives[i].GetDescription() : "-", result.IsObjectiveMet(i));
            }

            string stats = Loc.Format("victory.stats", result.time.ToString("0.0"), result.crystals, Level.Context.Crystals.Count);
            AddRawText(stats, UITheme.SmallSize, UITheme.TextMuted);
            if (outcome.NewBestStars && !outcome.FirstCompletion) AddText("victory.new_best", UITheme.SmallSize, UITheme.Success);
            if (outcome.UnlockedWorldId > 0) AddText("victory.world_unlocked", UITheme.BodySize, UITheme.Primary, outcome.UnlockedWorldId);

            if (outcome.GameCompleted || (Level.Level.levelId >= GameManager.Instance.Database.LevelCount && GameManager.Instance.SaveSystem.Data.masterAchieved))
            {
                AddButton("master.button", UITheme.Primary, () =>
                {
                    Close();
                    MasterRequested?.Invoke();
                }, ArtId.Star);
            }
            else if (Level.HasNextLevel)
            {
                AddButton("victory.next", UITheme.Primary, () =>
                {
                    Close();
                    Level.NextLevel();
                }, ArtId.IconPlay);
            }

            RectTransform row = AddRow(UITheme.ButtonHeight);
            AddButton("victory.replay", UITheme.Secondary, () =>
            {
                Close();
                Level.ResetLevel();
            }, ArtId.IconRestart, row);
            AddButton("victory.levels", UITheme.Neutral, () =>
            {
                Close();
                Level.QuitToLevelSelect();
            }, ArtId.IconLevels, row);
        }

        private void AddObjectiveRow(string description, bool met)
        {
            RectTransform row = UIFactory.CreateRect("Objective", Content);
            UIFactory.Horizontal(row.gameObject, 20f, null, TextAnchor.MiddleLeft);
            UIFactory.SetPreferredSize(row.gameObject, -1f, 70f);
            Image icon = UIFactory.Image(row, "Icon", met ? ArtId.IconCheck : ArtId.IconCross, met ? UITheme.Success : UITheme.Danger);
            UIFactory.SetPreferredSize(icon.gameObject, 60f, 60f);
            TextMeshProUGUI text = UIFactory.Text(row, "Text", description, UITheme.BodySize * 0.85f, met ? UITheme.TextDark : UITheme.TextMuted, TextAlignmentOptions.Left);
            UIFactory.SetPreferredSize(text.gameObject, -1f, 60f, 1f);
            UIFactory.AutoSize(text, 26f, UITheme.BodySize * 0.85f);
        }
    }
}
