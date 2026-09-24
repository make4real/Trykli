using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Localization
{
    /// <summary>
    /// Centralized localization. Every visible text goes through <see cref="Get"/>.
    /// Languages are JSON files in Resources/Localization; adding a file adds a language
    /// (no scene or code change required).
    /// </summary>
    public static class Loc
    {
        public const string ResourceFolder = "Localization";
        public const string FallbackLanguage = "en";

        public readonly struct LanguageInfo
        {
            public readonly string Code;
            public readonly string DisplayName;

            public LanguageInfo(string code, string displayName)
            {
                Code = code;
                DisplayName = displayName;
            }
        }

        private static readonly Dictionary<string, LocalizationTable> Tables = new Dictionary<string, LocalizationTable>();
        private static readonly Dictionary<string, Dictionary<string, string>> Lookups = new Dictionary<string, Dictionary<string, string>>();
        private static bool _resourcesLoaded;

        public static string CurrentLanguage { get; private set; } = "fr";

        public static event Action LanguageChanged;

        public static IReadOnlyList<LanguageInfo> AvailableLanguages
        {
            get
            {
                EnsureResourcesLoaded();
                var list = new List<LanguageInfo>();
                foreach (LocalizationTable table in Tables.Values) list.Add(new LanguageInfo(table.code, table.displayName));
                list.Sort((a, b) => string.CompareOrdinal(a.Code, b.Code));
                return list;
            }
        }

        /// <summary>Loads every language file from Resources and selects <paramref name="languageCode"/>.</summary>
        public static void Initialize(string languageCode)
        {
            EnsureResourcesLoaded();
            SetLanguage(languageCode);
        }

        public static void SetLanguage(string languageCode)
        {
            EnsureResourcesLoaded();
            if (string.IsNullOrEmpty(languageCode) || !Lookups.ContainsKey(languageCode))
            {
                languageCode = Lookups.ContainsKey(FallbackLanguage) ? FallbackLanguage : CurrentLanguage;
            }

            bool changed = CurrentLanguage != languageCode;
            CurrentLanguage = languageCode;
            if (changed) LanguageChanged?.Invoke();
        }

        /// <summary>Registers a table from JSON (used by tests and tools).</summary>
        public static bool LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;
            LocalizationTable table;
            try
            {
                table = JsonUtility.FromJson<LocalizationTable>(json);
            }
            catch (Exception exception)
            {
                Debug.LogError("[TRYKLI] Invalid localization file: " + exception.Message);
                return false;
            }

            if (table == null || string.IsNullOrEmpty(table.code)) return false;
            Register(table);
            return true;
        }

        public static void Register(LocalizationTable table)
        {
            Tables[table.code] = table;
            Lookups[table.code] = table.ToDictionary();
        }

        public static bool Has(string key)
        {
            return !string.IsNullOrEmpty(key) && TryGet(CurrentLanguage, key, out _);
        }

        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            EnsureResourcesLoaded();
            if (TryGet(CurrentLanguage, key, out string value)) return value;
            if (TryGet(FallbackLanguage, key, out value)) return value;
            return key;
        }

        public static string Format(string key, params object[] args)
        {
            string pattern = Get(key);
            try
            {
                return string.Format(pattern, args);
            }
            catch (FormatException)
            {
                return pattern;
            }
        }

        /// <summary>Picks a language code matching the device language.</summary>
        public static string DetectSystemLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.French: return "fr";
                case SystemLanguage.English: return "en";
                default:
                    EnsureResourcesLoaded();
                    string name = Application.systemLanguage.ToString().ToLowerInvariant();
                    foreach (LocalizationTable table in Tables.Values)
                    {
                        if (!string.IsNullOrEmpty(table.displayName) && table.displayName.ToLowerInvariant() == name) return table.code;
                    }

                    return FallbackLanguage;
            }
        }

        public static IEnumerable<string> KeysOf(string languageCode)
        {
            EnsureResourcesLoaded();
            if (Lookups.TryGetValue(languageCode, out Dictionary<string, string> lookup)) return lookup.Keys;
            return Array.Empty<string>();
        }

        private static bool TryGet(string language, string key, out string value)
        {
            value = null;
            return Lookups.TryGetValue(language, out Dictionary<string, string> lookup) && lookup.TryGetValue(key, out value);
        }

        private static void EnsureResourcesLoaded()
        {
            if (_resourcesLoaded) return;
            _resourcesLoaded = true;
            TextAsset[] files;
            try
            {
                files = Resources.LoadAll<TextAsset>(ResourceFolder);
            }
            catch (Exception)
            {
                return; // Outside of Unity (pure unit tests) there is no Resources folder.
            }

            foreach (TextAsset file in files) LoadFromJson(file.text);
        }
    }
}
