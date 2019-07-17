/* 
*   NatCam Pro
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System;
    using System.Runtime.InteropServices;
    using Pro;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamStandalone {

        #region --Properties--
        public bool SupportsRecording { get { return true;}}
        public bool IsRecording { get { return NatCamBridge.IsRecording();}}
        #endregion


        #region --Client API--

        public void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            NatCamBridge.PreviewBuffer(out ptr, out width, out height, out size);
        }

        public void StartRecording (Configuration configuration, SaveCallback callback) {

        }

        public void StopRecording () {

        }
        #endregion
    }
}