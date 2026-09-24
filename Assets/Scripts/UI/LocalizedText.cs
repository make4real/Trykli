using Trykli.Localization;
using TMPro;
using UnityEngine;

namespace Trykli.UI
{
    /// <summary>Keeps a TextMeshPro text in sync with a localization key (updates on language change).</summary>
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key = "";

        private TMP_Text _text;
        private object[] _args;

        public string Key => key;

        public void SetKey(string newKey, params object[] args)
        {
            key = newKey;
            _args = args != null && args.Length > 0 ? args : null;
            Refresh();
        }

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            Loc.LanguageChanged += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            Loc.LanguageChanged -= Refresh;
        }

        public void Refresh()
        {
            if (_text == null) _text = GetComponent<TMP_Text>();
            if (_text == null || string.IsNullOrEmpty(key)) return;
            _text.text = _args != null ? Loc.Format(key, _args) : Loc.Get(key);
        }
    }
}
