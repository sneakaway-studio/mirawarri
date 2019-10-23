/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Examples
{

    using UnityEngine;
    using UnityEngine.UI;
    using Core;
    using Core.UI;
    using Pro;
    using System;
    using System.Collections;

    public class OwenMiniCam1_6 : NatCamBehaviour
    {

        [Header("UI")]
        public NatCamPreview panel;
        public NatCamFocuser focuser;
        public Text flashText;
        //      public Button switchCamButton, flashButton;
        // public Image checkIco, flashIco;
        protected Texture2D photo;
        public FocusMode focusMode = FocusMode.AutoFocus | FocusMode.TapToFocus;


        #region --Unity Messages--

        public Text NeedsPhotoPermission;
        int cameraPermission = 0;



        // Use this for initialization
        public override void Start()
        {
            Debug.Log("OWEN: OwenMiniCam: Start() called");

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                // set UI Text photo prompt active
                NeedsPhotoPermission.gameObject.SetActive(true);
                // request camera use
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            else
            {
                // hide prompt
                NeedsPhotoPermission.gameObject.SetActive(false);
            }

            // if we don't have permission, wait until we do 
            while (cameraPermission != 1)
            {
                // wait a sec
                System.Threading.Thread.Sleep(2000);
                // if user enables preference
                if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                {
                    // hide UI Text photo prompt 
                    NeedsPhotoPermission.gameObject.SetActive(false);
                    // proceed to start app
                    cameraPermission = 1;
                }
            }


#if UNITY_ANDROID
            SetRearAndroidResolution();
#endif



            // Start base
            base.Start();
        }
        #endregion


        // only run this on rear camera on Android, didn't have the trouble on iOS
        private void SetRearAndroidResolution()
        {
            if (NatCam.Camera != DeviceCamera.RearCamera) return;
            NatCam.Camera = DeviceCamera.RearCamera;
            //			Debug.Log ("##### INITIAL camera resolution = " + NatCam.Camera.PhotoResolution + " camera facing = " + NatCam.Camera.Facing);



            //			Debug.Log ("##### CALLING THIS: NatCam.Camera.SetPreviewResolution(1920, 1080);");
            //			Debug.Log ("##### THEN THIS: NatCam.Camera.SetPhotoResolution(ResolutionPreset.FullHD);");

            // Set the preview resolution to Full HD
            NatCam.Camera.SetPreviewResolution(1920, 1080);
            // Set the photo resolution to highest
            NatCam.Camera.SetPhotoResolution(ResolutionPreset.FullHD);


            // BELOW DOESN'T WORK, ONLY THE ABOVE WORKS. FUCK IT. LEAVING AT FullHD

            /*
			// horizontal, so change resolution -> if res is 1280:720 then 720 * (720 / 1280)
			// Android (Samsung Galaxy A Tablet) rear = 2592, 1944 (.75)
			int w = 0, h = 0;
			if (NatCam.Camera.PhotoResolution.x > NatCam.Camera.PhotoResolution.y) {
				// 1944 * (1944 / 2592) = 
				w = (int)(NatCam.Camera.PhotoResolution.y * (NatCam.Camera.PhotoResolution.y / NatCam.Camera.PhotoResolution.x));
				h = (int)(NatCam.Camera.PhotoResolution.y);
			}
			Debug.Log ("CHANGE resolution = " + w + "," + h);
			//NatCam.Camera.SetPhotoResolution(ResolutionPreset.Lowest);// test
			NatCam.Camera.SetPhotoResolution(h,w);
			*/

            //			Debug.Log ("##### AFTER CALL camera resolution = " + NatCam.Camera.PhotoResolution + " camera facing = " + NatCam.Camera.Facing);

        }


        private void OwenReport()
        {
            string report = "";
            // details about the device
            report += "PhotoResolution = " + NatCam.Camera.PhotoResolution;
            report += "; PreviewResolution = " + NatCam.Camera.PreviewResolution;
            report += "; Facing = " + NatCam.Camera.Facing;
            report += "; FlashMode = " + NatCam.Camera.FlashMode;
            report += "; FocusMode = " + NatCam.Camera.FocusMode;
            report += "; ZoomRatio = " + NatCam.Camera.ZoomRatio;
            //report += "; Canvas.scaleFactor = " + canvas.scaleFactor;
            //report += "; Canvas.pixelRect = " + canvas.pixelRect;
            Debug.Log(report);
        }



        #region --NatCam and UI Callbacks--

        public override void OnStart()
        {
            // Display the preview
            panel.Apply(NatCam.Preview);
            // Start tracking focus gestures
            //focuser.StartTracking();
            var f = FocusMode.TapToFocus | FocusMode.AutoFocus;
            focuser.StartTracking(f);

            //OwenReport ();
        }


        public virtual void CapturePhoto()
        {
            // Divert control if we are checking the captured photo
            //if (!checkIco.gameObject.activeInHierarchy) NatCam.CapturePhoto(OnPhoto);
            // Check captured photo
            //else OnViewPhoto();

            Debug.Log("CapturePhoto() called, NatCam.Camera.PreviewResolution = " + NatCam.Camera.PreviewResolution);
            NatCam.CapturePhoto(OnPhoto);
        }

        protected virtual void OnPhoto(Texture2D photo, Orientation orientation)
        {


            Debug.Log("OnPhoto() called, NatCam.Camera.PreviewResolution = " + NatCam.Camera.PreviewResolution);

            // Cache the photo
            this.photo = photo;
            // Display the photo
            panel.Apply(photo, orientation, Core.UI.ScaleMode.ScaleWidth);


            Debug.Log("OnPhoto() called (post panel.Apply), NatCam.Camera.PreviewResolution = " + NatCam.Camera.PreviewResolution);

            // Enable the check icon
            //checkIco.gameObject.SetActive(true);
            // Disable the switch camera button
            //switchCamButton.gameObject.SetActive(false);
            // Disable the flash button
            //flashButton.gameObject.SetActive(false);
        }
        #endregion


        #region --UI Ops--

        public void SwitchCamera()
        {
            //Debug.Log ("SwitchCamera ()");
            //Switch camera
            base.SwitchCamera();
            //Set the flash icon
            //          SetFlashIcon();

#if UNITY_ANDROID
            SetRearAndroidResolution();
#endif
        }

        public void ToggleFlashMode()
        {
            //Set the active camera's flash mode
            NatCam.Camera.FlashMode = NatCam.Camera.IsFlashSupported ? NatCam.Camera.FlashMode == FlashMode.Auto ? FlashMode.On : NatCam.Camera.FlashMode == FlashMode.On ? FlashMode.Off : FlashMode.Auto : NatCam.Camera.FlashMode;
            //Set the flash icon
            //          SetFlashIcon();
        }

        public void ToggleTorchMode()
        {
            //Set the active camera's torch mode
            //			NatCam.Camera.TorchMode = NatCam.Camera.TorchMode == Switch.Off ? Switch.On : Switch.Off;
        }

        void OnViewPhoto(Texture2D photo, Orientation orientation)
        {
            // Disable the check icon
            //checkIco.gameObject.SetActive(false);
            // Display the preview
            panel.Apply(NatCam.Preview, orientation, Core.UI.ScaleMode.ScaleWidth);
            // Enable the switch camera button
            //          switchCamButton.gameObject.SetActive(true);
            // Enable the flash button
            //          flashButton.gameObject.SetActive(true);
            // Free the photo texture
            Texture2D.Destroy(photo); photo = null;
        }
        /*
		void SetFlashIcon () {
			//Null checking
			if (!NatCam.Camera) return;
			//Set the icon
			flashIco.color = !NatCam.Camera.IsFlashSupported || NatCam.Camera.FlashMode == FlashMode.Off ? (Color)new Color32(120, 120, 120, 255) : Color.white;
			//Set the auto text for flash
			flashText.text = NatCam.Camera.IsFlashSupported && NatCam.Camera.FlashMode == FlashMode.Auto ? "A" : "";
		}
		*/
        #endregion
    }
}







