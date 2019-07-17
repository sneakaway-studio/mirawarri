/* 
*   NatCam Pro
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using Pro;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamAndroid {

        #region --Op vars--
        private SaveCallback recordingCallback;
        /// <summary>
        /// This flag determines if NatCam's preview data pipeline is enabled
        /// The preview data pipeline is what powers the PreviewBuffer, PreviewFrame, and PreviewMatrix API's
        /// On the Galaxy S7, there is a graphics bug that causes the preview data pipeline to lag considerably
        /// </summary>
        private const bool PreviewData = true;
        private readonly AndroidJavaObject pro;
        #endregion


        #region --Properties--
        public bool SupportsRecording { get { return true;}}
        public bool IsRecording { get { return pro.Call<bool>("isRecording");}}
        #endregion


        #region --Client API--

        public void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            ptr = IntPtr.Zero; width = height = size = 0;
            AndroidJavaObject jRet = pro.Call<AndroidJavaObject>("previewBuffer");
            if (jRet.GetRawObject().ToInt32() != 0) {
                long[] res = AndroidJNIHelper.ConvertFromJNIArray<long[]>(jRet.GetRawObject());
                ptr = (IntPtr)res[0];
                width = (int)res[1];
                height = (int)res[2];
                size = (int)res[3];
            } else Util.LogError("Failed to get preview buffer");
        }

        public void StartRecording (Configuration configuration, SaveCallback callback) {
            recordingCallback = callback;
            pro.Call("startRecording", configuration.bitrate, configuration.keyframeInterval, configuration.recordAudio);
        }

        public void StopRecording () {
            pro.Call("stopRecording");
        }
        #endregion


        #region --Callbacks--

        partial void onVideo (string path) {
            dispatch.Dispatch(() => recordingCallback(path));
        }
        #endregion
    }
}