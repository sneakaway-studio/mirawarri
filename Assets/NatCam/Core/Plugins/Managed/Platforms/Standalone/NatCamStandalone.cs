/* 
*   NatCam Core
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using AOT;
    using UnityEngine;
    using System;
    using Dispatch;
    using Utilities;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamStandalone : INatCam { // NOTE: This should be base class for NatCamiOS

        #region --Events--

        public event PreviewCallback OnStart;
        public event PreviewCallback OnFrame;
        #endregion


        #region --Op vars--
        private Texture2D preview;
        #endregion


        #region --Properties--
        public INatCamDevice Device {get; private set;}
        public int Camera {
            get {
                return NatCamBridge.GetCamera();
            }
            set {
                // CHECK // Handle recording?
                NatCamBridge.SetCamera(value);
            }
        }
        public Texture Preview {
            get {
                return preview;
            }
        }
        public bool IsPlaying {
            get {
                return NatCamBridge.IsPlaying();
            }
        }
        public bool Verbose {
            set {
                NatCamBridge.SetVerboseMode(value);
            }
        }
        public bool HasPermissions {
            get {
                return true; // Assume always true
            }
        }
        #endregion


        #region --Ctor--

        public NatCamStandalone () {
            Device = new NatCamDeviceiOS();
            Utilities.Log("Initialized NatCam 1.6 Standalone backend");
        }
        #endregion


        #region --Operations--

        public void Play () {
            NatCamBridge.Play();
        }

        public void Pause () {
            NatCamBridge.Pause();
        }

        public void Release () {
            OnStart = 
            OnFrame = null;
            #if NATCAM_PRO
            // ...
            #endif
            NatCamBridge.Release();
            if (preview != null) MonoBehaviour.Destroy(preview); preview = null;
        }

        public void CapturePhoto (PhotoCallback callback) {

        }
        #endregion


        #region --Callbacks--

        [MonoPInvokeCallback(typeof(NatCamBridge.PhotoCallback))]
        private void onPhoto (IntPtr imgPtr, int width, int height, int size) {
            
        }
        #endregion
    }
}