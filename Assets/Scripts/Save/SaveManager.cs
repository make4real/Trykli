using System;
using UnityEngine;

namespace Trykli.Save
{
    /// <summary>
    /// Loads / saves <see cref="SaveData"/> as versioned JSON through an <see cref="ISaveStorage"/>.
    /// Corrupted files fall back to the backup, then to a fresh save.
    /// </summary>
    public sealed class SaveManager
    {
        public const int CurrentVersion = 1;

        private readonly ISaveStorage _storage;

        public SaveManager(ISaveStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Data = new SaveData();
        }

        public SaveData Data { get; private set; }

        public event Action Saved;

        public void Load()
        {
            if (_storage.TryRead(out string json) && TryDeserialize(json, out SaveData data))
            {
                Data = data;
                return;
            }

            if (_storage.TryReadBackup(out string backup) && TryDeserialize(backup, out data))
            {
                Debug.LogWarning("[TRYKLI] Main save unreadable, restored from backup.");
                Data = data;
                return;
            }

            Data = new SaveData();
        }

        public void Save()
        {
            Data.version = CurrentVersion;
            Data.lastSaveUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _storage.Write(Serialize(Data));
            Saved?.Invoke();
        }

        /// <summary>Erases the progression. Settings are kept unless <paramref name="keepSettings"/> is false.</summary>
        public void ResetProgress(bool keepSettings = true)
        {
            SettingsData settings = Data.settings;
            Data = new SaveData();
            if (keepSettings && settings != null) Data.settings = settings;
            Save();
        }

        public static string Serialize(SaveData data)
        {
            return JsonUtility.ToJson(data, true);
        }

        public static bool TryDeserialize(string json, out SaveData data)
        {
            data = null;
            if (string.IsNullOrWhiteSpace(json)) return false;
            try
            {
                data = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[TRYKLI] Save parse error: " + exception.Message);
                return false;
            }

            if (data == null) return false;
            if (data.version > CurrentVersion)
            {
                Debug.LogWarning($"[TRYKLI] Save version {data.version} is newer than supported ({CurrentVersion}).");
            }

            data = SaveMigrator.Migrate(data);
            return true;
        }
    }
}
