using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SA.CrossPlatform.Samples
{
    [InitializeOnLoad]
    public class UM_WelcomeWidgetHook : MonoBehaviour
    {
        private static bool s_IsActive = false;
        private static Dictionary<SceneView, UM_WelcomeWidget> s_Widgets = new Dictionary<SceneView, UM_WelcomeWidget>();

        static UM_WelcomeWidgetHook() 
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
           SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif

            UM_WelcomeController.OnWelcomeControllerAwake += () => { s_IsActive = true; };
            UM_WelcomeController.OnWelcomeControllerDestroy += () => { s_IsActive = false; };

        }

        public static void Restart() 
        {
            s_Widgets.Clear();
        }


        private static void OnSceneGUI(SceneView sceneView) 
        {

            if (Application.isPlaying || !s_IsActive)
            {
                return;
            }

            UM_WelcomeWidget widget;
            if (s_Widgets.ContainsKey(sceneView)) 
            {
                widget = s_Widgets[sceneView];
            } 
            else 
            {
                widget = new UM_WelcomeWidget(sceneView);
                s_Widgets.Add(sceneView, widget);
            }
            widget.OnGUI();
        }
    }
}