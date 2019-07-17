/* 
*   NatCam Pro
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Pro {

    using Core.Utilities;

    #region --Delegates--
    /// <summary>
    /// A delegate type used to provide the path to saved media
    /// </summary>
    [ProDoc(210)]
	public delegate void SaveCallback (string path);
    #endregion


    #region --Value Types--

    /// <summary>
    /// A value type used to specify recording configuration settings
    /// </summary>
    [ProDoc(211)]
    public struct Configuration {

        #region --Op vars--

        [ProDoc(212)] public readonly int keyframeInterval;
        [ProDoc(213)] public readonly int bitrate;
        [ProDoc(214)] public readonly bool recordAudio;

        /// <summary>
        /// Default recording configuration
        /// </summary>
        [ProDoc(215)]
        public static readonly Configuration Default = new Configuration((int)(11.4f * 1280 * 720), 3, false);
        /// <summary>
        /// Default recording configuration with microphone audio
        /// </summary>
        public static readonly Configuration DefaultWithAudio = new Configuration(Default.bitrate, Default.keyframeInterval, true);
        #endregion


        #region --Client API--

        /// <summary>
        /// Create a recoridng configuration
        /// </summary>
        /// <param name="bitrate">Bitrate of the video encoder</param>
        /// <param name="keyframeInterval">Keyframe interval in seconds of the video encoder</param>
        /// <param name="recordAudio">Whether audio should be recorded from the microphone</param>
        [ProDoc(216)]
        public Configuration (int bitrate, int keyframeInterval, bool recordAudio) {
            this.bitrate = bitrate;
            this.keyframeInterval = keyframeInterval;
            this.recordAudio = recordAudio;
        }
        #endregion
    }
    #endregion
}