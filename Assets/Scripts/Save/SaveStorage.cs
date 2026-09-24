using System;
using System.IO;
using UnityEngine;

namespace Trykli.Save
{
    /// <summary>Abstraction of the save medium (local file today, cloud save later).</summary>
    public interface ISaveStorage
    {
        bool TryRead(out string content);
        bool TryReadBackup(out string content);
        void Write(string content);
        void Delete();
    }

    /// <summary>
    /// JSON file in Application.persistentDataPath. Writes go to a temporary file first, then replace the
    /// main file while keeping a backup: a crash during a save never corrupts the progression.
    /// </summary>
    public sealed class FileSaveStorage : ISaveStorage
    {
        public const string DefaultFileName = "trykli_save.json";

        private readonly string _path;

        public FileSaveStorage(string directory = null, string fileName = DefaultFileName)
        {
            _path = Path.Combine(directory ?? Application.persistentDataPath, fileName);
        }

        public string FilePath => _path;
        private string TempPath => _path + ".tmp";
        private string BackupPath => _path + ".bak";

        public bool TryRead(out string content) => TryReadFile(_path, out content);

        public bool TryReadBackup(out string content) => TryReadFile(BackupPath, out content);

        public void Write(string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                File.WriteAllText(TempPath, content);
                if (File.Exists(_path))
                {
                    File.Copy(_path, BackupPath, true);
                    File.Delete(_path);
                }

                File.Move(TempPath, _path);
            }
            catch (Exception exception)
            {
                Debug.LogError("[TRYKLI] Save failed: " + exception.Message);
            }
        }

        public void Delete()
        {
            TryDelete(_path);
            TryDelete(BackupPath);
            TryDelete(TempPath);
        }

        private static bool TryReadFile(string path, out string content)
        {
            content = null;
            try
            {
                if (!File.Exists(path)) return false;
                content = File.ReadAllText(path);
                return !string.IsNullOrEmpty(content);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[TRYKLI] Could not read save file: " + exception.Message);
                return false;
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[TRYKLI] Could not delete " + path + ": " + exception.Message);
            }
        }
    }

    /// <summary>In-memory storage used by tests and by the editor tools.</summary>
    public sealed class MemorySaveStorage : ISaveStorage
    {
        public string Content;
        public string Backup;
        public int WriteCount;

        public bool TryRead(out string content)
        {
            content = Content;
            return !string.IsNullOrEmpty(content);
        }

        public bool TryReadBackup(out string content)
        {
            content = Backup;
            return !string.IsNullOrEmpty(content);
        }

        public void Write(string content)
        {
            Backup = Content;
            Content = content;
            WriteCount++;
        }

        public void Delete()
        {
            Content = null;
            Backup = null;
        }
    }
}
