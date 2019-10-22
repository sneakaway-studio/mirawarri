/* 
*   NatCam
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCam.Internal {

    using UnityEngine;
    using UnityEngine.Scripting;
    using System;

    public sealed class CameraDeviceAndroid : CameraDevice {

        #region --Introspection--

        public new static CameraDeviceAndroid[] GetDevices () {
            CameraDevice = CameraDevice ?? new AndroidJavaClass(@"com.olokobayusuf.natcam.CameraDevice");
            Unmanaged = Unmanaged ?? new AndroidJavaClass(@"com.olokobayusuf.natrender.Unmanaged");
            try {
                using (var devicesArray = CameraDevice.CallStatic<AndroidJavaObject>(@"getDevices")) {
                    var devices = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(devicesArray.GetRawObject());
                    var result = new CameraDeviceAndroid[devices.Length];
                    for (var i = 0; i < devices.Length; i++)
                        result[i] = new CameraDeviceAndroid(devices[i]);
                    return result;
                }
            } catch (Exception) { // Permissions denied
                return null;
            }
        }
        #endregion


        #region --Properties--

        public override string UniqueID {
            get { return device.Call<string>(@"uniqueID"); }
        }

        public override bool IsFrontFacing {
            get { return device.Call<bool>(@"isFrontFacing"); }
        }

        public override bool IsFlashSupported {
            get { return device.Call<bool>(@"isFlashSupported"); }
        }

        public override bool IsTorchSupported {
            get { return device.Call<bool>(@"isTorchSupported"); }
        }

        public override bool IsExposureLockSupported {
            get { return device.Call<bool>(@"isExposureLockSupported"); }
        }

        public override bool IsFocusLockSupported {
            get { return device.Call<bool>(@"isFocusLockSupported"); }
        }

        public override bool IsWhiteBalanceLockSupported {
            get { return device.Call<bool>(@"isWhiteBalanceLockSupported"); }
        }

        public override float HorizontalFOV {
            get { return device.Call<float>(@"horizontalFOV"); }
        }
        
        public override float VerticalFOV {
            get { return device.Call<float>(@"verticalFOV"); }
        }

        public override float MinExposureBias {
            get { return device.Call<float>(@"minExposureBias"); }
        }

        public override float MaxExposureBias {
            get { return device.Call<float>(@"maxExposureBias"); }
        }

        public override float MaxZoomRatio {
            get { return device.Call<float>(@"maxZoomRatio"); }
        }
        #endregion


        #region --Settings--

        public override Resolution PreviewResolution {
            get {
                using (var size = device.Call<AndroidJavaObject>(@"getPreviewResolution"))
                    return new Resolution { width = size.Call<int>(@"getWidth"), height = size.Call<int>(@"getHeight") };
            }
            set { device.Call(@"setPreviewResolution", value.width, value.height); }
        }

        public override Resolution PhotoResolution {
            get {
                using (var size = device.Call<AndroidJavaObject>(@"getPhotoResolution"))
                    return new Resolution { width = size.Call<int>(@"getWidth"), height = size.Call<int>(@"getHeight") };
            }
            set { device.Call(@"setPhotoResolution", value.width, value.height); }
        }
        
        public override int Framerate {
            get { return device.Call<int>(@"getFramerate"); }
            set { device.Call(@"setFramerate", value); }
        }

        public override float ExposureBias {
            get { return device.Call<float>(@"getExposureBias"); } 
            set { device.Call(@"setExposureBias", (int)value); }
        }

        public override bool ExposureLock {
            get { return device.Call<bool>(@"getExposureLock"); }
            set { device.Call(@"setExposureLock", value); }
        }

        public override Vector2 ExposurePoint {
            set { device.Call(@"setExposurePoint", value.x, value.y); }
        }

        public override FlashMode FlashMode {
            get { return (FlashMode)device.Call<int>(@"getFlashMode"); } 
            set { device.Call(@"setFlashMode", (int)value); }
        }

        public override bool FocusLock {
            get { return device.Call<bool>(@"getFocusLock"); }
            set { device.Call(@"setFocusLock", value); }
        }

        public override Vector2 FocusPoint {
            set { device.Call(@"setFocusPoint", value.x, value.y); }
        }

        public override bool TorchEnabled {
            get { return device.Call<bool>(@"getTorchEnabled"); } 
            set { device.Call(@"setTorchEnabled", value); }
        }

        public override bool WhiteBalanceLock {
            get { return device.Call<bool>(@"getWhiteBalanceLock"); }
            set { device.Call(@"setWhiteBalanceLock", value); }
        }

        public override float ZoomRatio {
            get { return device.Call<float>(@"getZoomRatio"); } 
            set { device.Call(@"setZoomRatio", value); }
        }
        #endregion


        #region --DeviceCamera--

        public override bool IsRunning {
            get { return callback != null; }
        }

        public override void StartPreview (Action<Texture2D> startCallback, Action<long> frameCallback, ScreenOrientation rotation) {
            this.rotation = rotation != 0 ? rotation : Screen.orientation;
            this.callback = new Callback(startCallback, frameCallback);
            this.focusHandler = FocusHandler.Create(this);
            RenderDispatcher.Dispatch(() => {
                AndroidJNI.AttachCurrentThread();
                device.Call(@"startPreview", (int)this.rotation, callback);
            });
        }

        public override void StopPreview () {
            device.Call(@"stopPreview");
            callback.Dispose();
            focusHandler.Dispose();
            callback = null;
            focusHandler = null;
        }

        public override void CapturePhoto (Action<Texture2D> callback) {
            this.callback.photoCallback = callback;
            device.Call(@"capturePhoto");
        }
        #endregion


        #region --Operations--

        private static AndroidJavaClass CameraDevice;
        private static AndroidJavaClass Unmanaged;
        private readonly AndroidJavaObject device;
        private Callback callback;
        private FocusHandler focusHandler;
        private ScreenOrientation rotation;

        private CameraDeviceAndroid (AndroidJavaObject device) {
            this.device = device;
        }

        ~CameraDeviceAndroid () {
            device.Dispose();
        }

        private sealed class Callback : AndroidJavaProxy, IDisposable {

            public readonly Action<Texture2D> startCallback;
            public readonly Action<long> frameCallback;
            public Action<Texture2D> photoCallback;
            private Texture2D previewTexture;

            public Callback (Action<Texture2D> startCallback, Action<long> frameCallback) : base(@"com.olokobayusuf.natcam.CameraDevice$Callback") {
                this.startCallback = startCallback;
                this.frameCallback = frameCallback;
            }

            public void Dispose () {
                Texture2D.Destroy(previewTexture);
                previewTexture = null;
            }

            [Preserve]
            private void onFrame (AndroidJavaObject nativeBuffer, int textureID, int width, int height, long timestamp) {
                // Inspect pixel buffer
                var pixelBuffer = (IntPtr)Unmanaged.CallStatic<long>(@"baseAddress", nativeBuffer);
                nativeBuffer.Dispose();
                // Update preview texture
                var firstFrame = !previewTexture;
                previewTexture = previewTexture ?? new Texture2D(width, height, TextureFormat.RGBA32, false, false);
                previewTexture.LoadRawTextureData(pixelBuffer, width * height * 4);
                previewTexture.UpdateExternalTexture((IntPtr)textureID);
                // Invoke handlers
                if (firstFrame)
                    startCallback(previewTexture);
                if (frameCallback != null)
                    frameCallback(timestamp);
            }

            [Preserve]
            private void onPhoto (AndroidJavaObject nativeBuffer, int width, int height) {
                // Inspect pixel buffer
                var pixelBuffer = (IntPtr)Unmanaged.CallStatic<long>(@"baseAddress", nativeBuffer);
                nativeBuffer.Dispose();
                // Send to delegates
                var photoTexture = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
                photoTexture.LoadRawTextureData(pixelBuffer, width * height * 4);
                photoTexture.Apply();
                photoCallback(photoTexture);
            }
        }

        private sealed class FocusHandler : MonoBehaviour, IDisposable {

            private CameraDeviceAndroid receiver;
            private Action<Texture2D> startCallback;
            private Action<long> frameCallback;
            private bool pausing;

            public static FocusHandler Create (CameraDeviceAndroid receiver) {
                var handler = new GameObject("NatCam Focus Handler").AddComponent<FocusHandler>();
                handler.receiver = receiver;
                handler.startCallback = receiver.callback.startCallback;
                handler.frameCallback = receiver.callback.frameCallback;
                return handler;
            }

            public void Dispose () {
                // If this was triggered by a pause, ignore
                if (pausing)
                    return;
                FocusHandler.Destroy(this);
                GameObject.Destroy(this.gameObject);
            }

            private void Awake () {
                DontDestroyOnLoad(this);
                DontDestroyOnLoad(this.gameObject);
            }

            private void OnApplicationPause (bool paused) {
                this.pausing = paused;
                if (paused)
                    receiver.StopPreview();
                else {
                    receiver.StartPreview(startCallback, frameCallback, receiver.rotation);
                    Dispose();
                }
            }
        }
        #endregion
    }
}