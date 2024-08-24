using System;
using System.Collections.Generic;
using System.Linq;
using Better.Commons.EditorAddons.Utility;
using Better.Commons.Runtime.Extensions;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public class ValidationTabsPage : ValidationWindowPage
    {
        private BaseValidationTab[] _tabs;

        public override void Initialize()
        {
            _tabs = typeof(BaseValidationTab).GetAllInheritedTypesWithoutUnityObject()
                .Select(type => (BaseValidationTab)Activator.CreateInstance(type)).OrderBy(tab => tab.Order).ToArray();

            foreach (var validationButton in _tabs)
            {
                validationButton.Initialize();
            }

            style.Height(new Length(100, LengthUnit.Percent));
            CreateElements();
        }

        private void CreateElements()
        {
            if (_tabs.Length <= 0) return;

            var horizontalGroup = VisualElementUtility.CreateHorizontalGroup();
            horizontalGroup.style.Height(new Length(100, LengthUnit.Percent));
            Add(horizontalGroup);

            var toolbar = new Toolbar();
            toolbar.style
                .FlexDirection(FlexDirection.Column)
                .MinWidth(new Length(30, LengthUnit.Percent))
                .Height(StyleKeyword.Auto)
                .BorderRightWidth(StyleDefinition.OneStyleFloat);
            
            horizontalGroup.Add(toolbar);

            foreach (var validationTab in _tabs)
            {
                var button = new Button()
                {
                    text = validationTab.GetTabName()
                };
                button.style.TextOverflow(new StyleEnum<TextOverflow>(TextOverflow.Ellipsis));

                toolbar.Add(button);

                validationTab.CommandSelected += OpenResultPage;
                button.RegisterCallback<ClickEvent, BaseValidationTab>(OnTabSelected, validationTab);

                horizontalGroup.Add(validationTab);
            }

            if (_tabs.Length > 0)
            {
                OpenTab(_tabs.FirstOrDefault());
            }
        }

        private void OnTabSelected(ClickEvent clickEvent, BaseValidationTab tab)
        {
            OpenTab(tab);
        }

        private void OpenTab(BaseValidationTab tab)
        {
            foreach (var validationTab in _tabs)
            {
                validationTab.style.SetVisible(false);
            }

            tab.style.SetVisible(true);
        }

        private void OpenResultPage(List<ValidationCommandData> list)
        {
            if (list != null)
            {
                var page = new DataDisplayPage();
                OpenPage(page);
                page.SetData(list);
            }
        }

        public override void Deconstruct()
        {
        }
    }
}