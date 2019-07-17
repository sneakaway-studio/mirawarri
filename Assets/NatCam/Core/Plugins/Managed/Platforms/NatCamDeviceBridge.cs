/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System.Runtime.InteropServices;

    public static class NatCamDeviceBridge {

        private const string Assembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCam";
        #endif

        #if INATCAM_C

        #region --Properties--
        [DllImport(Assembly, EntryPoint = "NCCoreIsRearFacing")]
        public static extern bool IsRearFacing (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreIsFlashSupported")]
        public static extern bool IsFlashSupported (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreIsTorchSupported")]
        public static extern bool IsTorchSupported (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreHorizontalFOV")]
        public static extern float HorizontalFOV (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreVerticalFOV")]
        public static extern float VerticalFOV (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreMinExposureBias")]
        public static extern float MinExposureBias (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreMaxExposureBias")]
        public static extern float MaxExposureBias (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreMaxZoomRatio")]
        public static extern float MaxZoomRatio (this int camera);
        #endregion


        #region --Getters--

        [DllImport(Assembly, EntryPoint = "NCCoreGetPreviewResolution")]
        public static extern void GetPreviewResolution (this int camera, out int width, out int height);
        [DllImport(Assembly, EntryPoint = "NCCoreGetPhotoResolution")]
        public static extern void GetPhotoResolution (this int camera, out int width, out int height);
        [DllImport(Assembly, EntryPoint = "NCCoreGetFramerate")]
        public static extern float GetFramerate (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreGetExposure")]
        public static extern float GetExposure (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreGetExposureMode")]
        public static extern int GetExposureMode (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreGetFocusMode")]
        public static extern int GetFocusMode (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreGetFlash")]
        public static extern int GetFlash (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreGetTorch")]
        public static extern int GetTorch (this int camera);
        [DllImport(Assembly, EntryPoint = "NCCoreGetZoom")]
        public static extern float GetZoom (this int camera);
        #endregion


        #region --Setters--
        [DllImport(Assembly, EntryPoint = "NCCoreSetPreviewResolution")]
        public static extern void SetPreviewResolution (this int camera, int width, int height);
        [DllImport(Assembly, EntryPoint = "NCCoreSetPhotoResolution")]
        public static extern void SetPhotoResolution (this int camera, int width, int height);
        [DllImport(Assembly, EntryPoint = "NCCoreSetFramerate")]
        public static extern void SetFramerate (this int camera, float framerate);
        [DllImport(Assembly, EntryPoint = "NCCoreSetFocus")]
        public static extern bool SetFocus (this int camera, float x, float y);
        [DllImport(Assembly, EntryPoint = "NCCoreSetExposure")]
        public static extern float SetExposure (this int camera, float bias);
        [DllImport(Assembly, EntryPoint = "NCCoreSetFocusMode")]
        public static extern bool SetFocusMode (this int camera, int state);
        [DllImport(Assembly, EntryPoint = "NCCoreSetExposureMode")]
        public static extern bool SetExposureMode (this int camera, int state);
        [DllImport(Assembly, EntryPoint = "NCCoreSetFlash")]
        public static extern bool SetFlash (this int camera, int state);
        [DllImport(Assembly, EntryPoint = "NCCoreSetTorch")]
        public static extern bool SetTorch (this int camera, int state);
        [DllImport(Assembly, EntryPoint = "NCCoreSetZoom")]
        public static extern bool SetZoom (this int camera, float ratio);
        #endregion


        #else
        public static bool IsRearFacing (this int camera) {return true;}
        public static bool IsFlashSupported (this int camera) {return false;}
        public static bool IsTorchSupported (this int camera) {return false;}
        public static float HorizontalFOV (this int camera) {return 0;}
        public static float VerticalFOV (this int camera) {return 0;}
        public static float MinExposureBias (this int camera) {return 0;}
        public static float MaxExposureBias (this int camera) {return 0;}
        public static float MaxZoomRatio (this int camera) {return 1;}
        public static void GetPreviewResolution (this int camera, out int width, out int height) {width = height = 0;}
        public static void GetPhotoResolution (this int camera, out int width, out int height) {width = height = 0;}
        public static float GetFramerate (this int camera) {return 0;}
        public static float GetExposure (this int camera) {return 0;}
        public static int GetExposureMode (this int camera) {return 0;}
        public static int GetFocusMode (this int camera) {return 0;}
        public static int GetFlash (this int camera) {return 0;}
        public static int GetTorch (this int camera) {return 0;}
        public static float GetZoom (this int camera) {return 0;}
        public static void SetPreviewResolution (this int camera, int width, int height) {}
        public static void SetPhotoResolution (this int camera, int width, int height) {}
        public static void SetFramerate (this int camera, float framerate) {}
        public static bool SetFocus (this int camera, float x, float y) {return false;}
        public static float SetExposure (this int camera, float bias) {return 0;}
        public static bool SetFocusMode (this int camera, int state) {return false;}
        public static bool SetExposureMode (this int camera, int state) {return false;}
        public static bool SetFlash (this int camera, int state) {return false;}
        public static bool SetTorch (this int camera, int state) {return false;}
        public static bool SetZoom (this int camera, float ratio) {return false;}
        #endif
    }
}