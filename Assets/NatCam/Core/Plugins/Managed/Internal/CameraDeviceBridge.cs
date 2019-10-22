/* 
*   NatCam
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCam.Internal {

    using System;
    using System.Runtime.InteropServices;

    public static class CameraDeviceBridge {

        #region ---Delegates---
        public delegate void FrameCallback (IntPtr context, IntPtr pixelBuffer, IntPtr texturePtr, int width, int height, long timestamp);
        public delegate void PhotoCallback (IntPtr context, IntPtr pixelBuffer, int width, int height);
        #endregion

        #if UNITY_IOS && !UNITY_EDITOR

        [DllImport(@"__Internal", EntryPoint = @"NCGetDevices")]
        public static extern void GetDevices (out IntPtr outDevicesArray, out int outDevicesArrayCount);
        [DllImport(@"__Internal", EntryPoint = @"NCFreeDevice")]
        public static extern void FreeDevice (this IntPtr device);
        [DllImport(@"__Internal", EntryPoint = @"NCUniqueID")]
        public static extern IntPtr DeviceUID (this IntPtr device);
        [DllImport(@"__Internal", EntryPoint = @"NCIsFrontFacing")]
        public static extern bool IsFrontFacing (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCIsFlashSupported")]
        public static extern bool IsFlashSupported (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCIsTorchSupported")]
        public static extern bool IsTorchSupported (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCIsExposureLockSupported")]
        public static extern bool IsExposureLockSupported (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCIsFocusLockSupported")]
        public static extern bool IsFocusLockSupported (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCIsWhiteBalanceLockSupported")]
        public static extern bool IsWhiteBalanceLockSupported (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCHorizontalFOV")]
        public static extern float HorizontalFOV (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCVerticalFOV")]
        public static extern float VerticalFOV (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCMinExposureBias")]
        public static extern float MinExposureBias (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCMaxExposureBias")]
        public static extern float MaxExposureBias (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCMaxZoomRatio")]
        public static extern float MaxZoomRatio (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCGetPreviewResolution")]
        public static extern void GetPreviewResolution (this IntPtr camera, out int width, out int height);
        [DllImport(@"__Internal", EntryPoint = @"NCSetPreviewResolution")]
        public static extern void SetPreviewResolution (this IntPtr camera, int width, int height);
        [DllImport(@"__Internal", EntryPoint = @"NCGetPhotoResolution")]
        public static extern void GetPhotoResolution (this IntPtr camera, out int width, out int height);
        [DllImport(@"__Internal", EntryPoint = @"NCSetPhotoResolution")]
        public static extern void SetPhotoResolution (this IntPtr camera, int width, int height);
        [DllImport(@"__Internal", EntryPoint = @"NCGetFramerate")]
        public static extern int GetFramerate (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetFramerate")]
        public static extern void SetFramerate (this IntPtr camera, int framerate);
        [DllImport(@"__Internal", EntryPoint = @"NCGetExposureBias")]
        public static extern float GetExposureBias (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetExposureBias")]
        public static extern void SetExposureBias (this IntPtr camera, float bias);
        [DllImport(@"__Internal", EntryPoint = @"NCSetExposurePoint")]
        public static extern void SetExposurePoint (this IntPtr camera, float x, float y);
        [DllImport(@"__Internal", EntryPoint = @"NCGetExposureLock")]
        public static extern bool GetExposureLock (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetExposureLock")]
        public static extern void SetExposureLock (this IntPtr camera, bool locked);
        [DllImport(@"__Internal", EntryPoint = @"NCGetFlashMode")]
        public static extern FlashMode GetFlashMode (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetFlashMode")]
        public static extern void SetFlashMode (this IntPtr camera, FlashMode state);
        [DllImport(@"__Internal", EntryPoint = @"NCGetFocusLock")]
        public static extern bool GetFocusLock (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetFocusLock")]
        public static extern void SetFocusLock (this IntPtr camera, bool locked);
        [DllImport(@"__Internal", EntryPoint = @"NCSetFocusPoint")]
        public static extern void SetFocusPoint (this IntPtr camera, float x, float y);
        [DllImport(@"__Internal", EntryPoint = @"NCGetTorchEnabled")]
        public static extern bool GetTorchEnabled (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetTorchEnabled")]
        public static extern void SetTorchEnabled (this IntPtr camera, bool enabled);
        [DllImport(@"__Internal", EntryPoint = @"NCGetWhiteBalanceLock")]
        public static extern bool GetWhiteBalanceLock (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetWhiteBalanceLock")]
        public static extern void SetWhiteBalanceLock (this IntPtr camera, bool locked);
        [DllImport(@"__Internal", EntryPoint = @"NCGetZoomRatio")]
        public static extern float GetZoomRatio (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCSetZoomRatio")]
        public static extern void SetZoomRatio (this IntPtr camera, float ratio);
        [DllImport(@"__Internal", EntryPoint = @"NCIsRunning")]
        public static extern bool IsRunning (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCStartPreview")]
        public static extern void StartPreview (this IntPtr camera, int rotation, FrameCallback frameCallback, IntPtr context);
        [DllImport(@"__Internal", EntryPoint = @"NCStopPreview")]
        public static extern void StopPreview (this IntPtr camera);
        [DllImport(@"__Internal", EntryPoint = @"NCCapturePhoto")]
        public static extern void CapturePhoto (this IntPtr camera, PhotoCallback callback, IntPtr context);
        #else
        public static void GetDevices (out IntPtr outDevicesArray, out int outDevicesArrayCount) { outDevicesArray = IntPtr.Zero; outDevicesArrayCount = 0; }
        public static void FreeDevice (this IntPtr device) {}
        public static IntPtr DeviceUID (this IntPtr device) { return IntPtr.Zero; }
        public static bool IsFrontFacing (this IntPtr camera) { return true; }
        public static bool IsFlashSupported (this IntPtr camera) { return false; }
        public static bool IsTorchSupported (this IntPtr camera) { return false; }
        public static bool IsExposureLockSupported (this IntPtr camera) { return false; }
        public static bool IsFocusLockSupported (this IntPtr camera) { return false; }
        public static bool IsWhiteBalanceLockSupported (this IntPtr camera) { return false; }
        public static float HorizontalFOV (this IntPtr camera) { return 0; }
        public static float VerticalFOV (this IntPtr camera) { return 0; }
        public static float MinExposureBias (this IntPtr camera) { return 0; }
        public static float MaxExposureBias (this IntPtr camera) { return 0; }
        public static float MaxZoomRatio (this IntPtr camera) { return 1; }
        public static void GetPreviewResolution (this IntPtr camera, out int width, out int height) { width = height = 0; }
        public static void SetPreviewResolution (this IntPtr camera, int width, int height) {}
        public static void GetPhotoResolution (this IntPtr camera, out int width, out int height) { width = height = 0; }
        public static void SetPhotoResolution (this IntPtr camera, int width, int height) {}
        public static int GetFramerate (this IntPtr camera) { return 0; }
        public static void SetFramerate (this IntPtr camera, int framerate) {}
        public static float GetExposureBias (this IntPtr camera) { return 0; }
        public static void SetExposureBias (this IntPtr camera, float bias) {}
        public static void SetExposurePoint (this IntPtr camera, float x, float y) {}
        public static bool GetExposureLock (this IntPtr camera) { return false; }
        public static void SetExposureLock (this IntPtr camera, bool locked) {}
        public static FlashMode GetFlashMode (this IntPtr camera) { return 0; }
        public static void SetFlashMode (this IntPtr camera, FlashMode state) {}
        public static bool GetFocusLock (this IntPtr camera) { return false; }
        public static void SetFocusLock (this IntPtr camera, bool locked) {}
        public static void SetFocusPoint (this IntPtr camera, float x, float y) {}
        public static bool GetTorchEnabled (this IntPtr camera) { return false; }
        public static void SetTorchEnabled (this IntPtr camera, bool state) {}
        public static bool GetWhiteBalanceLock (this IntPtr camera) { return false; }
        public static void SetWhiteBalanceLock (this IntPtr camera, bool locked) {}
        public static float GetZoomRatio (this IntPtr camera) { return 0; }
        public static void SetZoomRatio (this IntPtr camera, float ratio) {}
        public static bool IsRunning (this IntPtr camera) { return false; }
        public static void StartPreview (this IntPtr camera, int rotation, FrameCallback frameCallback, IntPtr context) {}
        public static void StopPreview (this IntPtr camera) {}
        public static void CapturePhoto (this IntPtr camera, PhotoCallback callback, IntPtr context) {}
        #endif
    }
}