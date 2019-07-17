/* 
*   NatCam Pro
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System;
    using System.Runtime.InteropServices;
    using Pro;

    public static partial class NatCamBridge {

        private const string ProAssembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCamPro";
        #endif

        #region --Memory--

        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        [DllImport("libSystem")]
        #elif UNITY_STANDALONE_LINUX
        [DllImport("libc.so")]
        #elif UNITY_ANDROID
        [DllImport("c")]
        #elif UNITY_IOS
        [DllImport("__Internal", EntryPoint = "NCPromemcpy")] // libNatCamProfessional.a
        #endif
        public static extern IntPtr memcpy (IntPtr dest, IntPtr src, UIntPtr size);
        #endregion

        #if INATCAM_C

        [DllImport(ProAssembly, EntryPoint = "NCProRegisterCallbacks")]
        public static extern void RegisterProCallbacks (SaveCallback saveCallback);

        #region --PreviewBuffer--
        [DllImport(ProAssembly, EntryPoint = "NCProPreviewBuffer")]
        public static extern void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size);
        [DllImport(ProAssembly, EntryPoint = "NCProReleaseBuffer")]
        public static extern void ReleaseBuffer ();
        #endregion

        #region --Recording--
        [DllImport(ProAssembly, EntryPoint = "NCProStartRecording")]
        public static extern void StartRecording (int bitrate, int keyframes, bool audio);
        [DllImport(ProAssembly, EntryPoint = "NCProStopRecording")]
        public static extern void StopRecording ();
        [DllImport(ProAssembly, EntryPoint = "NCProIsRecording")]
        public static extern bool IsRecording ();
        #endregion


        #else
        public static void RegisterProCallbacks (SaveCallback saveCallback) {}
        public static void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {width = height = size = 0; ptr = (IntPtr)0;}
        public static void ReleaseBuffer () {}
        public static void StartRecording (int bitrate, int keyframes, bool audio) {}
        public static void StopRecording () {}
        public static bool IsRecording () {return false;}
        #endif
    }
}