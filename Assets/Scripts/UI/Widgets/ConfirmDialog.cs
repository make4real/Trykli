using System;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>Yes / no confirmation (reset progression...).</summary>
    public sealed class ConfirmDialog : ModalPanel
    {
        private string _messageKey;
        private Action _onConfirm;

        public static ConfirmDialog Show(Transform parent, string messageKey, Action onConfirm)
        {
            var dialog = Open<ConfirmDialog>(parent);
            dialog._messageKey = messageKey;
            dialog._onConfirm = onConfirm;
            dialog.GetComponentInChildren<LocalizedText>().SetKey(messageKey);
            return dialog;
        }

        protected override void Build()
        {
            AddText("common.confirm", UITheme.SubheadingSize);
            RectTransform row = AddRow(UITheme.ButtonHeight);
            AddButton("common.no", UITheme.Neutral, Close, null, row);
            AddButton("common.yes", UITheme.Danger, () =>
            {
                _onConfirm?.Invoke();
                Close();
            }, null, row);
        }
    }
}
