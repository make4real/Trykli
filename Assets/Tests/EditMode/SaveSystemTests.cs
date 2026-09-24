using NUnit.Framework;
using Trykli.Save;

namespace Trykli.Tests
{
    public class SaveSystemTests
    {
        [Test]
        public void NewSave_HasOnlyFirstLevelAndWorldUnlocked()
        {
            var manager = new SaveManager(new MemorySaveStorage());
            manager.Load();

            Assert.AreEqual(1, manager.Data.highestUnlockedLevel);
            CollectionAssert.AreEqual(new[] { 1 }, manager.Data.unlockedWorlds);
            Assert.AreEqual(SaveManager.CurrentVersion, manager.Data.version);
            Assert.IsEmpty(manager.Data.levels);
        }

        [Test]
        public void SaveThenLoad_RoundTripsProgressAndSettings()
        {
            var storage = new MemorySaveStorage();
            var manager = new SaveManager(storage);
            manager.Load();
            manager.Data.highestUnlockedLevel = 12;
            manager.Data.unlockedWorlds.Add(2);
            LevelProgress progress = manager.Data.GetOrCreateLevel(3);
            progress.completed = true;
            progress.bestStars = 2;
            progress.bestCrystals = 1;
            manager.Data.settings.musicOn = false;
            manager.Data.settings.language = "en";
            manager.Save();

            var reloaded = new SaveManager(storage);
            reloaded.Load();

            Assert.AreEqual(12, reloaded.Data.highestUnlockedLevel);
            CollectionAssert.Contains(reloaded.Data.unlockedWorlds, 2);
            LevelProgress loaded = reloaded.Data.FindLevel(3);
            Assert.IsNotNull(loaded);
            Assert.IsTrue(loaded.completed);
            Assert.AreEqual(2, loaded.bestStars);
            Assert.AreEqual(1, loaded.bestCrystals);
            Assert.IsFalse(reloaded.Data.settings.musicOn);
            Assert.AreEqual("en", reloaded.Data.settings.language);
        }

        [Test]
        public void CorruptedMainFile_FallsBackToBackup()
        {
            var storage = new MemorySaveStorage();
            var manager = new SaveManager(storage);
            manager.Load();
            manager.Data.highestUnlockedLevel = 7;
            manager.Save();
            manager.Data.highestUnlockedLevel = 8;
            manager.Save();

            storage.Content = "{ this is not json";
            var reloaded = new SaveManager(storage);
            reloaded.Load();

            Assert.AreEqual(7, reloaded.Data.highestUnlockedLevel, "The backup holds the previous save.");
        }

        [Test]
        public void UnreadableFiles_StartANewSave()
        {
            var storage = new MemorySaveStorage { Content = "garbage", Backup = "more garbage" };
            var manager = new SaveManager(storage);
            manager.Load();

            Assert.AreEqual(1, manager.Data.highestUnlockedLevel);
        }

        [Test]
        public void OldVersion_IsMigratedAndRepaired()
        {
            const string json = "{\"version\":0,\"highestUnlockedLevel\":0,\"unlockedWorlds\":[],\"levels\":[]}";
            Assert.IsTrue(SaveManager.TryDeserialize(json, out SaveData data));

            Assert.AreEqual(SaveManager.CurrentVersion, data.version);
            Assert.GreaterOrEqual(data.highestUnlockedLevel, 1);
            CollectionAssert.Contains(data.unlockedWorlds, 1);
            Assert.IsNotNull(data.settings);
        }

        [Test]
        public void ResetProgress_KeepsSettingsByDefault()
        {
            var manager = new SaveManager(new MemorySaveStorage());
            manager.Load();
            manager.Data.highestUnlockedLevel = 30;
            manager.Data.settings.sfxOn = false;

            manager.ResetProgress();

            Assert.AreEqual(1, manager.Data.highestUnlockedLevel);
            Assert.IsFalse(manager.Data.settings.sfxOn);
        }

        [Test]
        public void GetOrCreateLevel_KeepsLevelsSorted()
        {
            var data = new SaveData();
            data.GetOrCreateLevel(5);
            data.GetOrCreateLevel(2);
            data.GetOrCreateLevel(9);
            data.GetOrCreateLevel(2);

            Assert.AreEqual(3, data.levels.Count);
            Assert.AreEqual(2, data.levels[0].levelId);
            Assert.AreEqual(5, data.levels[1].levelId);
            Assert.AreEqual(9, data.levels[2].levelId);
        }
    }
}
