/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Dispatch {

    using UnityEngine;
    using System;
    using System.Collections;
    #if NATCAM_CORE
    using Core;
    #endif
    
    [AddComponentMenu("")]
    public sealed class DispatchUtility : MonoBehaviour {

        #region --Events--
        public static event Action onFrame, onQuit;
        public static event Action<bool> onPause;
        #endregion


        #region --State--
        private static DispatchUtility instance;

        static DispatchUtility () {
            instance = new GameObject("NatCam Dispatch Utility").AddComponent<DispatchUtility>();
            instance.StartCoroutine(instance.OnFrame());
        }
        #endregion


        #region --Operations--

        void Awake () {
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(this);
            #if NATCAM_CORE
            CheckOrientation();
            #endif
        }

        void Update () {
            #if NATCAM_CORE
            CheckOrientation();
            #endif
        }
        
        void OnApplicationPause (bool paused) {
            if (onPause != null) onPause(paused);
        }
        
        void OnApplicationQuit () {
            if (onQuit != null) onQuit();
        }

        IEnumerator OnFrame () {
            YieldInstruction yielder = new WaitForEndOfFrame();
            for (;;) {
                yield return yielder;
                if (onFrame != null) onFrame();
            }
        }

        #if NATCAM_CORE

        private DeviceOrientation orientation = 0;
        public static event Action onOrient;
        public static Orientation Orientation {
            get {
                if (!Application.isMobilePlatform) return Orientation.Rotation_0;
                switch (instance.orientation) {
                    case DeviceOrientation.LandscapeLeft: return Orientation.Rotation_0;
                    case DeviceOrientation.Portrait: return Orientation.Rotation_90;
                    case DeviceOrientation.LandscapeRight: return Orientation.Rotation_180;
                    default: return Orientation.Rotation_90; // Why not 0?
                }
            }
        }

        void CheckOrientation () {
            DeviceOrientation reference = (DeviceOrientation)(int)Screen.orientation; //Input.deviceOrientation
            switch (reference) {
                case DeviceOrientation.FaceDown: case DeviceOrientation.FaceUp: case DeviceOrientation.Unknown: break;
                default: if (orientation != reference) {
                    orientation = reference;
                    if (onOrient != null) onOrient();
                }
                break;
            }
        }
        #endif
        #endregion
    }
}