using System.Collections.Generic;
using Better.Internal.Core.Runtime;
using Better.ProjectSettings.EditorAddons;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.Settings
{
    internal class ValidationSettingsProvider : ProjectSettingsProvider<ValidationSettings>
    {
        private readonly Editor _editor;
        public const string Path = PrefixConstants.BetterPrefix + "/" + nameof(Validation);

        public ValidationSettingsProvider() : base(Path)
        {
            keywords = new HashSet<string>(new[] { "Better", "Validation", "Warnings", "Ignore" });
            _editor = Editor.CreateEditor(_settings);
        }

        [MenuItem(Path + "/" + PrefixConstants.HighlightPrefix, false, 999)]
        private static void Highlight()
        {
            SettingsService.OpenProjectSettings(ProjectPath + Path);
        }

        //TODO: maybe change to new InspectorElement(_editor); for DefaultProjectSettingsProvider<T>
        protected override void CreateVisualElements(VisualElement rootElement)
        {
            var inspectorElement = new InspectorElement(_editor);
            rootElement.Add(inspectorElement);
        }
    }
}