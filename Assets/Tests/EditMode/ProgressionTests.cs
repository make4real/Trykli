using NUnit.Framework;
using Trykli.Objectives;
using Trykli.Save;

namespace Trykli.Tests
{
    public class ProgressionTests
    {
        private SaveData _data;
        private ProgressionService _progression;

        [SetUp]
        public void SetUp()
        {
            _data = new SaveData();
            _progression = new ProgressionService(_data, 100, 10);
        }

        private static LevelResult Win(int levelId, int stars, int crystals = 0, float time = 5f, int items = 1)
        {
            return new LevelResult
            {
                levelId = levelId, completed = true, stars = stars, crystals = crystals, time = time, itemsUsed = items,
                objectiveMask = (1 << stars) - 1
            };
        }

        [Test]
        public void OnlyFirstLevelIsUnlockedAtStart()
        {
            Assert.IsTrue(_progression.IsLevelUnlocked(1));
            Assert.IsFalse(_progression.IsLevelUnlocked(2));
            Assert.IsTrue(_progression.IsWorldUnlocked(1));
            Assert.IsFalse(_progression.IsWorldUnlocked(2));
            Assert.AreEqual(10, _progression.WorldCount);
        }

        [Test]
        public void CompletingALevelUnlocksTheNextOne()
        {
            ProgressionService.RecordOutcome outcome = _progression.RecordResult(Win(1, 2));

            Assert.IsTrue(outcome.FirstCompletion);
            Assert.AreEqual(2, outcome.UnlockedLevelId);
            Assert.IsTrue(_progression.IsLevelUnlocked(2));
            Assert.IsTrue(_progression.IsLevelCompleted(1));
            Assert.AreEqual(2, _progression.GetStars(1));
        }

        [Test]
        public void FailedRunDoesNotChangeProgress()
        {
            _progression.RecordResult(new LevelResult { levelId = 1, completed = false });

            Assert.IsFalse(_progression.IsLevelCompleted(1));
            Assert.IsFalse(_progression.IsLevelUnlocked(2));
        }

        [Test]
        public void BestStarsAndCrystalsAreKept()
        {
            _progression.RecordResult(Win(1, 3, crystals: 2));
            ProgressionService.RecordOutcome outcome = _progression.RecordResult(Win(1, 1, crystals: 0));

            Assert.IsFalse(outcome.FirstCompletion);
            Assert.IsFalse(outcome.NewBestStars);
            Assert.AreEqual(3, _progression.GetStars(1));
            Assert.AreEqual(2, _progression.GetCrystals(1));
        }

        [Test]
        public void ReplayingAnOldLevelDoesNotRelockAnything()
        {
            for (int id = 1; id <= 5; id++) _progression.RecordResult(Win(id, 1));
            _progression.RecordResult(Win(2, 3));

            Assert.AreEqual(6, _progression.HighestUnlockedLevel);
        }

        [Test]
        public void FinishingTheBossUnlocksTheNextWorld()
        {
            for (int id = 1; id <= 9; id++) _progression.RecordResult(Win(id, 1));
            Assert.IsFalse(_progression.IsWorldUnlocked(2));

            ProgressionService.RecordOutcome outcome = _progression.RecordResult(Win(10, 1));

            Assert.AreEqual(2, outcome.UnlockedWorldId);
            Assert.IsTrue(_progression.IsWorldUnlocked(2));
            Assert.IsTrue(_progression.IsLevelUnlocked(11));
        }

        [Test]
        public void FinishingLevel100CompletesTheGameOnce()
        {
            _data.highestUnlockedLevel = 100;
            ProgressionService.RecordOutcome first = _progression.RecordResult(Win(100, 1));
            ProgressionService.RecordOutcome second = _progression.RecordResult(Win(100, 3));

            Assert.IsTrue(first.GameCompleted);
            Assert.IsFalse(second.GameCompleted);
            Assert.IsTrue(_data.masterAchieved);
            Assert.AreEqual(100, _progression.HighestUnlockedLevel, "Nothing exists after level 100.");
        }

        [Test]
        public void WorldHelpersMapLevelsToWorlds()
        {
            Assert.AreEqual(1, _progression.WorldOf(1));
            Assert.AreEqual(1, _progression.WorldOf(10));
            Assert.AreEqual(2, _progression.WorldOf(11));
            Assert.AreEqual(10, _progression.WorldOf(100));
            Assert.AreEqual(41, _progression.FirstLevelOf(5));
            Assert.AreEqual(10, _progression.LevelNumberInWorld(20));
        }

        [Test]
        public void TotalsAndContinueLevel()
        {
            _progression.RecordResult(Win(1, 3, crystals: 1));
            _progression.RecordResult(Win(2, 2, crystals: 2));

            Assert.AreEqual(5, _progression.TotalStars);
            Assert.AreEqual(3, _progression.TotalCrystals);
            Assert.AreEqual(2, _progression.CompletedLevels);
            Assert.AreEqual(3, _progression.GetContinueLevelId());
            Assert.AreEqual(5, _progression.GetWorldStars(1));
        }

        [Test]
        public void BestTimeAndItemsKeepTheLowestValue()
        {
            _progression.RecordResult(Win(1, 1, time: 6f, items: 3));
            _progression.RecordResult(Win(1, 1, time: 4f, items: 2));
            _progression.RecordResult(Win(1, 1, time: 9f, items: 5));

            LevelProgress progress = _data.FindLevel(1);
            Assert.AreEqual(4f, progress.bestTime, 1e-4f);
            Assert.AreEqual(2, progress.bestItemsUsed);
            Assert.AreEqual(3, progress.victories);
        }
    }
}
