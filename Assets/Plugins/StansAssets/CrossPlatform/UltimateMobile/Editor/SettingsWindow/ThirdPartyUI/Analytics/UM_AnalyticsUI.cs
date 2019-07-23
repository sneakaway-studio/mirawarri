using UnityEngine;
using UnityEditor;

using SA.Android;
using SA.Foundation.Editor;

namespace SA.CrossPlatform
{
    public class UM_AnalyticsUI : UM_PluginSettingsUI
    {
        private UM_AnalyticsResolver m_ServiceResolver;

        private UM_AdvertisementPlatfromUI m_UnityBlock;
        private UM_AdvertisementPlatfromUI m_FacebookBlock;
        private UM_AdvertisementPlatfromUI m_FirebaseBlock;

        private const int k_ToggleWidth = 150;

        public override void OnAwake() 
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/ultimate-mobile-pro/getting-started-807");
            AddFeatureUrl("Analytics API", "https://unionassets.com/ultimate-mobile-pro/analytics-api-808");
            AddFeatureUrl("Automatic Tracking", "https://unionassets.com/ultimate-mobile-pro/automatic-tracking-809");

            AddFeatureUrl("Unity Analytics", "https://unionassets.com/ultimate-mobile-pro/unity-analytics-810");
            AddFeatureUrl("Google Analytics", "https://unionassets.com/ultimate-mobile-pro/google-analytics-811");
            AddFeatureUrl("Facebook Analytics ", "https://unionassets.com/ultimate-mobile-pro/facebook-analytics-812");
        }


        public override void OnLayoutEnable() {

            base.OnLayoutEnable();

            m_UnityBlock = new UM_AdvertisementPlatfromUI("Unity Analytics", "unity_icon.png", new UM_AnalyticsResolver(), () => {
                UM_UnityAnalyticsUI.OnGUI();
            });

            m_FirebaseBlock = new UM_AdvertisementPlatfromUI("Firebase Analytics", "firebase_icon.png", AN_Preprocessor.GetResolver<AN_FirebaseResolver>(), () => {
                UM_FirebaseAnalyticsUI.OnGUI();
            });

           
            m_FacebookBlock = new UM_AdvertisementPlatfromUI("Facebook Analytics", "facebook_icon.png", new UM_FacebookResolver(), () => {
                EditorGUILayout.HelpBox("No additiona settings required.", MessageType.Info);
            });
        }



        public override string Title {
            get {
                return "Analytics";
            }
        }

        public override string Description {
            get {
                return "Service allows you to submit an analytics data " +
                    "to the different analytics services using one unified API.";
            }
        }

        protected override Texture2D Icon {
            get {
                return UM_Skin.GetServiceIcon("um_analytics_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver 
        {
            get {
                if (m_ServiceResolver == null) 
                {
                    m_ServiceResolver = new UM_AnalyticsResolver();
                }

                return m_ServiceResolver;
            }
        }

        protected override void OnServiceUI() 
        {
            m_UnityBlock.OnGUI();
            m_FirebaseBlock.OnGUI();
            m_FacebookBlock.OnGUI();
            
            AutomationUI();
        }

        private void AutomationUI() 
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Automation"))) 
            {
                EditorGUILayout.HelpBox("Analytics service can automate some analytics event propagation " +
                    "based on using Ultimate Mobile & Unity API.", MessageType.Info);
                var automation = UM_Settings.Instance.Analytics.Automation;
                using (new SA_H2WindowBlockWithSpace(new GUIContent("GENERAL"))) {
                    using (new SA_GuiBeginHorizontal()) {
                        automation.Exceptions = GUILayout.Toggle(automation.Exceptions, " Exceptions", GUILayout.Width(k_ToggleWidth));
                    }
                }

                using (new SA_H2WindowBlockWithSpace(new GUIContent("GAME SERVICES"))) 
                {
                    using (new SA_GuiBeginHorizontal()) 
                    {
                        automation.Scores = GUILayout.Toggle(automation.Scores, " Scores", GUILayout.Width(k_ToggleWidth));
                        automation.Achievements = GUILayout.Toggle(automation.Achievements, " Achievements", GUILayout.Width(k_ToggleWidth));
                    }

                    using (new SA_GuiBeginHorizontal()) 
                    {
                        automation.GameSaves = GUILayout.Toggle(automation.GameSaves, " GameSaves", GUILayout.Width(k_ToggleWidth));
                        automation.PlayerIdTracking = GUILayout.Toggle(automation.PlayerIdTracking, " Player Id Tracking", GUILayout.Width(k_ToggleWidth));
                    }
                }

                using (new SA_H2WindowBlockWithSpace(new GUIContent("IN-APP PURCHASING"))) 
                {
                    using (new SA_GuiBeginHorizontal()) 
                    {
                        automation.SuccessfulTransactions = GUILayout.Toggle(automation.SuccessfulTransactions, " Payments", GUILayout.Width(k_ToggleWidth));
                        automation.FailedTransactions = GUILayout.Toggle(automation.FailedTransactions, " Failed Transactions", GUILayout.Width(k_ToggleWidth));
                    }

                    using (new SA_GuiBeginHorizontal()) 
                    {
                        automation.RestoreRequests = GUILayout.Toggle(automation.RestoreRequests, " Restore Requests", GUILayout.Width(k_ToggleWidth));
                    }
                }
            }
        }
    }
}