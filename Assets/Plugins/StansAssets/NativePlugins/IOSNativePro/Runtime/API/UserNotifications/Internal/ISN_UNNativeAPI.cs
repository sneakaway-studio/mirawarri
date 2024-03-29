////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.Utilities;
using SA.Foundation.Templates;
using SA.Foundation.Events;


#if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.UserNotifications.Internal
{

    internal class ISN_UNNativeAPI : ISN_Singleton<ISN_UNNativeAPI>, ISN_iUNAPI
    {
        #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            
        [DllImport("__Internal")] static extern void _ISN_UN_RequestAuthorization(int options);
        [DllImport("__Internal")] static extern void _ISN_UN_GetNotificationSettings();
        [DllImport("__Internal")] static extern void _ISN_UN_AddNotificationRequest(string contentJSON);

        [DllImport("__Internal")] static extern void _ISN_UN_RemoveAllPendingNotificationRequests();
        [DllImport("__Internal")] static extern void _ISN_UN_RemovePendingNotificationRequests(string contentJSON);
        [DllImport("__Internal")] static extern void _ISN_UN_GetPendingNotificationRequests();
        [DllImport("__Internal")] static extern void _ISN_UN_RemoveAllDeliveredNotifications();
        [DllImport("__Internal")] static extern void _ISN_UN_RemoveDeliveredNotifications(string contentJSON);
        [DllImport("__Internal")] static extern void _ISN_UN_GetDeliveredNotifications();

        [DllImport("__Internal")] static extern string _ISN_UN_GetLastReceivedResponse();
        [DllImport("__Internal")] static extern void _ISN_UN_ClearLastReceivedResponse();

#endif

        private SA_Event<ISN_UNNotification> m_willPresentNotification = new SA_Event<ISN_UNNotification>();
        private SA_Event<ISN_UNNotificationResponse> m_didReceiveNotificationResponse = new SA_Event<ISN_UNNotificationResponse>();


        //--------------------------------------
        // _ISN_UN_RequestAuthorization
        //--------------------------------------
     
        private event Action<SA_Result> m_onRequestAuthorization;
        public void RequestAuthorization(int options, Action<SA_Result> callback) {
            m_onRequestAuthorization = callback;

            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            _ISN_UN_RequestAuthorization(options);
            #endif
        }

        void OnRequestAuthorization(string json) {
            SA_Result result = JsonUtility.FromJson<SA_Result>(json);
            m_onRequestAuthorization.Invoke(result);
        }


        //--------------------------------------
        // _ISN_UN_GetNotificationSettings
        //--------------------------------------

        private event Action<ISN_UNNotificationSettings> m_onGetNotificationSettings = null;
        public void GetNotificationSettings(Action<ISN_UNNotificationSettings> callback) {
            m_onGetNotificationSettings = callback;
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            _ISN_UN_GetNotificationSettings();
            #endif
        }


        void OnGetNotificationSettings(string json) {
            ISN_UNNotificationSettings result = JsonUtility.FromJson<ISN_UNNotificationSettings>(json);
            m_onGetNotificationSettings.Invoke(result);
        }

        //--------------------------------------
        // _ISN_UN_AddNotificationRequest
        //--------------------------------------

        private Action<SA_Result> m_onAddNotificationRequest;
        public void AddNotificationRequest(ISN_UNNotificationRequest request, Action<SA_Result> callback) {

            m_onAddNotificationRequest = callback;
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            _ISN_UN_AddNotificationRequest(JsonUtility.ToJson(request));
            #endif
        }

        void OnAddNotificationRequest(string json) {
            SA_Result result = JsonUtility.FromJson<SA_Result>(json);
            m_onAddNotificationRequest.Invoke(result);
        }


        //-----------------------------------------
        // _ISN_UN_GetPendingNotificationRequests
        //-----------------------------------------

        private Action<List<ISN_UNNotificationRequest>> m_onGetPendingNotificationRequests = null;
        public void GetPendingNotificationRequests(Action<List<ISN_UNNotificationRequest>> callback) {
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            m_onGetPendingNotificationRequests = callback;
            _ISN_UN_GetPendingNotificationRequests();
            #endif
        }

        void OnGetPendingNotificationRequests(string json) {
            var result = JsonUtility.FromJson<ISN_UNNotificationRequests>(json);
            m_onGetPendingNotificationRequests.Invoke(result.Requests);
        }


        public void RemoveAllPendingNotificationRequests() {
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            _ISN_UN_RemoveAllPendingNotificationRequests();
            #endif
        }


        public void RemovePendingNotificationRequests(List<string> notificationRequestsIds) {
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            var request = new ISN_UNNotifcationRequestsIds(notificationRequestsIds);
            _ISN_UN_RemovePendingNotificationRequests(JsonUtility.ToJson(request));
            #endif
        }


        //-----------------------------------------
        // _ISN_UN_GetDeliveredNotifications
        //-----------------------------------------


        private Action<List<ISN_UNNotification>> m_onGetDeliveredNotifications = null;
        public void GetDeliveredNotifications(Action<List<ISN_UNNotification>> callback) {
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            m_onGetDeliveredNotifications = callback;
            _ISN_UN_GetDeliveredNotifications();
            #endif
        }


        void OnGetDeliveredNotifications(string json) {
            var result = JsonUtility.FromJson<ISN_UNNotifications>(json);
            m_onGetDeliveredNotifications.Invoke(result.Notifications);
        }


        public void RemoveAllDeliveredNotifications() {
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            _ISN_UN_RemoveAllDeliveredNotifications();
            #endif
        }


        public void RemoveDeliveredNotifications(List<string> notificationIds) {
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            var request = new ISN_UNNotifcationRequestsIds(notificationIds);
            _ISN_UN_RemoveDeliveredNotifications(JsonUtility.ToJson(request));
            #endif
        }

        public void ClearLastReceivedResponse(List<string> notificationIds) {
            #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
            _ISN_UN_ClearLastReceivedResponse();
            #endif
        }



        //-----------------------------------------
        // ISN_UNUserNotificationCenterDelegate
        //-----------------------------------------


        public void ClearLastReceivedResponse() {}



        public SA_iEvent<ISN_UNNotification> WillPresentNotification {
            get {
                return m_willPresentNotification;
            }
        }

        public SA_iEvent<ISN_UNNotificationResponse> DidReceiveNotificationResponse {
            get {
                return m_didReceiveNotificationResponse;
            }
        }


        public ISN_UNNotificationResponse LastReceivedResponse {
            get {
                string json = string.Empty;
                #if (UNITY_IPHONE || UNITY_TVOS) && USER_NOTIFICATIONS_API_ENABLED
                json =  _ISN_UN_GetLastReceivedResponse();
                #endif

                if(string.IsNullOrEmpty(json)) {
                    return null;
                } else {
                    return JsonUtility.FromJson<ISN_UNNotificationResponse>(json);
                }
            }
        }


        //--------------------------------------
        // Native events to catch
        //--------------------------------------

        void DidReceiveNotificationResponseEvent(string json) {
            ISN_UNNotificationResponse response = JsonUtility.FromJson<ISN_UNNotificationResponse>(json);
            m_didReceiveNotificationResponse.Invoke(response);
        }


        void WillPresentNotificationEvent(string json) {
            ISN_UNNotification notification = JsonUtility.FromJson<ISN_UNNotification>(json);
            m_willPresentNotification.Invoke(notification);
        }

    }
}
