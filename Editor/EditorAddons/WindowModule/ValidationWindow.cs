using System.Collections.Generic;
using Better.Commons.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace Better.Validation.EditorAddons.WindowModule
{
    public class ValidationWindow : EditorWindow
    {
        private const string WindowIcon = "d_console.erroricon.inactive.sml";

        private ValidationWindowPage _currentPage;

        public static void OpenWindow(List<ValidationCommandData> dataList)
        {
            var window = GetWindow<ValidationWindow>(false, nameof(Validation).PrettyCamelCase());
            var content = EditorGUIUtility.IconContent(WindowIcon);
            content.text = nameof(Validation).PrettyCamelCase();
            window.titleContent = content;
            window.Show();
            window.Display(dataList);

            window.minSize = new Vector2(200, 400);
        }

        private void Display(List<ValidationCommandData> dataList)
        {
            var page = new DataDisplayPage();
            OpenPage(page);
            page.SetData(dataList);
        }

        public static void OpenWindow()
        {
            var window = GetWindow<ValidationWindow>(false, nameof(Validation).PrettyCamelCase());
            var content = EditorGUIUtility.IconContent(WindowIcon);
            content.text = nameof(Validation).PrettyCamelCase();
            window.titleContent = content;
            window.Show();
            window.minSize = new Vector2(200, 400);
        }

        private void OnDestroy()
        {
            _currentPage?.Deconstruct();
        }

        private void CreateGUI()
        {
            OpenPage(new ValidationTabsPage());
        }

        private void OpenPage(ValidationWindowPage page)
        {
            if (_currentPage != null)
            {
                _currentPage.Deconstruct();
                _currentPage.PageOpenRequest -= OpenPage;
                _currentPage.RemoveFromHierarchy();
            }
            
            _currentPage = page;
            _currentPage.Initialize();
            _currentPage.PageOpenRequest += OpenPage;
            rootVisualElement.Add(_currentPage);
        }
    }
}