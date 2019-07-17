/* 
*   NatCam Pro
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define OPENCV_API // Uncomment this to have access to the PreviewMatrix OpenCV API

namespace NatCamU.Core {

    using UnityEngine;
    using System;
    using Platforms;
    using Pro;
    using Utilities;
    using Util = Utilities.Utilities;
    
    #if OPENCV_API
    using OpenCVForUnity;
    #endif

    public static partial class NatCam {

        #if NATCAM_PRO || NATCAM_PROFESSIONAL

        #region --Properties--

        /// <summary>
        /// Is a video recording?
        /// </summary>
        [ProDoc(173)]
        public static bool IsRecording { get { return Implementation.IsRecording;}}
        #endregion


        #region --Operations--

        /// <summary>
        /// Read the current preview buffer
        /// </summary>
        /// <param name="ptr">Preview buffer handle</param>
        /// <param name="width">Preview buffer width</param>
        /// <param name="height">Preview buffer height</param>
        /// <param name="size">Preview buffer size in bytes</param>
        /// <returns>Was the preview buffer read?</returns>
        [ProDoc(174, 6), ProCode(26)]
        public static bool PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            Implementation.PreviewBuffer(out ptr, out width, out height, out size);
            return ptr != IntPtr.Zero && size > 0;
        }

        /// <summary>
        /// Read the current preview frame
        /// </summary>
        /// <param name="frame">Destination texture</param>
        /// <returns>Was the preview frame read?</returns>
        [ProDoc(171, 5), ProCode(24)]
        public static bool PreviewFrame (ref Texture2D frame) {
            IntPtr ptr; int width, height, size;
            if (!PreviewBuffer(out ptr, out width, out height, out size)) return false;
            const TextureFormat format =
            #if UNITY_IOS && !UNITY_EDITOR
            TextureFormat.BGRA32;
            #else
            TextureFormat.RGBA32;
            #endif
            if (frame && (frame.width != width || frame.height != height)) {
                Texture2D.Destroy(frame);
                frame = null;
            }
            frame = frame ?? new Texture2D(width, height, format, false, false);
            frame.LoadRawTextureData(ptr, size); frame.Apply();
            return true;
        }

        #if OPENCV_API
        /// <summary>
        /// Read the current preview frame as an OpenCV Matrix
        /// </summary>
        /// <param name="matrix">Destination matrix</param>
        /// <returns>Was the preview matrix read?</returns>
        [ProDoc(172), ProCode(25)]
        public static bool PreviewMatrix (ref Mat matrix) {
            IntPtr ptr; int width, height, size;
            if (!PreviewBuffer(out ptr, out width, out height, out size)) return false;
            if (matrix != null && (matrix.cols() != width || matrix.rows() != height)) {
                matrix.release();
                matrix = null;
            }
            matrix = matrix ?? new Mat(height, width, CvType.CV_8UC4);
            Utils.copyToMat(ptr, matrix);
            //Core.flip (matrix, matrix, 0); // Dev should do this themselves
            return true;
        }
        #endif

        /// <summary>
        /// Start recording a video
        /// </summary>
        /// <param name="configuration">Configuration for video recording</param>
        /// <param name="callback">The callback to be invoked when the video is saved</param>
        [ProDoc(175), ProCode(27)]
        public static void StartRecording (Configuration configuration, SaveCallback callback) {
            if (!Implementation.SupportsRecording) {
                Util.LogError("Cannot record video because implementation does not support recording");
                return;
            } if (!IsPlaying) {
                Util.LogError("Cannot record video when session is not running");
                return;
            } if (callback == null) {
                Util.LogError("Cannot record video without callback");
                return;
            }
            Implementation.StartRecording(configuration, callback);
        }

        /// <summary>
        /// Stop recording a video
        /// </summary>
        [ProDoc(176)]
        public static void StopRecording () {
            if (!Implementation.SupportsRecording) {
                Util.LogError("Cannot stop recording video because implementation does not support recording");
                return;
            }
            Implementation.StopRecording();
        }
        #endregion

        #endif
    }
}