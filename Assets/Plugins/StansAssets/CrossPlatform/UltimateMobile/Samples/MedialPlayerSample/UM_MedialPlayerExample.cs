using SA.CrossPlatform.Media;
using UnityEngine;
using UnityEngine.UI;

public class UM_MedialPlayerExample : MonoBehaviour
{
   [SerializeField] Button m_PlayButton = null;

   void Awake()
   {
      m_PlayButton.onClick.AddListener(() =>
      {
         //var movieURL = "https://videocdn.bodybuilding.com/video/mp4/62000/62792m.mp4";
         var movieURL = "http://techslides.com/demos/sample-videos/small.mp4";
         UM_MediaPlayer.ShowRemoteVideo(movieURL, () => {
            Debug.Log("Video closed");
         });
      });
   }
}
