using Better.Commons.EditorAddons.Utility;
using Better.Commons.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public class ValidateTab : BaseValidationTab
    {
        private const string BottomText = "Validation commands will go through Unity SerializedProperties and run validation attributes";
        private GUIStyle _style;
        private const string Name = "Validate";

        public override int Order => 0;

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
            
            var currentSceneButton = new Button(() => SelectCommands(_commands.ValidateAttributesInCurrentScene())) { text = "In Current Scene" };
            currentSceneButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
            horizontalGroup.Add(currentSceneButton);
            
            var allScenesButton = new Button(() => SelectCommands(_commands.ValidateAttributesInAllScenes())) { text = "In All Scenes" };
            allScenesButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
            horizontalGroup.Add(allScenesButton);
            
            var inProjectButton = new Button(() => SelectCommands(_commands.ValidateAttributesInProject())) { text = "In Project" };
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