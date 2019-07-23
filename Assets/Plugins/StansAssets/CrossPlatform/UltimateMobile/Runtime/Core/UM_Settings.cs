using System.Collections.Generic;

using SA.CrossPlatform.App;
using SA.CrossPlatform.Analytics;
using SA.CrossPlatform.GameServices;

using SA.Foundation.Config;
using SA.Foundation.Patterns;

namespace SA.CrossPlatform {
    public class UM_Settings : SA_ScriptableSingleton<UM_Settings> {
        public const string PLUGIN_NAME = "Ultimate Mobile";
        public const string DOCUMENTATION_URL = "https://unionassets.com/ultimate-mobile-pro/manual";

        public const string PLUGIN_FOLDER = SA_Config.STANS_ASSETS_CROSS_PLATFORM_PLUGINS_PATH + "UltimateMobile/";
        
        //--------------------------------------
        // Example Scenes
        //--------------------------------------
        
        private const string k_ExamplesPath = PLUGIN_FOLDER + "Samples/";
        public const string k_WelcomeSamplesScenePath = k_ExamplesPath + "WelcomePage/UM_Welcome.unity"; 
        public const string k_InAppSamplesScenePath = k_ExamplesPath + "InAppSample/UM_InAppExample.unity"; 
        public const string k_GameServicesSamplesScenePath = k_ExamplesPath + "GameServiceSample/UM_GameService.unity"; 
        public const string k_SharingSamplesScenePath = k_ExamplesPath + "SoicalSample/UM_SharingExample.unity"; 
        public const string k_NotificationsSamplesScenePath = k_ExamplesPath + "LocalNotificationsSample/UM_LocalNotifications.unity"; 
        public const string k_CameraAndGallerySamplesScenePath = k_ExamplesPath + "CameraAndGallerySample/UM_CameraAndGalleryExample.unity"; 
        public const string k_MedialPlayerSamplesScenePath = k_ExamplesPath + "MedialPlayerSample/UM_MedialPlayerExample.unity"; 
        public const string k_FirebaseSamplesScenePath = k_ExamplesPath + "FirebaseSample/UM_FirebaseSample.unity"; 
        public const string k_GIFSamplesScenePath = k_ExamplesPath + "GIFAPISamples/UM_GIF_Samples.unity"; 
        public const string k_FBSamplesScenePath = k_ExamplesPath + "FacebookSamples/UM_FB_Samples.unity"; 
        
        //--------------------------------------
        // Game Service API
        //--------------------------------------
       
        public bool AndroidRequestProfile = true;
        public bool AndroidRequestEmail = true;

        public bool AndroidSavedGamesEnabled;
        public bool AndroidRequestServerAuthCode;
        public string AndroidGMSServerId = string.Empty;


        //--------------------------------------
        // Game Service Editor API Emulation
        //--------------------------------------

        //this is actually has to be moved to the editor only serialized setting.
        public UM_EditorPlayer GSEditorPlayer = new UM_EditorPlayer();
        public List<UM_LeaderboardMeta> GSLeaderboards = new List<UM_LeaderboardMeta>();


        //--------------------------------------
        // Vending Service Editor API Emulation
        //--------------------------------------

        public List<string> TestRestoreProducts = new List<string>();
        


        //--------------------------------------
        // Contacts API
        //--------------------------------------


        public List<UM_EditorContact> EditorTestingContacts = new List<UM_EditorContact>();


        //--------------------------------------
        // Analytics API
        //--------------------------------------


        public UM_AnalyticsSettings Analytics = new UM_AnalyticsSettings();





        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------


        protected override string BasePath {
            get { return PLUGIN_FOLDER; }
        }


        public override string PluginName {
            get {
                return PLUGIN_NAME;
            }
        }

        public override string DocumentationURL {
            get {
                return DOCUMENTATION_URL;
            }
        }


        public override string SettingsUIMenuItem {
            get {
                return SA_Config.EDITOR_MENU_ROOT + "Cross-Platform/Services";
            }
        }
    }
}