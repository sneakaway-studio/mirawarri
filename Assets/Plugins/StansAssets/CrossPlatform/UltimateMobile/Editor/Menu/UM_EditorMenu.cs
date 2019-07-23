using UnityEngine;
using UnityEditor;
using SA.Foundation.Config;


namespace SA.CrossPlatform
{
    public class UM_EditorMenu 
    {
        // WARNING: same menu item path is duplicated for settings UI.
        // if you need to change it here, make a proper config first.
        // do not change MenuItem path before you 100% what is mean by a statement above.

        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "Cross-Platform/Services", false, 310)]
        public static void Services() 
        {
            var window = UM_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(0);
        }

        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "Cross-Platform/3rd-Party", false, 311)]
        public static void ThirdParty() 
        {
            var window = UM_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(1);
        }

        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "Cross-Platform/Summary", false, 312)]
        public static void Summary() 
        {
            var window = UM_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(2);
        }
        
        public static void About() 
        {
            var window = UM_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(3);
        }

        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "Cross-Platform/Documentation", false, 313)]
        public static void Documentation() 
        {
            Application.OpenURL(UM_Settings.DOCUMENTATION_URL);
        }

        private static GUIContent WindowTitle {
            get {
                return new GUIContent("Ultimate", UM_Skin.SettingsWindowIcon);
            }
        }

    }
}

