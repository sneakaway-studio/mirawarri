/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using Dispatch;
    using Utilities;

    //[CoreDoc(91)]
    public sealed partial class NatCamAndroid : AndroidJavaProxy, INatCam {

        #region --Events--
        public event PreviewCallback OnStart;
        public event PreviewCallback OnFrame;
        #endregion


        #region --Op vars--
        private Texture2D preview;
        private IDispatch dispatch;
        private PhotoCallback photoCallback;
        #pragma warning disable 0414
        private readonly IDispatch renderDispatch;
        #pragma warning restore 0414
        private readonly AndroidJavaObject core;
        #endregion


        #region --Properties--
        public INatCamDevice Device {get; private set;}
        public int Camera {
            get {
                return core.Call<int>("getCameraIndex");
            }
            set {
                #if NATCAM_PRO || NATCAM_PROFESSIONAL
                if (IsRecording) {
                    Utilities.LogError("Cannot switch cameras while recording");
                    return;
                }
                #endif
                core.Call("setCamera", value);
            }
        }
        public Texture Preview { get { return preview;}}
        public bool IsPlaying { get { return core.Call<bool>("isPlaying");}}
        public bool Verbose { set { core.Call("setVerboseMode", value);}}
        public bool HasPermissions { get { return core.Call<bool>("hasPermissions");}}
        #endregion


        #region --Ctor--

        public NatCamAndroid () : base("com.yusufolokoba.natcam.NatCamDelegate") {
            core = new AndroidJavaObject("com.yusufolokoba.natcam.NatCam", this);
            renderDispatch = new RenderDispatch();
            Device = new NatCamDeviceAndroid();
            DispatchUtility.onPause += OnPause;
            #if NATCAM_CORE
            DispatchUtility.onOrient += OnOrient;
            #endif
            #if NATCAM_PRO || NATCAM_PROFESSIONAL
            pro = new AndroidJavaObject("com.yusufolokoba.natcampro.NatCamPro");
            core.Call("setReadablePreview", PreviewData);
            #endif
            Utilities.Log("Initialized NatCam 1.6 Android backend");
        }
        #endregion
        

        #region --Operations--

        public void Play () {
            dispatch = dispatch ?? new MainDispatch();
            OnOrient();
            core.Call("play");
        }

        public void Pause () {
            core.Call("pause");
        }

        public void Release () {
            OnStart = 
            OnFrame = null;
            core.Call("release");
            if (preview != null) MonoBehaviour.Destroy(preview); preview = null;
            if (dispatch != null) dispatch.Release(); dispatch = null;
        }

        public void CapturePhoto (PhotoCallback callback) {
            photoCallback = callback;
            core.Call("capturePhoto");
        }
        #endregion


        #region --Callbacks--

        private void onStart (int texPtr, int width, int height) {
            dispatch.Dispatch(() => {
                preview = preview ?? Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, (IntPtr)texPtr);
                if (preview.width != width || preview.height != height) preview.Resize(width, height, preview.format, false);
                if (OnStart != null) OnStart();
            });
        }

        private void onFrame (int texPtr) {
            dispatch.Dispatch(() => {
                if (preview == null) return;
                preview.UpdateExternalTexture((IntPtr)texPtr);
                if (OnFrame != null) OnFrame();
            });
        }

        private void onPhoto (AndroidJavaObject bitmap, int width, int height, byte orientation) {
            var pixelBuffer = bitmap.Get<AndroidJavaObject>("mBuffer"); // This is unsafe, but we'll stick with it anyway // `bitmap` is an android.graphics.Bitmap
            var pixelData = AndroidJNI.FromByteArray(pixelBuffer.GetRawObject());
            dispatch.Dispatch(() => {
                var photo = new Texture2D(width, height, TextureFormat.RGB565, false);
                photo.LoadRawTextureData(pixelData);
                photo.Apply();
                photoCallback(photo, (Orientation)orientation);
            });
        }

        partial void onVideo (string path);
        #endregion


        #region --Utility--
        
        private void OnPause (bool paused) {
            #if NATCAM_PRO || NATCAM_PROFESSIONAL
            if (IsRecording) {
                Utilities.LogError("Suspending app while recording. Ending recording");
                StopRecording();
            }
            #endif
            core.Call("onPause", paused);
        }

        private void OnOrient () {
            #if NATCAM_CORE
            core.Call("onOrient", (byte)DispatchUtility.Orientation);
            #endif
        }
        #endregion
    }
}