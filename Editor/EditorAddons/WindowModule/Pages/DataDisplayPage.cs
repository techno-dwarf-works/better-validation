using System;
using System.Collections.Generic;
using System.Linq;
using Better.Commons.EditorAddons.Utility;
using Better.Commons.Runtime.Extensions;
using Better.Validation.EditorAddons.WindowModule.CollectionDrawing;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public class DataDisplayPage : ValidationWindowPage
    {
        private Vector2 _scrollPosition;
        private const string AllClear = "All clear. There is no failed validation!";

        private int _groupID;

        private CollectionDrawer _currentDrawer;

        private CollectionDrawer[] _drawers;
        private ScrollView _scroll;
        private Label _clearLabel;

        public DataDisplayPage()
        {
            PrefabStage.prefabSaved += OnPrefabSaved;
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        public void SetData(List<ValidationCommandData> data)
        {
            if (_drawers.Length <= 0)
            {
                return;
            }

            var collectionDrawer = _drawers.First();
            collectionDrawer.Initialize(data);
            SetCurrentDrawer(collectionDrawer);
        }

        public override void Initialize()
        {
            _drawers = typeof(CollectionDrawer).GetAllInheritedTypesWithoutUnityObject()
                .Select(type => (CollectionDrawer)Activator.CreateInstance(type))
                .OrderBy(drawer => drawer.Order).ToArray();

            style.FlexDirection(FlexDirection.Column)
                .Height(new Length(100, LengthUnit.Percent));

            CreateElements();
        }

        private void CreateElements()
        {
            if (_drawers.Length <= 0) return;

            var button = new Button(OnBackClicked)
            {
                text = "Go Back"
            };

            Add(button);

            var toolbar = VisualElementUtility.CreateHorizontalGroup();
            toolbar.style.MinHeight(new Length(22f, LengthUnit.Pixel));
            Add(toolbar);

            _scroll = new ScrollView();
            _scroll.style.FlexShrink(StyleDefinition.OneStyleFloat);
            Add(_scroll);

            _clearLabel = new Label(AllClear);
            _clearLabel.style.Padding(new StyleLength(new Length(5, LengthUnit.Pixel)));
            Add(_clearLabel);

            foreach (var drawer in _drawers)
            {
                _scroll.Add(drawer);
                var toolbarButton = new Button()
                {
                    text = drawer.GetOptionName(),
                    userData = drawer
                };
                toolbarButton.RegisterCallback<ClickEvent, CollectionDrawer>(OnSelected, drawer);
                toolbarButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
                toolbar.Add(toolbarButton);
            }

            var verticalGroup = VisualElementUtility.CreateVerticalGroup();
            verticalGroup.style
                .MarginTop(new StyleLength(StyleKeyword.Auto))
                .MinHeight(new Length(44f, LengthUnit.Pixel));

            Add(verticalGroup);
            var horizontalGroup = VisualElementUtility.CreateHorizontalGroup();
            horizontalGroup.style
                .MarginBottom(new StyleLength(0f))
                .MarginTop(new StyleLength(StyleKeyword.Auto))
                .MinHeight(StyleDefinition.SingleLineHeight)
                .FlexGrow(StyleDefinition.OneStyleFloat);

            verticalGroup.Add(horizontalGroup);

            var prevButton = new Button(PreviousClicked)
            {
                text = "Previous",
                name = "Previous"
            };
            prevButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
            horizontalGroup.Add(prevButton);

            var nextButton = new Button(NextClicked)
            {
                text = "Next",
                name = "Next"
            };
            nextButton.style.FlexGrow(StyleDefinition.OneStyleFloat);
            horizontalGroup.Add(nextButton);

            var clearButton = new Button(ClearResolved)
            {
                text = "Clear Resolved",
                name = "Clear Resolved"
            };

            clearButton.style.FlexGrow(StyleDefinition.OneStyleFloat);

            verticalGroup.Add(clearButton);
        }

        private void ClearResolved()
        {
            _currentDrawer.ClearResolved();
        }

        private void PreviousClicked()
        {
            var data = _currentDrawer.GetPrevious();
            _currentDrawer.UpdateCurrent(data);
            SelectionUtility.OpenReference(data.Target);
        }

        private void NextClicked()
        {
            var data = _currentDrawer.GetNext();
            _currentDrawer.UpdateCurrent(data);
            SelectionUtility.OpenReference(data.Target);
        }

        private void OnSelected(ClickEvent clickEvent, CollectionDrawer drawer)
        {
            SetCurrentDrawer(drawer);
        }

        private void SetCurrentDrawer(CollectionDrawer newDrawer)
        {
            if (_currentDrawer != null)
            {
                newDrawer.Copy(_currentDrawer);
            }

            _currentDrawer = newDrawer;

            foreach (var drawer in _drawers)
            {
                drawer.style.SetVisible(false);
            }

            _currentDrawer.style.SetVisible(true);

            _clearLabel.style.SetVisible(_currentDrawer.Count <= 0);
        }

        private void OnBackClicked()
        {
            OpenPage(new ValidationTabsPage());
        }

        public override void Deconstruct()
        {
            PrefabStage.prefabSaved -= OnPrefabSaved;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
        }

        private void OnSceneSaved(Scene scene)
        {
            _currentDrawer.Revalidate();
        }

        private void OnPrefabSaved(GameObject obj)
        {
            _currentDrawer.Revalidate();
        }
    }
}