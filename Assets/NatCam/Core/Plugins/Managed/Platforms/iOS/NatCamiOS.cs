/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using AOT;
    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    using Dispatch;
    using Utilities;

    //[CoreDoc(92)]
    public sealed partial class NatCamiOS : INatCam {

        #region --Events--
        public event PreviewCallback OnStart;
        public event PreviewCallback OnFrame;
        #endregion


        #region --Op vars--
        private Texture2D preview;
        private PhotoCallback photoCallback;
        private static NatCamiOS instance { get {return NatCam.Implementation as NatCamiOS;}}
        #endregion
        

        #region --Properties--
        public INatCamDevice Device {get; private set;}
        public int Camera {
            get {
                return NatCamBridge.GetCamera();
            }
            set {
                #if NATCAM_PRO || NATCAM_PROFESSIONAL
                if (IsRecording) {
                    Utilities.LogError("Cannot switch cameras while recording");
                    return;
                }
                #endif
                NatCamBridge.SetCamera(value);
            }
        }
        public Texture Preview { get { return preview;}}
        public bool IsPlaying { get { return NatCamBridge.IsPlaying();}}
        public bool Verbose { set { NatCamBridge.SetVerboseMode(value);}}
        public bool HasPermissions { get { return NatCamBridge.HasPermissions();}}
        #endregion


        #region --Ctor--

        public NatCamiOS () {
            NatCamBridge.RegisterCoreCallbacks(onStart, onFrame, onPhoto);
            Device = new NatCamDeviceiOS();
            #if NATCAM_CORE
            DispatchUtility.onOrient += OnOrient;
            #endif
            #if NATCAM_PRO || NATCAM_PROFESSIONAL
            NatCamBridge.RegisterProCallbacks(OnVideo);
            #endif
            Utilities.Log("Initialized NatCam 1.6 iOS backend");
        }
        #endregion
        

        #region --Operations--

        public void Play () {
            OnOrient();
            NatCamBridge.Play();
        }

        public void Pause () {
            NatCamBridge.Pause();
        }

        public void Release () {
            OnStart = 
            OnFrame = null;
            #if NATCAM_PRO || NATCAM_PROFESSIONAL
            NatCamBridge.ReleaseBuffer();
            #endif
            NatCamBridge.Release();
            if (preview != null) MonoBehaviour.Destroy(preview); preview = null;
        }

        public void CapturePhoto (PhotoCallback callback) {
            photoCallback = callback;
            NatCamBridge.CapturePhoto();
        }
        #endregion


        #region --Callbacks--

        [MonoPInvokeCallback(typeof(NatCamBridge.StartCallback))]
        private static void onStart (IntPtr texPtr, int width, int height) {
            instance.preview = instance.preview ?? Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, texPtr);
            if (instance.preview.width != width || instance.preview.height != height) instance.preview.Resize(width, height, instance.preview.format, false);
            if (instance.OnStart != null) instance.OnStart();
        }

        [MonoPInvokeCallback(typeof(NatCamBridge.PreviewCallback))]
        private static void onFrame (IntPtr texPtr) {
            if (instance.preview == null) return;
            instance.preview.UpdateExternalTexture(texPtr);
            if (instance.OnFrame != null) instance.OnFrame();
        }
        
        [MonoPInvokeCallback(typeof(NatCamBridge.PhotoCallback))]
        private static void onPhoto (IntPtr imgPtr, int width, int height, int size) {
            using (var dispatch = new MainDispatch()) {
                if (instance.photoCallback == null) {
                    NatCamBridge.ReleasePhoto();
                    return;
                }
                if (imgPtr == IntPtr.Zero) return;
                var photo = new Texture2D(width, height, TextureFormat.BGRA32, false);
                photo.LoadRawTextureData(unchecked((IntPtr)(long)(ulong)imgPtr), size);
                photo.Apply();
                NatCamBridge.ReleasePhoto();
                instance.photoCallback(photo, 0);
                instance.photoCallback = null;
            }
        }
        #endregion


        #region --Utility--

        private void OnOrient () {
            #if NATCAM_CORE
            NatCamBridge.SetOrientation((byte)DispatchUtility.Orientation);
            #endif
        }
        #endregion
    }
}