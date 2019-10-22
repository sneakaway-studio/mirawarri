/* 
*   NatCam
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCam.Examples {

	using UnityEngine;
	using UnityEngine.UI;
	using System;

	/*
	* GreyCam Example
	* Example showcasing NatCam Preview Data Pipeline
	* Make sure to run this on the lowest camera resolution as it is heavily computationally expensive
	*/
	public class GreyCam : MonoBehaviour {

		[Header("Camera")]
		public bool useFrontCamera;

		[Header("UI")]
		public RawImage rawImage;
		public AspectRatioFitter aspectFitter;

		private CameraDevice deviceCamera;
		private Texture2D previewTexture, greyPreviewTexture;

		void Start () {
			// Check permission
			var cameras = CameraDevice.GetDevices();
			if (cameras == null) {
				Debug.Log("User has not granted camera permission");
				return;
			}
			// Pick camera
			CameraDevice deviceCamera = null;
			foreach (var camera in cameras)
				if (camera.IsFrontFacing == useFrontCamera) {
					deviceCamera = camera;
					break;
				}
			if (!deviceCamera) {
                Debug.LogError("Camera is null. Consider using " + (useFrontCamera ? "rear" : "front") + " camera");
                return;
            }
			// Start preview
			deviceCamera.PreviewResolution = new Resolution { width = 640, height = 480 };
			deviceCamera.StartPreview(OnStart, OnFrame);
		}
		
		void OnStart (Texture2D preview) {
			// Create texture
			this.previewTexture = preview;
			rawImage.texture =
			this.greyPreviewTexture = new Texture2D(preview.width, preview.height, TextureFormat.RGBA32, false, false);
			// Scale the panel to match aspect ratios
            aspectFitter.aspectRatio = preview.width / (float)preview.height;
		}

		void OnFrame (long timestamp) {
			// Convert to greyscale
			var pixelBuffer = previewTexture.GetRawTextureData();
			ConvertToGrey(pixelBuffer);
			// Fill the texture with the greys
			greyPreviewTexture.LoadRawTextureData(pixelBuffer);
			greyPreviewTexture.Apply();
		}

		static void ConvertToGrey (byte[] buffer) {
			for (int i = 0; i < buffer.Length; i += 4) {
				byte
				r = buffer[i + 0], g = buffer[i + 1],
				b = buffer[i + 2], a = buffer[i + 3],
				// Use quick luminance approximation to save time and memory
				l = (byte)((r + r + r + b + g + g + g + g) >> 3);
				buffer[i] = buffer[i + 1] = buffer[i + 2] = l; buffer[i + 3] = a;
			}
		}
	}
}