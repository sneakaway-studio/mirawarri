/* 
*   NatCam Pro
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    using Pro;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamLegacy {

        #region --Properties--
        public bool SupportsRecording { get { return false;}}
        public bool IsRecording { get { return false;}}
        #endregion


        #region --Op vars--
        private Color32[] buffer;
        private IntPtr handle;
        #endregion


        #region --Client API--

        public void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            width = preview.width; height = preview.height;
            if (buffer != null && buffer.Length != width * height) ReleaseBuffer();
            if (buffer == null) InitializeBuffer();
            preview.GetPixels32(buffer);
            size = buffer.Length * Marshal.SizeOf(typeof(Color32));
            GCHandle pin = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            NatCamBridge.memcpy(handle, pin.AddrOfPinnedObject(), (UIntPtr)size);
            pin.Free();
            ptr = handle;
        }

        public void StartRecording (Configuration configuration, SaveCallback callback) {
            Util.LogError("Recording is not supported on legacy");
        }

        public void StopRecording() {
            Util.LogError("Recording is not supported on legacy");
        }
        #endregion


        #region --Operations--

        private void InitializeBuffer () {
            buffer = new Color32[preview.width * preview.height];
            var size = buffer.Length * Marshal.SizeOf(typeof(Color32));
            handle = Marshal.AllocHGlobal(size);
            GC.AddMemoryPressure(size);
        }

        private void ReleaseBuffer () {
            if (handle == IntPtr.Zero) return;
            Marshal.FreeHGlobal(handle);
            GC.RemoveMemoryPressure(buffer.Length * Marshal.SizeOf(typeof(Color32)));
            handle = IntPtr.Zero;
            buffer = null;
        }
        #endregion
    }
}