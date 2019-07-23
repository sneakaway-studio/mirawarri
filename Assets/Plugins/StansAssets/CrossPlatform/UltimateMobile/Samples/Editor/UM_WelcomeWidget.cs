using SA.Foundation.Editor;
using UnityEditor;
using UnityEngine;

namespace SA.CrossPlatform.Samples
{
    public class UM_WelcomeWidget 
    {
        private const int k_WindowPrefixId = 12516789;
        private const int k_ButtonWidth = 100;
      
        private readonly int m_WindowId;
        private readonly SceneView m_SceneView;
        private Rect m_WindowRect = Rect.zero;
     
        public UM_WelcomeWidget(SceneView sceneView) {
            m_SceneView = sceneView;
            m_WindowId = k_WindowPrefixId + m_SceneView.GetInstanceID();

            m_WindowRect.y = 20;
        }   

        
        public void OnGUI() {
            m_WindowRect = GUILayout.Window(m_WindowId, m_WindowRect, OnWindowGui, "Ultimate Mobile Samples.", GUILayout.ExpandHeight(true));
        }

        private void OnWindowGui(int id) {


           GUILayout.Label("Welcome!", EditorStyles.boldLabel);
           var message = "Welcome to samples application main page. \n" +
                           "This is an early version that is still under development. \n \n" +
                           "You are welcome to try it inside the editor and check out how different features are implemented, " +
                           "or can also build an application on a real device. \n" +
                           "Please note that some section of the sample app may not work if you haven't configured corresponded module.";
           
           var style = new GUIStyle (EditorStyles.helpBox);
           style.richText = true;
           GUILayout.Label(message, style, GUILayout.Width(350));
           
           GUILayout.Label("Run in Editor!", EditorStyles.boldLabel);
           EditorGUILayout.HelpBox("If you want to run a sample app inside an editor, " +
                                   "you need to include all the application scene ot the Project Settings. " +
                                   "However, we do not want to mess with your project configuration. " +
                                   "Use Buttons below to include or exclude scenes automatically.", MessageType.Info);
           using (new SA_GuiBeginHorizontal())
           {
               var include = GUILayout.Button("Include", EditorStyles.miniButton, GUILayout.Width(k_ButtonWidth));
               if (include)
               {
                   UM_SamplesManager.IncludeToBuildSettings();
               }
               
               var exclude = GUILayout.Button("Exclude", EditorStyles.miniButton, GUILayout.Width(k_ButtonWidth));
               if (exclude)
               {
                   UM_SamplesManager.ExcludeFromSettings();
               }
           }
           
           GUILayout.Label("Build On Device!", EditorStyles.boldLabel);
           EditorGUILayout.HelpBox("Only Sample Scenes will be added to the build, " +
                                   "despite Project Settings Scenes configuration", MessageType.Info);
           var build = GUILayout.Button("Build", EditorStyles.miniButton, GUILayout.Width(k_ButtonWidth));
           if (build)
           {
               UM_SamplesManager.BuildWelcomeScene();
           }
           
           GUILayout.Label("More!", EditorStyles.boldLabel);
           using (new SA_GuiBeginHorizontal())
           {
              var settings =  GUILayout.Button("Settings", EditorStyles.miniButton, GUILayout.Width(k_ButtonWidth));
              if (settings)
              {
                  UM_EditorMenu.Services();
              }
              
              var documentation =  GUILayout.Button("Documentation", EditorStyles.miniButton, GUILayout.Width(k_ButtonWidth));
              if (documentation)
              {
                  UM_EditorMenu.Documentation();
              }
              
              var about = GUILayout.Button("About", EditorStyles.miniButton, GUILayout.Width(k_ButtonWidth));
              if (about)
              {
                  UM_EditorMenu.About();
              }
               
           }

            GUI.DragWindow(new Rect(0, 0, m_SceneView.position.width, m_SceneView.position.height));
        }
    }
}