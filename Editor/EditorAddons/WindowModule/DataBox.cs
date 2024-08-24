using System;
using Better.Attributes.EditorAddons.Drawers.Validation.Extensions;
using Better.Commons.EditorAddons.Enums;
using Better.Commons.EditorAddons.Extensions;
using Better.Commons.EditorAddons.Utility;
using Better.Commons.Runtime.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public class DataBox : Box
    {
        public ValidationCommandData Data { get; }
        private readonly Image _boxIcon;
        private const string ScriptIconName = "cs Script Icon";
        public event Action<DataBox> Selected;

        public DataBox(ValidationCommandData data)
        {
            Data = data;
            style.FlexDirection(FlexDirection.Column)
                .Width(new StyleLength(new Length(100, LengthUnit.Percent)));

            var scriptLabel = CreateScriptLabel(data);
            Add(scriptLabel);
            var helpBox = new Box();
            _boxIcon = helpBox.AddIcon(null);
            _boxIcon.style
                .Height(32)
                .Width(32);
            helpBox.AddToClassList(HelpBox.ussClassName);
            Add(helpBox);

            var helpBoxLabel = new Label(data.Result);
            helpBoxLabel.AddToClassList(HelpBox.labelUssClassName);
            helpBox.Add(helpBoxLabel);
        }

        private VisualElement CreateScriptLabel(ValidationCommandData data)
        {
            var horizontalGroup = VisualElementUtility.CreateHorizontalGroup();
            horizontalGroup.style.FlexGrow(StyleDefinition.OneStyleFloat);
            var reference = data.Target;

            var labelGroup = VisualElementUtility.CreateHorizontalGroup();
            horizontalGroup.Add(labelGroup);
            var csIcon = EditorGUIUtility.IconContent(ScriptIconName).image;

            var icon = EditorGUIUtility.GetIconForObject(reference);
            if (icon)
            {
                csIcon = icon;
            }

            var label = VisualElementUtility.CreateLabel(reference.GetType().Name);
            labelGroup.AddIcon(csIcon);
            labelGroup.Add(label);

            var button = new Button()
            {
                text = "Show"
            };
            button.style.MarginLeft(StyleKeyword.Auto);
            button.RegisterCallback<ClickEvent>(OnClick);
            horizontalGroup.Add(button);

            return horizontalGroup;
        }

        private void OnClick(ClickEvent evt)
        {
            Selected?.Invoke(this);
        }

        public void UpdateStyle(ValidationCommandData data)
        {
            if (Data == data)
            {
                style.backgroundColor = new StyleColor(new Color(1f, 0.92f, 0.02f, 0.4f));
            }
            else
            {
                style.backgroundColor = new StyleColor(Color.clear);
            }

            var iconType = Data.Type.GetIconType();
            if (Data.IsValid)
            {
                style.backgroundColor = new StyleColor(new Color(0f, 1f, 0f, 0.4f));
                iconType = IconType.Checkmark;
            }

            _boxIcon.image = iconType.GetIcon();
        }
    }
}