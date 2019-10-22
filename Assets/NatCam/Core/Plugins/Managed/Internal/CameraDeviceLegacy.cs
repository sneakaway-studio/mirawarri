/* 
*   NatCam
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCam.Internal {

    using UnityEngine;
    using System;
    using System.Collections;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public sealed class CameraDeviceLegacy : CameraDevice {

        #region --Introspection--

        public new static CameraDeviceLegacy[] GetDevices () {
            var devices = WebCamTexture.devices;
            var result = new CameraDeviceLegacy[devices.Length];
            for (var i = 0; i < devices.Length; i++)
                result[i] = new CameraDeviceLegacy(devices[i]);
            return result;
        }
        #endregion


        #region --Properties--

        public override string UniqueID {
            get { return device.name; }
        }

        public override bool IsFrontFacing {
            get { return device.isFrontFacing; }
        }

        public override bool IsFlashSupported {
            get { return false; }
        }

        public override bool IsTorchSupported {
            get { return false; }
        }

        public override bool IsExposureLockSupported {
            get { return false; }
        }

        public override bool IsFocusLockSupported {
            get { return false; }
        }

        public override bool IsWhiteBalanceLockSupported {
            get { return false; }
        }

        public override float HorizontalFOV {
            get {
                Debug.LogWarning("NatCam Error: Field of view is not supported on legacy backend");
                return 0f;
            }
        }

        public override float VerticalFOV {
            get {
                Debug.LogWarning("NatCam Error: Field of view is not supported on legacy backend");
                return 0f;
            }
        }

        public override float MinExposureBias {
            get { return 0f; }
        }

        public override float MaxExposureBias {
            get { return 0f; }
        }

        public override float MaxZoomRatio {
            get { return 1f; }
        }
        #endregion


        #region --Settings--

        public override Resolution PreviewResolution { get; set; }

        public override Resolution PhotoResolution {
            get { return PreviewResolution; }
            set { }
        }

        public override int Framerate { get; set; }

        public override float ExposureBias {
            get { return 0f; } 
            set { }
        }

        public override bool ExposureLock {
            get { return false; } 
            set { }
        }

        public override Vector2 ExposurePoint {
            set { }
        }

        public override FlashMode FlashMode {
            get { return 0; } 
            set { }
        }

        public override bool FocusLock {
            get { return false; } 
            set { }
        }

        public override Vector2 FocusPoint {
            set {  }
        }

        public override bool TorchEnabled {
            get { return false; } 
            set { }
        }

        public override bool WhiteBalanceLock {
            get { return false; } 
            set { }
        }

        public override float ZoomRatio {
            get { return 1f; } 
            set { }
        }
        #endregion
        

        #region --DeviceCamera--

        public override bool IsRunning {
            get { return webcamTexture; }
        }

        public override void StartPreview (Action<Texture2D> startCallback, Action<long> frameCallback, ScreenOrientation rotation) {
            this.startCallback = startCallback;
            this.frameCallback = frameCallback;
            webcamTexture = new WebCamTexture(device.name, PreviewResolution.width, PreviewResolution.height, Framerate);
            webcamTexture.Play();
            frameHelper = new GameObject("NatCam Helper").AddComponent<CameraDeviceAttachment>();
            frameHelper.StartCoroutine(Update());
        }

        public override void StopPreview () {
            CameraDeviceAttachment.Destroy(frameHelper);
            webcamTexture.Stop();
            WebCamTexture.Destroy(webcamTexture);
            Texture2D.Destroy(previewTexture);
            frameHelper = null;
            webcamTexture = null;
            previewTexture = null;
            startCallback = null;
            frameCallback = null;
            pixelBuffer = null;
        }

        public override void CapturePhoto (Action<Texture2D> callback) {
            var photo = new Texture2D(previewTexture.width, previewTexture.height, previewTexture.format, false, false);
            photo.SetPixels32(pixelBuffer);
            photo.Apply();
            callback(photo);
        }
        #endregion


        #region --Operations--

        private readonly WebCamDevice device;
        private Action<Texture2D> startCallback;
        private Action<long> frameCallback;
        private WebCamTexture webcamTexture;
        private Texture2D previewTexture;
        private Color32[] pixelBuffer;
        private CameraDeviceAttachment frameHelper;
        
        private CameraDeviceLegacy (WebCamDevice device) {
            this.device = device;
            this.PreviewResolution = new Resolution { width=1280, height=720 };
            this.Framerate = 30;
        }

        private IEnumerator Update () {
            for (;;) {
                yield return new WaitForEndOfFrame();
                // Check that we are playing
                if (webcamTexture.width == 16 || webcamTexture.height == 16)
                    continue;
                // Update preview buffer
                bool dirty = previewTexture == null;
                previewTexture = previewTexture ?? new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false, false);
                pixelBuffer = pixelBuffer ?? webcamTexture.GetPixels32();
                webcamTexture.GetPixels32(pixelBuffer);
                // Update preview texture
                previewTexture.SetPixels32(pixelBuffer);
                previewTexture.Apply();
                // Invoke handlers
                if (dirty)
                    startCallback(previewTexture);
                if (frameCallback != null)
                    frameCallback(Stopwatch.GetTimestamp() * 100L);
            }
        }

        private class CameraDeviceAttachment : MonoBehaviour { }
        #endregion
    }
}