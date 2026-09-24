using System.Collections.Generic;
using NUnit.Framework;
using Trykli.Localization;
using Trykli.Data;
using Trykli.Objectives;
using UnityEngine;

namespace Trykli.Tests
{
    /// <summary>Checks the shipped content: the 100 JSON level definitions, the worlds and the localization files.</summary>
    public class LevelContentTests
    {
        private const int LevelCount = 100;
        private const int WorldCount = 10;
        private const int LevelsPerWorld = 10;

        private static List<LevelDefinition> LoadLevels()
        {
            var levels = new List<LevelDefinition>();
            foreach (TextAsset asset in Resources.LoadAll<TextAsset>(LevelDatabase.DefinitionsFolder))
            {
                if (asset.name == "worlds") continue;
                LevelDefinition definition = LevelDefinitionConverter.ParseLevel(asset.text);
                Assert.IsNotNull(definition, "Unreadable level file " + asset.name);
                levels.Add(definition);
            }

            levels.Sort((a, b) => a.levelId.CompareTo(b.levelId));
            return levels;
        }

        private static Dictionary<string, string> LoadLanguage(string code)
        {
            TextAsset asset = Resources.Load<TextAsset>(Loc.ResourceFolder + "/" + code);
            Assert.IsNotNull(asset, "Missing localization file " + code);
            var table = JsonUtility.FromJson<LocalizationTable>(asset.text);
            return table.ToDictionary();
        }

        [Test]
        public void ThereAreExactly100LevelsWithConsecutiveIds()
        {
            List<LevelDefinition> levels = LoadLevels();
            Assert.AreEqual(LevelCount, levels.Count);
            for (int i = 0; i < levels.Count; i++)
            {
                LevelDefinition level = levels[i];
                Assert.AreEqual(i + 1, level.levelId);
                Assert.AreEqual(i / LevelsPerWorld + 1, level.worldId, "World of level " + level.levelId);
                Assert.AreEqual(i % LevelsPerWorld + 1, level.levelNumber, "Number of level " + level.levelId);
            }
        }

        [Test]
        public void EveryLevelPassesTheValidator()
        {
            List<LevelValidator.Issue> issues = LevelValidator.ValidateAll(LoadLevels(), WorldCount, LevelsPerWorld, ItemCatalog.CreateDefault());
            var errors = new List<string>();
            foreach (LevelValidator.Issue issue in issues)
            {
                if (issue.Severity == LevelValidator.Severity.Error) errors.Add(issue.ToString());
            }

            Assert.IsEmpty(errors, string.Join("\n", errors));
        }

        [Test]
        public void EveryLevelHasThreeObjectivesStartingWithComplete()
        {
            foreach (LevelDefinition level in LoadLevels())
            {
                Assert.AreEqual(StarEvaluator.StarsPerLevel, level.objectives.Count, "Level " + level.levelId);
                Assert.AreEqual(ObjectiveDefinition.Complete, level.objectives[0].type, "Level " + level.levelId);
            }
        }

        [Test]
        public void EveryLevelHasAReferenceSolutionUsingItsInventory()
        {
            foreach (LevelDefinition level in LoadLevels())
            {
                var available = new Dictionary<string, int>();
                foreach (ItemCount count in level.inventory) available[count.itemId] = count.count;
                Assert.IsNotEmpty(level.solution, "Level " + level.levelId + " has no reference solution.");
                foreach (SolutionStep step in level.solution)
                {
                    Assert.IsTrue(available.ContainsKey(step.itemId), $"Level {level.levelId}: solution uses '{step.itemId}' not in the inventory.");
                    Assert.That(step.zoneIndex, Is.InRange(0, level.layout.zones.Count - 1), "Level " + level.levelId);
                }
            }
        }

        [Test]
        public void BossesAreEveryTenLevels()
        {
            foreach (LevelDefinition level in LoadLevels())
            {
                Assert.AreEqual(level.levelNumber == LevelsPerWorld, level.isBoss, "Level " + level.levelId);
            }
        }

        [Test]
        public void LevelLayoutsAreUnique()
        {
            var signatures = new Dictionary<string, int>();
            foreach (LevelDefinition level in LoadLevels())
            {
                string signature = JsonUtility.ToJson(level.layout);
                if (signatures.TryGetValue(signature, out int other)) Assert.Fail($"Levels {other} and {level.levelId} share the same layout.");
                signatures[signature] = level.levelId;
            }
        }

