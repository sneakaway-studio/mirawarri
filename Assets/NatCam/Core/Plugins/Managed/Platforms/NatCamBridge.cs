/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System;
    using System.Runtime.InteropServices;

    public static partial class NatCamBridge {

        private const string CoreAssembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCam";
        #endif

        #region ---Delegates---
        public delegate void StartCallback (IntPtr texPtr, int width, int height);
        public delegate void PreviewCallback (IntPtr texPtr);
        public delegate void PhotoCallback (IntPtr imgPtr, int width, int height, int size);
        #endregion
        
        #if INATCAM_C

        #region --Operations--
        [DllImport(CoreAssembly, EntryPoint = "NCCoreRegisterCallbacks")]
        public static extern void RegisterCoreCallbacks (StartCallback startCallback,  PreviewCallback previewCallback, PhotoCallback photoCallback);
        [DllImport(CoreAssembly, EntryPoint = "NCCoreGetCamera")]
        public static extern int GetCamera ();
        [DllImport(CoreAssembly, EntryPoint = "NCCoreSetCamera")]
        public static extern void SetCamera (int camera);
        [DllImport(CoreAssembly, EntryPoint = "NCCoreIsPlaying")]
        public static extern bool IsPlaying ();
        [DllImport(CoreAssembly, EntryPoint = "NCCorePlay")]
        public static extern void Play ();
        [DllImport(CoreAssembly, EntryPoint = "NCCorePause")]
        public static extern void Pause ();
        [DllImport(CoreAssembly, EntryPoint = "NCCoreRelease")]
        public static extern void Release ();
        [DllImport(CoreAssembly, EntryPoint = "NCCoreCapturePhoto")]
        public static extern void CapturePhoto ();
        [DllImport(CoreAssembly, EntryPoint = "NCCoreReleasePhoto")]
        public static extern void ReleasePhoto ();
        [DllImport(CoreAssembly, EntryPoint = "NCCoreGetOrientation")]
        public static extern byte GetOrientation ();
        [DllImport(CoreAssembly, EntryPoint = "NCCoreSetOrientation")]
        public static extern void SetOrientation (byte orientation);
        #endregion

        #region --Utility--
        [DllImport(CoreAssembly, EntryPoint = "NCCoreOnPause")]
        public static extern void OnPause (bool paused);
        [DllImport(CoreAssembly, EntryPoint = "NCCoreHasPermissions")]
        public static extern bool HasPermissions ();
        [DllImport(CoreAssembly, EntryPoint = "NCCoreSetVerboseMode")]
        public static extern void SetVerboseMode (bool verbose);
        #endregion


        #else
        public static void RegisterCoreCallbacks (StartCallback startCallback,  PreviewCallback previewCallback, PhotoCallback photoCallback) {}
        public static int GetCamera () {return -1;}
        public static void SetCamera (int camera) {}
        public static bool IsPlaying () {return false;}
        public static void Play () {}
        public static void Pause () {}
        public static void Release () {}
        public static void CapturePhoto () {}
        public static void ReleasePhoto () {}
        public static byte GetOrientation () {return 0;}
        public static void SetOrientation (byte orientation) {}
        public static void OnPause (bool paused) {}
        public static bool HasPermissions () {return false;}
        public static void SetVerboseMode (bool verbose) {}
        #endif
    }
}