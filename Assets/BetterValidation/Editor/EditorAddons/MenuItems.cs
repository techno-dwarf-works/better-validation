using Better.Validation.EditorAddons.Settings;
using Better.Validation.EditorAddons.WindowModule;
using UnityEditor;

namespace Better.Validation.EditorAddons
{
    public static class MenuItems
    {
        [MenuItem(ValidationSettingsProvider.Path + "/Open Validation Window", false, 50)]
        private static void ValidateInProject()
        {
            ValidationWindow.OpenWindow();
        }
    }
}