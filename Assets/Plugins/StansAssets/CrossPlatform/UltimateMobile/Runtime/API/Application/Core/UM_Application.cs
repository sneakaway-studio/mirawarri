using SA.Android.App;
using SA.iOS.UIKit;
using UnityEngine;

namespace SA.CrossPlatform.App
{
    /// <summary>
    /// Main entry point for the Application APIs. 
    /// This class provides APIs and interfaces to access the Application services functionality.
    /// </summary>
    public static class UM_Application 
    {

        /// <summary>
        /// Returns a shared instance of <see cref="UM_iGalleryService"/>
        /// </summary>
        private static UM_iGalleryService m_galleryService = null;
        public static UM_iGalleryService GalleryService {
            get {
                if(m_galleryService == null) {
                    switch (Application.platform) {
                        case RuntimePlatform.Android:
                            m_galleryService = new UM_AndroidGalleryService();
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            m_galleryService = new UM_IOSGalleryService();
                            break;
                        default:
                            m_galleryService = new UM_EditorGalleryService();
                            break;
                    }
                }
                return m_galleryService;
            }
        }


        /// <summary>
        /// Returns a shared instance of <see cref="UM_iCameraService"/>
        /// </summary>
        private static UM_iCameraService m_cameraService;
        public static UM_iCameraService CameraService {
            get {
                if (m_cameraService == null) {
                    switch (Application.platform) {
                        case RuntimePlatform.Android:
                            m_cameraService = new UM_AndroidCameraService();
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            m_cameraService = new UM_IOSCameraService();
                            break;
                        default:
                            m_cameraService = new UM_EditorCameraService();
                            break;
                    }
                }
                return m_cameraService;
            }
        }


        /// <summary>
        /// Returns a shared instance of <see cref="UM_iContactsService"/>
        /// </summary>
        private static UM_iContactsService m_contactsService;
        public static UM_iContactsService ContactsService {
            get {
                if (m_contactsService == null) {
                    switch (Application.platform) {
                        case RuntimePlatform.Android:
                            m_contactsService = new UM_AndroidContactsService();
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            m_contactsService = new UM_IOSContactsService();
                            break;
                        default:
                            m_contactsService = new UM_EditorContactsService();
                            break;
                    }
                }
                return m_contactsService;
            }
        }


        /// <summary>
        /// Will send an application to background.
        /// The method can be used to emulate pressing a Home button.
        /// </summary>
        public static void SendToBackground()
        {
            switch (Application.platform) {
                case RuntimePlatform.Android:
                    AN_MainActivity.Instance.MoveTaskToBack(true);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ISN_UIApplication.Suspend();
                    break;
            }
        }

    }

}