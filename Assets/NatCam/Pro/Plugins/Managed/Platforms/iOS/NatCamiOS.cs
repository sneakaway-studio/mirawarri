/* 
*   NatCam Pro
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using AOT;
    using System;
    using System.Runtime.InteropServices;
    using Dispatch;
    using Pro;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamiOS {

        #region --Op vars--
        private SaveCallback recordingCallback;
        #endregion


        #region --Properties--
        public bool SupportsRecording { get { return true;}}
        public bool IsRecording { get { return NatCamBridge.IsRecording();}}
        #endregion


        #region --Client API--

        public void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            NatCamBridge.PreviewBuffer(out ptr, out width, out height, out size);
        }

        public void StartRecording (Configuration configuration, SaveCallback callback) {
            recordingCallback = callback;
            NatCamBridge.StartRecording(configuration.bitrate, configuration.keyframeInterval, configuration.recordAudio);
        }

        public void StopRecording () {
            NatCamBridge.StopRecording();
        }
        #endregion


        #region --Callbacks--

        [MonoPInvokeCallback(typeof(SaveCallback))]
        private static void OnVideo (string path) {
            using (var dispatch = new MainDispatch()) dispatch.Dispatch(() => instance.recordingCallback(path));
        }
        #endregion
    }
}