using SA.CrossPlatform.Media;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.Firebase.Analytics;

public class UM_FirebaseSample : MonoBehaviour
{
 
   [SerializeField] Button m_LogEvent = null;

   void Awake()
   {
      m_LogEvent.onClick.AddListener(() =>
      {
         AN_FirebaseAnalytics.LogEvent("sample_event_ultimate_mobile");
      });
   }
   
}
