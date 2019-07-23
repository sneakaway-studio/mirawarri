using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SA.CrossPlatform.Samples
{
    [ExecuteInEditMode]
    public class UM_WelcomeController : MonoBehaviour
    {
        public static event Action OnWelcomeControllerAwake = delegate {  };
        public static event Action OnWelcomeControllerDestroy = delegate {  };
        
        [SerializeField] private GameObject m_ButtonsPanel = null;
        [SerializeField] private GameObject m_FeatureViewport = null;

        private Scene m_currentlyFeaturedScene;

        private void OnEnable() {
            OnWelcomeControllerAwake.Invoke();
        }

        private void OnDestroy()
        {
            OnWelcomeControllerDestroy.Invoke();
        }

        private void Start()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            var buttons = m_ButtonsPanel.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                var currentButton = button;
                button.onClick.AddListener(() =>
                {
                    var sceneLink = currentButton.GetComponent<UM_SceneLink>();
                    if (sceneLink != null)
                    {
                        LoadScene(sceneLink.SceneName);
                    }
                });
            }

            InitMainScreenServices();
        }

        private void InitMainScreenServices()
        {
            UM_LocalNotificationsExample.SubscribeToTheNotificationEvents();
        }

        private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            m_currentlyFeaturedScene = scene;
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                if (rootGameObject.GetComponent<Canvas>() == null)
                {
                    Destroy(rootGameObject);
                }
                else
                {
                    var canvasRect = rootGameObject.GetComponent<RectTransform>();
                    rootGameObject.transform.SetParent(m_FeatureViewport.transform);
                    canvasRect.anchorMin = new Vector2(0, 0);
                    canvasRect.anchorMax = new Vector2(1, 1);

                    canvasRect.transform.localScale = Vector3.one;
                    canvasRect.anchoredPosition = Vector2.zero;

                    canvasRect.offsetMin = Vector2.zero;
                    canvasRect.offsetMax = Vector2.zero;

                }
            }
        }

        private void LoadScene(string sceneName)
        {
            if (sceneName.Equals(m_currentlyFeaturedScene.name))
            {
                return;
            }

            m_FeatureViewport.Clear();
            if (m_currentlyFeaturedScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(m_currentlyFeaturedScene);
            }

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
