using UnityEngine;
using UnityEditor;
using SA.Android;
using SA.Foundation.Editor;

namespace SA.CrossPlatform
{
    public static class UM_FirebaseAnalyticsUI
    {
        private static GUIContent s_SessionTimeoutDuration = new GUIContent("Session Timeout Duration");

        public static void OnGUI() 
        {
            if(AN_FirebaseDefinesResolver.IsAnalyticsSDKInstalled) 
            {
                var firebaseClient = UM_Settings.Instance.Analytics.FirebaseClient;
                EditorGUILayout.HelpBox("Controls whether the sending of device stats at runtime is enabled. (seconds)", MessageType.Info);
                firebaseClient.SessionTimeoutDuration = SA_EditorGUILayout.IntField(s_SessionTimeoutDuration, firebaseClient.SessionTimeoutDuration);

            } 
            else 
            {
                AN_FirebaseFeaturesUI.DrawAnalyticsInstalRequired();
            }
        }
    }
}