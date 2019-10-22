/* 
*   NatCam
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCam.Internal {

    using AOT;
    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;

    public sealed class CameraDeviceiOS : CameraDevice {

        #region --Introspection--

        public new static CameraDeviceiOS[] GetDevices () {
            // Get native devices
            IntPtr deviceArray;
            int deviceCount;
            CameraDeviceBridge.GetDevices(out deviceArray, out deviceCount);
            // Check permissions
            if (deviceArray == IntPtr.Zero)
                return null;
            // Marshal
            var devices = new CameraDeviceiOS[deviceCount];
            for (var i = 0; i < deviceCount; i++) {
                var device = Marshal.ReadIntPtr(deviceArray, i * Marshal.SizeOf(typeof(IntPtr)));
                devices[i] = new CameraDeviceiOS(device);
            }
            Marshal.FreeCoTaskMem(deviceArray);
            return devices;
        }
        #endregion


        #region --Properties--
        
        public override string UniqueID {
            get {
                var nativeStr = device.DeviceUID();
                var name = Marshal.PtrToStringAuto(nativeStr);
                Marshal.FreeCoTaskMem(nativeStr);
                return name;
            }
        }

        public override bool IsFrontFacing {
            get { return device.IsFrontFacing(); }
        }

        public override bool IsFlashSupported {
            get { return device.IsFlashSupported(); }
        }

        public override bool IsTorchSupported {
            get { return device.IsTorchSupported(); }
        }

        public override bool IsExposureLockSupported {
            get { return device.IsExposureLockSupported(); }
        }

        public override bool IsFocusLockSupported {
            get { return device.IsFocusLockSupported(); }
        }

        public override bool IsWhiteBalanceLockSupported {
            get { return device.IsWhiteBalanceLockSupported(); }
        }

        public override float HorizontalFOV {
            get { return device.HorizontalFOV(); }
        }

        public override float VerticalFOV {
            get { return device.VerticalFOV(); }
        }

        public override float MinExposureBias {
            get { return device.MinExposureBias(); }
        }

        public override float MaxExposureBias {
            get { return device.MaxExposureBias(); }
        }

        public override float MaxZoomRatio {
            get { return device.MaxZoomRatio(); }
        }
        #endregion


        #region --Settings--

        public override Resolution PreviewResolution {
            get {
                int width, height;
                device.GetPreviewResolution(out width, out height);
                return new Resolution { width = width, height = height };
            }
            set { device.SetPreviewResolution(value.width, value.height); }
        }

        public override Resolution PhotoResolution {
            get {
                int width, height;
                device.GetPhotoResolution(out width, out height);
                return new Resolution { width = width, height = height };
            }
            set { device.SetPhotoResolution(value.width, value.height); }
        }
        
        public override int Framerate {
            get { return device.GetFramerate(); }
            set { device.SetFramerate(value); }
        }

        public override float ExposureBias {
            get { return device.GetExposureBias(); }
            set { device.SetExposureBias(value); }
        }

        public override bool ExposureLock {
            get { return device.GetExposureLock(); }
            set { device.SetExposureLock(value); }
        }

        public override Vector2 ExposurePoint {
            set { device.SetExposurePoint(value.x, value.y); }
        }

        public override FlashMode FlashMode {
            get { return device.GetFlashMode(); } 
            set { device.SetFlashMode(value); }
        }

        public override bool FocusLock {
            get { return device.GetFocusLock(); }
            set { device.SetFocusLock(value); }
        }

        public override Vector2 FocusPoint {
            set { device.SetFocusPoint(value.x, value.y); }
        }

        public override bool TorchEnabled {
            get { return device.GetTorchEnabled(); } 
            set { device.SetTorchEnabled(value); }
        }

        public override bool WhiteBalanceLock {
            get { return device.GetWhiteBalanceLock(); }
            set { device.SetWhiteBalanceLock(value); }
        }

        public override float ZoomRatio {
            get { return device.GetZoomRatio(); } 
            set { device.SetZoomRatio(value); }
        }
        #endregion


        #region --DeviceCamera--

        public override bool IsRunning {
            get { return device.IsRunning(); }
        }

        public override void StartPreview (Action<Texture2D> startCallback, Action<long> frameCallback, ScreenOrientation rotation) {
            rotation = rotation != 0 ? rotation : Screen.orientation;
            this.self = GCHandle.Alloc(this, GCHandleType.Normal); // Keep strong ref
            this.startCallback = startCallback;
            this.frameCallback = frameCallback;
            device.StartPreview((int)rotation, OnFrame, (IntPtr)self);
        }

        public override void StopPreview () {
            device.StopPreview();
            Texture2D.Destroy(previewTexture);
            self.Free();
            previewTexture = null;
            startCallback = null;
            frameCallback = null;
        }

        public override void CapturePhoto (Action<Texture2D> callback) {
            this.photoCallback = callback;
            device.CapturePhoto(OnPhoto, (IntPtr)self);
        }
        #endregion


        #region --Operations--

        private readonly IntPtr device;
        private GCHandle self;
        private Action<Texture2D> startCallback;
        private Action<long> frameCallback;
        private Action<Texture2D> photoCallback;
        private Texture2D previewTexture;

        private CameraDeviceiOS (IntPtr device) {
            this.device = device;
        }

        ~CameraDeviceiOS () {
            device.FreeDevice();
        }

        [MonoPInvokeCallback(typeof(CameraDeviceBridge.FrameCallback))]
        private static void OnFrame (IntPtr context, IntPtr pixelBuffer, IntPtr texturePtr, int width, int height, long timestamp) {            
            var camera = ((GCHandle)context).Target as CameraDeviceiOS;
            // Dirty checking
            var firstFrame = !camera.previewTexture;
            camera.previewTexture = camera.previewTexture ?? new Texture2D(width, height, TextureFormat.RGBA32, false, false);
            camera.previewTexture.LoadRawTextureData(pixelBuffer, width * height * 4);
            camera.previewTexture.UpdateExternalTexture(texturePtr);
            // Invoke handlers
            if (firstFrame)
                camera.startCallback(camera.previewTexture);
            else if (camera.frameCallback != null)
                camera.frameCallback(timestamp);
        }

        [MonoPInvokeCallback(typeof(CameraDeviceBridge.PhotoCallback))]
        private static void OnPhoto (IntPtr context, IntPtr pixelBuffer, int width, int height) {            
            // Create photo
            var camera = ((GCHandle)context).Target as CameraDeviceiOS;
            var photo = new Texture2D(width, height, TextureFormat.BGRA32, false);
            photo.LoadRawTextureData(pixelBuffer, width * height * 4);
            photo.Apply();
            camera.photoCallback(photo);
        }
        #endregion
    }
}