/*

using UnityEngine;
using UnityEngine.UI;
using NatCamU.Core;
using NatCamU.Extended;
using NatCamU.Core.UI;
using System.Collections.Generic;
using CompositorU;

namespace NatCamU.Examples {

	public class OwenMiniCam : NatCamBehaviour {
        
		[Header("UI")]
		public NatCamPreview panel;
        public NatCamFocuser focuser;
		public UnityEngine.UI.Text flashText;
        public Button switchCamButton, flashButton;
		public Image checkIco, flashIco;
		private Texture2D photo;


		#region --Owen: Setup--
		[Header("Save Photo")]
		public Orientation orientation;
		public SaveMode saveMode = SaveMode.SaveToPhotoGallery;
		public GameObject prefabSprite;								// test sprite to composite
		List<GameObject> prefabSprites = new List<GameObject>(); 	// track layers added
		private string report = "";
		public Canvas canvas;
		private GameObject rawImageTest;
		public enum App { capturePhoto, editPhoto, savePhoto, sharePhoto }
		public App appState = App.capturePhoto; 
		#endregion



        #region --Unity Messages--

        // Use this for initialization
		public override void Start () {
			base.Start();	// Start base
			SetFlashIcon();	// Set the flash icon
			OwenSetup ();
        }
        #endregion



		#region --Functions added by Owen--

		private void OwenSetup(){
			rawImageTest = GameObject.Find ("RawImage");

			// details about the device
			report += "PhotoResolution = " + NatCam.Camera.PhotoResolution;
			report += "; PreviewResolution = " + NatCam.Camera.PreviewResolution;
			report += "; Facing = " + NatCam.Camera.Facing;
			report += "; FlashMode = " + NatCam.Camera.FlashMode;
			report += "; FocusMode = " + NatCam.Camera.FocusMode;
			report += "; ZoomRatio = " + NatCam.Camera.ZoomRatio;
			report += "; Canvas.scaleFactor = " + canvas.scaleFactor;
			report += "; Canvas.pixelRect = " + canvas.pixelRect;
			Debug.Log (report);
		}

		// Main button pressed, what happens depends on the mode
		public void CaptureButton () {
			if (appState == App.capturePhoto){
				CapturePhoto ();
				SetState (App.editPhoto);
			} else if (appState == App.editPhoto){
				CreateComposite(photo,orientation,prefabSprites);
				SetState (App.capturePhoto);
				OnViewPhoto ();
			}
		}

		// set visible / functions per mode
		void SetState(App newState){
			Debug.Log ("<color=blue>SetState(" + newState + ")</color>");
			appState = newState; // update state
			CleanUpLayers();
		}

		// remove all layers in prefabSprites from scene
		private void CleanUpLayers(){
			if (prefabSprites.Count > 0){
				foreach (var layer in prefabSprites) {
					Destroy(layer);		// destroy layer in scene
				}
				prefabSprites.Clear ();	// remove all references in the List
			}
		}
		#endregion




        
        #region --NatCam and UI Callbacks--
		public override void OnStart () {
			panel.Apply(NatCam.Preview);// Display the preview	
			focuser.StartTracking();	// Start tracking focus gestures
        }
        
		public void CapturePhoto () {
			Debug.Log ("CapturePhoto() called");
            // Divert control if we are checking the captured photo
            if (!checkIco.gameObject.activeInHierarchy) NatCam.CapturePhoto(OnPhoto);
            // Check captured photo
            else OnViewPhoto();
        }
        
        void OnPhoto (Texture2D photo, Orientation _orientation) {
			Debug.Log ("OnPhoto() called");
			orientation = _orientation;  		// keep a global orientation
			Debug.Log ("(1) orientation = " + orientation + " // photo = " + photo.width +" x "+ photo.height);
			this.photo = photo;					// Cache the photo
			panel.Apply(photo, orientation);	// Display the photo

			// original things
			checkIco.gameObject.SetActive(true);		// Enable the check icon
			switchCamButton.gameObject.SetActive(false);// Disable the switch camera button
			flashButton.gameObject.SetActive(false);	// Disable the flash button
		}
		#endregion




		#region --Owen: Edit & Save Image

		void Update() {

			if (appState == App.capturePhoto) {
				return;	// do nothing
			}
			else if (appState == App.editPhoto) {
				// add stickers to screen
				if (Input.GetMouseButtonUp (0) && Input.mousePosition.y > 140) {

					Vector3 mousePos = Input.mousePosition;
					mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0.0f);

					// transform mousePosition to screen point
					Vector3 posWorld = Camera.main.ScreenToWorldPoint (mousePos);

					// instantiate new sprite
					GameObject pfs = Instantiate (prefabSprite, posWorld, Quaternion.identity);

					// attach it to the panel
					//pfs.transform.parent = panel.transform;

					// add to its sorting order to make sure it appears above the background
					pfs.GetComponent<SpriteRenderer> ().sortingOrder += 1;

					// add to list
					prefabSprites.Add (pfs);

					Debug.Log (""+
						"Input.mousePosition " + Input.mousePosition + 
						" // mousePos = " + mousePos +
						" // posWorld " + posWorld + 
						" // instantiated pos " + pfs.transform.position.x +" x "+ pfs.transform.position.y +" x "+ pfs.transform.position.z +
						" // total: " + prefabSprites.Count);
				}
			}
		}

		public void CreateComposite(Texture2D bgTexture, Orientation orientation, List<GameObject> prefabSprites){
			Debug.Log ("<color=red>CreateComposite() called w/ "+ prefabSprites.Count +" sprites</color>");
			if (prefabSprites.Count == 0) return;	// return if there are no layers

			// Create a compositor
			using (var compositor = new RenderCompositor (bgTexture.width, bgTexture.height)) {
				Debug.Log ("(2) orientation = " + orientation + " // bgTexture = " + bgTexture.width +" x "+ bgTexture.height);

				Texture2D bgTextureCorrectOrientation;

				// Add background photo first because it determines size. 
				// But first, look to see what orientation we received from the device. 
				// *iPhone returns a photo rotated 90 degrees  
				if (orientation == Orientation.Rotation_90) {
					Debug.Log ("Orientation = " + orientation + "... making adjustments");

					// same size but different orientation as actual bgTexture
					bgTextureCorrectOrientation = new Texture2D (bgTexture.height, bgTexture.width);

					// Let Compositor rotate for us by adding a "spoof" texture in the first layer 
					// with the orientation rotated (bgTexture width / height are switched).
					compositor.AddLayer (new Layer (bgTextureCorrectOrientation, Vector2.zero, 0f, Vector2.one));

					// Then add actual bgTexture with rotation
					compositor.AddLayer (new Layer (bgTexture, new Point (-280, 280), 270f, Vector2.one));
				} else {
					// for below references
					bgTextureCorrectOrientation = new Texture2D (bgTexture.width, bgTexture.height);

					// add bgTexture with no rotation
					compositor.AddLayer (new Layer (bgTexture, Vector2.zero, 0f, Vector2.one));
				}



				// now figure out position



				Debug.Log (
					"bgTextureCorrectOrientation = " + bgTextureCorrectOrientation.width +" x "+ bgTextureCorrectOrientation.height +
					" // bgTexture = " + bgTexture.width +" x "+ bgTexture.height +
					" // Screen = " + Screen.width +" x "+ Screen.height
				);

				float ratio = 0f;		// the ratio to determine new position for layer


				// if bgTexture is horizontal
				if (bgTextureCorrectOrientation.width >= bgTextureCorrectOrientation.height) {
					// if bgTexture is larger than the screen
					if (bgTextureCorrectOrientation.width >= Screen.width) {
						Debug.Log ("bgTextureCorrectOrientation.width >= Screen.width");
						ratio = (float)(bgTextureCorrectOrientation.width / Screen.width);
					} else {
						Debug.Log ("bgTextureCorrectOrientation.width < Screen.width");
						// iphone 
						ratio = (float)Screen.width / (float)bgTextureCorrectOrientation.width;
					}
				} else {}



				// only for mac
				#if (UNITY_EDITOR)
				Debug.Log ("my mac");
				ratio = .75f; // should == .75f
				#endif



				// calc offsets, e.g. 1280 - 640
				float horzOffset = (float)( (bgTextureCorrectOrientation.width / 2) - (Screen.width / 2));
				float vertOffset = (float)( (bgTextureCorrectOrientation.height / 2) - (Screen.height / 2));


				Debug.Log ("horzOffset: " + horzOffset + " // vertOffset = "+ vertOffset + " // ratio = "+ ratio );







				int i = 0;					// current layer
				Texture2D currentTexture;	// current sprite texture


				Vector2 layerWorldPosition = new Vector2();			// layer world position
				Vector2 layerScreenPosition = new Vector2();		// layer world position converted to screen
				Vector2 layerViewportPosition = new Vector2();		// layer world position converted to viewport
				Vector2 layerCorrectedPosition = new Vector2();		// layer adjusted position


				// Add layers (added through clicking/instantiation)
				foreach (var layer in prefabSprites) {

					// get texture from GameObject SpriteRenderer
					currentTexture = layer.GetComponent<SpriteRenderer>().sprite.texture;


					// get layer position(s)
					layerWorldPosition = layer.transform.position;
					layerScreenPosition = Camera.main.WorldToScreenPoint(layerWorldPosition);
					layerViewportPosition = Camera.main.ScreenToViewportPoint(layerScreenPosition);
					//layerScreenPosition = new Vector2 (40, 70); // test




					// normalize screen sizes (same as viewport position)
					float normalizedX = (float)(layerScreenPosition.x / Screen.width);
					float normalizedY = (float)(layerScreenPosition.y / Screen.height);



					// (original X screen position) - (1/2 the width of image texture)
					layerCorrectedPosition.x = (layerScreenPosition.x) - (currentTexture.width / 2);
					// ^ this works (on mac anyway), but trying normalized instead
					layerCorrectedPosition.x = (normalizedX * Screen.width) - (currentTexture.width / 2);



					// (original Y screen position * ratio) - (1/2 the height of image texture)
					layerCorrectedPosition.y = (layerScreenPosition.y * ratio) - (currentTexture.height / 2); // perfect!!!!
					// ^ this works (on mac anyway), but trying normalized instead
					layerCorrectedPosition.y = (normalizedY * Screen.height) - (currentTexture.height / 2);


		


					#if (UNITY_EDITOR)
					layerCorrectedPosition.x += horzOffset;
					layerCorrectedPosition.y += vertOffset;
					#endif


				



					// set layer rotation
					float layerRotation = 0f;

					// set layer size
					Vector2 layerSize = Vector2.one;

					// add layer to compositor
					compositor.AddLayer (new Layer (currentTexture, layerCorrectedPosition, layerRotation, layerSize));

					Debug.Log ("layer #"+ i +". " + currentTexture.width +","+ currentTexture.height +
						" // layerWorldPosition: " + layerWorldPosition + 
						" // layerScreenPosition: " + Mathf.Round(layerScreenPosition.x) +" x "+ Mathf.Round(layerScreenPosition.y) +
						" // layerViewportPosition: " + Mathf.Round(layerViewportPosition.x * 100f) / 100f +" x "+ Mathf.Round(layerViewportPosition.y * 100f) / 100f +
						" // normalizedW/H: " + Mathf.Round(normalizedX * 100f) / 100f +" x "+ Mathf.Round(normalizedY * 100f) / 100f +
						//" // layerRotation: " + layerRotation + " // layerSize: " + layerSize.x +" x "+ layerSize.y +
						" // layerCorrectedPosition: " + Mathf.Round(layerCorrectedPosition.x) +" x "+ Mathf.Round(layerCorrectedPosition.y) 
					);

					i++; // just a counter
				}

				// call Composite and display result
				// compositor.Composite (result => rawImageTest.GetComponent<RawImage> ().texture = result);

				// call Composite with callback function
				compositor.Composite(OnComposite);
			}
		}

		void OnComposite (Texture2D result) {
			Debug.Log ("OnComposite() called with a "+ result.width +" x "+ result.height +" sprite");

			// test: display the result in a raw image
			rawImageTest.GetComponent<RawImage> ().texture = result;

			photo = result;
			Save ();
		}

		private void Save(){
			//Debug.Log ("Save() called");
			#if (!UNITY_EDITOR)
			saveMode = SaveMode.SaveToPhotoGallery;
			#endif
			NatCam.SavePhoto(photo, saveMode, Orientation.Rotation_0, OnSave);
		}
		private void OnSave (SaveMode mode, string path) {
			Debug.Log ("Photo saved with " + mode + " to path: " + path);
		}
        #endregion
        
        




        #region --UI Ops--
        
        public void SwitchCamera () {
			base.SwitchCamera();//Switch camera
			SetFlashIcon();		//Set the flash icon
        }
        
        public void ToggleFlashMode () {
            //Set the active camera's flash mode
            NatCam.Camera.FlashMode = NatCam.Camera.IsFlashSupported ? NatCam.Camera.FlashMode == FlashMode.Auto ? 
				FlashMode.On : NatCam.Camera.FlashMode == FlashMode.On ? FlashMode.Off : FlashMode.Auto : NatCam.Camera.FlashMode;
            //Set the flash icon
            SetFlashIcon();
        }

        public void ToggleTorchMode () {
            //Set the active camera's torch mode
            NatCam.Camera.TorchMode = NatCam.Camera.TorchMode == Switch.Off ? Switch.On : Switch.Off;
        }
        
		void OnViewPhoto () {
			Debug.Log ("OnViewPhoto() called");

			checkIco.gameObject.SetActive(false);		// Disable the check icon
			panel.Apply(NatCam.Preview);				// Display the preview
			switchCamButton.gameObject.SetActive(true);	// Enable the switch camera button
			flashButton.gameObject.SetActive(true);		// Enable the flash button
			//Texture2D.Destroy(photo); photo = null;		// Free the photo texture
        }
        
        void SetFlashIcon () {
            //Null checking
            if (!NatCam.Camera) return;
            //Set the icon
            flashIco.color = !NatCam.Camera.IsFlashSupported || NatCam.Camera.FlashMode == FlashMode.Off ? (Color)new Color32(120, 120, 120, 255) : Color.white;
            //Set the auto text for flash
            flashText.text = NatCam.Camera.IsFlashSupported && NatCam.Camera.FlashMode == FlashMode.Auto ? "A" : "";
        }
        #endregion
    }
}

*/
