using System.Collections.Generic;
using UnityEngine;
using SA.CrossPlatform;
using SA.CrossPlatform.Analytics;
using SA.Facebook;
using UnityEngine.UI;

public class UM_AnalyticsUseExample : MonoBehaviour
{

    [SerializeField] private Button m_InitButton = null;
    [SerializeField] private Button m_LogSimpleEventButton = null;
    [SerializeField] private Button m_LogDataEventButton = null;
    [SerializeField] private Button m_LogTransactionButton = null;
    
    void Start()
    {
        //Unity settings via code example
        UM_Settings.Instance.Analytics.UnityClient.LimitUserTracking = true;
        UM_Settings.Instance.Analytics.UnityClient.DeviceStatsEnabled = true;

        //Firebase settings via code
        UM_Settings.Instance.Analytics.FirebaseClient.SessionTimeoutDuration = 30;

        if (SA_FB.IsSDKInstalled)
        {
            SA_FB.Init();
        }

        m_InitButton.onClick.AddListener(() =>
        {
            UM_AnalyticsService.Client.Init();
            
            //Demographics
            UM_AnalyticsService.Client.SetUserId(SystemInfo.deviceUniqueIdentifier);
            UM_AnalyticsService.Client.SetUserGender(UM_Gender.Male);
            UM_AnalyticsService.Client.SetUserBirthYear(1989);
        });
        
        m_LogSimpleEventButton.onClick.AddListener(() =>
        {
            UM_AnalyticsService.Client.Event("my_simple_event");
        });
        
        m_LogDataEventButton.onClick.AddListener(() =>
        {
            var data =  new Dictionary<string, object>();
            data.Add("key_1", 100);
            data.Add("key_2", "Hello World");
            UM_AnalyticsService.Client.Event("my_data_event", data);
        });
        
        m_LogTransactionButton.onClick.AddListener(() =>
        {
            var productId = "my_item";
            float amount = 1;
            var currency = "USD";
            
            UM_AnalyticsService.Client.Transaction(productId, amount, currency);
        });
    }


    private void Update()
    {
        m_InitButton.interactable = !UM_AnalyticsService.Client.IsInitialized;
        
        m_LogSimpleEventButton.interactable = UM_AnalyticsService.Client.IsInitialized;
        m_LogDataEventButton.interactable = UM_AnalyticsService.Client.IsInitialized;
        m_LogTransactionButton.interactable = UM_AnalyticsService.Client.IsInitialized;
    }
}