        [Test]
        public void WorldsFileDescribesTenWorlds()
        {
            TextAsset asset = Resources.Load<TextAsset>(LevelDatabase.WorldsResource);
            Assert.IsNotNull(asset);
            WorldCollectionDefinition worlds = LevelDefinitionConverter.ParseWorlds(asset.text);
            Assert.AreEqual(WorldCount, worlds.worlds.Count);
            for (int i = 0; i < worlds.worlds.Count; i++) Assert.AreEqual(i + 1, worlds.worlds[i].worldId);
        }

        [Test]
        public void FrenchAndEnglishHaveTheSameKeys()
        {
            Dictionary<string, string> fr = LoadLanguage("fr");
            Dictionary<string, string> en = LoadLanguage("en");
            var missingInEnglish = new List<string>();
            var missingInFrench = new List<string>();
            foreach (string key in fr.Keys)
            {
                if (!en.ContainsKey(key)) missingInEnglish.Add(key);
            }

            foreach (string key in en.Keys)
            {
                if (!fr.ContainsKey(key)) missingInFrench.Add(key);
            }

            Assert.IsEmpty(missingInEnglish, "Missing in en: " + string.Join(", ", missingInEnglish));
            Assert.IsEmpty(missingInFrench, "Missing in fr: " + string.Join(", ", missingInFrench));
        }

        [Test]
        public void EveryLevelTextIsLocalized()
        {
            Dictionary<string, string> fr = LoadLanguage("fr");
            var missing = new List<string>();
            foreach (LevelDefinition level in LoadLevels())
            {
                if (!fr.ContainsKey(level.nameKey)) missing.Add(level.nameKey);
                if (!string.IsNullOrEmpty(level.tipKey) && !fr.ContainsKey(level.tipKey)) missing.Add(level.tipKey);
                foreach (ObjectiveDefinition objective in level.objectives)
                {
                    if (!string.IsNullOrEmpty(objective.descriptionKey) && !fr.ContainsKey(objective.descriptionKey)) missing.Add(objective.descriptionKey);
                }

                foreach (HintData hint in level.hints)
                {
                    if (!string.IsNullOrEmpty(hint.textKey) && !fr.ContainsKey(hint.textKey)) missing.Add(hint.textKey);
                }
            }

            Assert.IsEmpty(missing, "Missing keys: " + string.Join(", ", missing));
        }

        [Test]
        public void DefinitionRoundTripsThroughLevelData()
        {
            LevelDefinition original = LoadLevels()[41];
            ItemCatalog catalog = ItemCatalog.CreateDefault();
            var data = ScriptableObject.CreateInstance<LevelData>();
            try
            {
                LevelDefinitionConverter.Apply(original, data, catalog);
                LevelDefinition back = LevelDefinitionConverter.ToDefinition(data);
                Assert.AreEqual(original.levelId, back.levelId);
                Assert.AreEqual(original.layout.elements.Count, back.layout.elements.Count);
                Assert.AreEqual(original.layout.zones.Count, back.layout.zones.Count);
                Assert.AreEqual(original.objectives.Count, back.objectives.Count);
                Assert.AreEqual(original.solution.Count, back.solution.Count);
            }
            finally
            {
                Object.DestroyImmediate(data);
            }
        }

        [Test]
        public void ValidatorDetectsBrokenLevels()
        {
            var level = new LevelDefinition { levelId = 5, worldId = 42, levelNumber = 5, nameKey = "x" };
            level.layout.goalRadius = 0f;
            level.layout.spawn = new Vector2(99f, 99f);
            List<LevelValidator.Issue> issues = LevelValidator.Validate(level, WorldCount, LevelsPerWorld);
            string all = string.Join("\n", issues);
            StringAssert.Contains("Invalid worldId", all);
            StringAssert.Contains("Spawn", all);
            StringAssert.Contains("no exit", all);
            StringAssert.Contains("star objectives", all);

            var duplicate = new List<LevelDefinition> { LoadLevels()[0], LoadLevels()[0] };
            StringAssert.Contains("Duplicate levelId", string.Join("\n", LevelValidator.ValidateAll(duplicate, WorldCount, LevelsPerWorld)));
        }
    }
}
