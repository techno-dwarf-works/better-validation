using Better.Commons.EditorAddons.Utility;
using Better.Commons.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public class MissingReferenceTab : BaseValidationTab
    {
        private const string BottomText = "Those commands looking for missing references in Unity SerializedProperties";
        private GUIStyle _style;
        private const string Name = "Find Missing References";

        public override int Order => 1;

        public override string GetTabName()
        {
            return Name;
        }

        public override void Initialize()
        {
            base.Initialize();
            CreateVisualElements();
            style.Width(new StyleLength(new Length(100, LengthUnit.Percent)))
                .Height(new StyleLength(new Length(100, LengthUnit.Percent)));
        }

        private void CreateVisualElements()
        {
            var horizontalGroup = VisualElementUtility.CreateHorizontalGroup();
            Add(horizontalGroup);

            var currentSceneButton = new Button(() => SelectCommands(_commands.FindMissingReferencesInCurrentScene())) { text = "In Current Scene" };
            currentSceneButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
            horizontalGroup.Add(currentSceneButton);

            var allScenesButton = new Button(() => SelectCommands(_commands.FindMissingInAllScenes())) { text = "In All Scenes" };
            allScenesButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
            horizontalGroup.Add(allScenesButton);

            var inProjectButton = new Button(() => SelectCommands(_commands.FindMissingReferencesInProject())) { text = "In Project" };
            inProjectButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
            horizontalGroup.Add(inProjectButton);

            var label = new Label(BottomText);
            label.style
                .WhiteSpace(new StyleEnum<WhiteSpace>(WhiteSpace.Normal))
                .MarginTop(new StyleLength(StyleKeyword.Auto))
                .Padding(new StyleLength(new Length(5, LengthUnit.Pixel)));
            Add(label);
        }
    }
}