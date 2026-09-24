using System.Collections.Generic;
using Trykli.Core;
using Trykli.Localization;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.UI
{
    public sealed class CreditsPanel : ModalPanel
    {
        protected override bool CloseOnDimmerTap => true;

        protected override void Build()
        {
            AddTitle("credits.title");
            AddText("credits.body", UITheme.BodySize);
            AddRawText(AppInfo.VersionLabel, UITheme.SmallSize, UITheme.TextMuted);
            AddButton("common.close", UITheme.Primary, Close);
        }
    }
}
