using System.Collections.Generic;
using Better.Internal.Core.Runtime;
using Better.ProjectSettings.EditorAddons;
using UnityEditor;

namespace Better.Validation.EditorAddons.Settings
{
    internal class ValidationSettingsProvider : DefaultProjectSettingsProvider<ValidationSettings>
    {
        public const string Path = PrefixConstants.BetterPrefix + "/" + nameof(Validation);

        public ValidationSettingsProvider() : base(Path)
        {
            keywords = new HashSet<string>(new[] { "Better", "Validation", "Warnings", "Ignore" });
        }

        [MenuItem(Path + "/" + PrefixConstants.HighlightPrefix, false, 999)]
        private static void Highlight()
        {
            SettingsService.OpenProjectSettings(ProjectPath + Path);
        }
    }
}