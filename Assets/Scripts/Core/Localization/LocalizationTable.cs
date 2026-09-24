using System;
using System.Collections.Generic;

namespace Trykli.Localization
{
    /// <summary>JSON model of a language file stored in Resources/Localization/&lt;code&gt;.json.</summary>
    [Serializable]
    public class LocalizationTable
    {
        [Serializable]
        public class Entry
        {
            public string key = "";
            public string value = "";
        }

        public string code = "";
        public string displayName = "";
        public List<Entry> entries = new List<Entry>();

        public Dictionary<string, string> ToDictionary()
        {
            var dictionary = new Dictionary<string, string>(entries.Count);
            foreach (Entry entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.key)) dictionary[entry.key] = entry.value ?? string.Empty;
            }

            return dictionary;
        }
    }
}
