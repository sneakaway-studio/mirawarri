using System;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.App;
using SA.CrossPlatform;
using SA.CrossPlatform.App;
using SA.CrossPlatform.UI;
using SA.CrossPlatform.Notifications;
using SA.Foundation.Utility;
using SA.iOS.UIKit;

public class UM_LocalNotificationsExample : MonoBehaviour
{
    [Header("Unified API Buttons")] 
    [SerializeField] private Button m_Create5SecNotification = null;
    [SerializeField] private Button m_Create20SecNotification = null;
    [SerializeField] private Button m_RemoveDelivered = null;
    [SerializeField] private Button m_RemovePending = null;
    
    [Header("Debug Actions")] 
    [SerializeField] private Button m_CloseApp = null;
    [SerializeField] private Button m_ToBackground = null;
    
    [Header("Android Only")] 
    [SerializeField] private Button m_LargeImageStyle = null;
    [SerializeField] private Button m_LargeTextStyle = null;
    
    [Header("iOS Only")] 
    [SerializeField] private Button m_RegisterForRemoteNotifications = null;
    private void Start()
    {
        var client = UM_NotificationCenter.Client;
        m_Create5SecNotification.onClick.AddListener(() => { CreateNotificationWithInterval(5); });
        m_Create20SecNotification.onClick.AddListener(() => { CreateNotificationWithInterval(20); });

        m_RemoveDelivered.onClick.AddListener(() =>
        {
            client.RemoveAllDeliveredNotifications();
        });
        
        m_RemovePending.onClick.AddListener(() =>
        {
            client.RemoveAllPendingNotifications();
        });
        
        m_CloseApp.onClick.AddListener(Application.Quit);
        
        m_ToBackground.onClick.AddListener(() => { UM_Application.SendToBackground(); });
        
        iOSOnlySetup();
        AndroidOnlySetup();
    }

    /// Uncomment if you wan to test Notification scheduling on application pause 
    /*
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            CreateNotificationWithInterval(5);
        }
    }*/


    private void AndroidOnlySetup()
    {
        m_LargeImageStyle.onClick.AddListener(AndroidBigPictureStyle);
        m_LargeTextStyle.onClick.AddListener(AndroidBigTextStyle);
    }

    private void iOSOnlySetup()
    {
        m_RegisterForRemoteNotifications.onClick.AddListener(ISN_UIApplication.RegisterForRemoteNotifications);
        ISN_UIApplication.ApplicationDelegate.DidRegisterForRemoteNotifications.AddListener((result) => {
            if(result.IsSucceeded) {
                var token = result.DeviceTokenUTF8;
                UM_DialogsUtility.ShowMessage("Register For Remote Notifications", "ANS token string:" + token);
                Debug.Log("ANS token string:" + token);
            } else {
                UM_DialogsUtility.ShowMessage("Register For Remote Notifications", "Error: " + result.Error.Message);
                Debug.Log("Error: " + result.Error.Message);
            }
        });
    }
    

    private void CreateNotificationWithInterval(int delay)
    {
        var client = UM_NotificationCenter.Client;
        var content = new UM_Notification();
            
        content.SetTitle("Title X");
        content.SetBody("Body message X");
        content.SetSoundName(UM_SamplesConfig.NotificationSoundSampleFile);
        content.SetSmallIconName(UM_SamplesConfig.NotificationIconSampleFile);

        var requestId = SA_IdFactory.NextId;
        //2 seconds
        var trigger = new UM_TimeIntervalNotificationTrigger(delay);
        var request = new UM_NotificationRequest(requestId, content, trigger);
        client.AddNotificationRequest(request, (result) => {
            if(result.IsSucceeded) {
                UM_DialogsUtility.ShowMessage("Succeeded", "Notification was successfully scheduled.");
            } else {
                UM_DialogsUtility.ShowMessage("Failed", result.Error.FullMessage);
            }
        });
    }

    private void AndroidBigPictureStyle()
    {
        SA_ScreenUtil.TakeScreenshot(256, (screenshot) => {
            var builder = new AN_NotificationCompat.Builder();
            builder.SetContentText("Big Picture Style");
            builder.SetContentTitle("Big Picture Style title");
            var bigPictureStyle = new AN_NotificationCompat.BigPictureStyle();

     
            bigPictureStyle.BigPicture(screenshot);
            bigPictureStyle.BigLargeIcon(screenshot);
            builder.SetStyle(bigPictureStyle);
            builder.SetDefaults(AN_Notification.DEFAULT_ALL);

            var trigger = new AN_AlarmNotificationTrigger();
            trigger.SetDate(TimeSpan.FromSeconds(1));

            var id = SA_IdFactory.NextId;
            var request = new AN_NotificationRequest(id, builder, trigger);
            AN_NotificationManager.Schedule(request);
        });
    }
    
    private void AndroidBigTextStyle()
    {
        SA_ScreenUtil.TakeScreenshot(256, (screenshot) => {
            var builder = new AN_NotificationCompat.Builder();
            builder.SetContentText("Big Text Style");
            builder.SetContentTitle("Big TextStyle Title");

            var bigTextStyle = new AN_NotificationCompat.BigTextStyle();
            bigTextStyle.BigText("This is test big text style");
            builder.SetStyle(bigTextStyle);
            builder.SetDefaults(AN_Notification.DEFAULT_ALL);

            var trigger = new AN_AlarmNotificationTrigger();
            trigger.SetDate(TimeSpan.FromSeconds(1));

            var id = SA_IdFactory.NextId;
            var request = new AN_NotificationRequest(id, builder, trigger);
            AN_NotificationManager.Schedule(request);
        });
    }

    public static void SubscribeToTheNotificationEvents()
    {
        var client = UM_NotificationCenter.Client;
        var startRequest = client.LastOpenedNotification;
        if(startRequest != null) {
            UM_DialogsUtility.ShowMessage("Launched via Notification", GetNotificationInfo(startRequest));
            //if this isn't null on your app launch, means user launched your app by clicking on notification icon
        }
        
        client.OnNotificationReceived.AddListener(request => {
            //Notification was received while app is running
            UM_DialogsUtility.ShowMessage("Notification Received", GetNotificationInfo(request));
        });
        
        client.OnNotificationClick.AddListener(request => {
            //User clicked on notification while app is running
            UM_DialogsUtility.ShowMessage("Restored from background via Notification", GetNotificationInfo(request));
        });
    }

    private static string GetNotificationInfo(UM_NotificationRequest request)
    {
        return string.Format("request.Identifier: {0} \n request.Content.Title: {1}",
            request.Identifier,
            request.Content.Title);
    }
}
