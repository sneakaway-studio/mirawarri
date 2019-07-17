/* 
*   NatCam Pro
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Examples {
	
	using UnityEngine;
	using NatCamU.Core;
	using NatCamU.Pro;

	public class ReplayCam : NatCamBehaviour {

		#if NATCAM_PRO || NATCAM_PROFESSIONAL

		public void StartRecording () {
			// Start recording
			NatCam.StartRecording(Configuration.Default, OnVideo);
		}

		public void StopRecording () {
			// Stop recording // The OnVideo callback will then be invoked with the video path
			NatCam.StopRecording();
		}

		private void OnVideo (string path) {
			// Play the video
			Handheld.PlayFullScreenMovie(path);
		}
		#endif
	}
}