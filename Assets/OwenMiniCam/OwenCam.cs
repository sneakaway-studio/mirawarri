using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using NatCamU;
using NatCamU.Examples;
using NatCamU.Core;
using NatCamU.Core.UI;
using NatCamU.Pro;
using CompositorU;
using DigitalRubyShared; // Fingers
using UnityEngine.Analytics;

public class OwenCam : OwenMiniCam /*, IPointerUpHandler*/ {

	private static bool DEBUG = false;

	[Header("Prefabs and Canvas")]
	public GameObject stickerPrefab;
	public GameObject paintingStickerPrefab;
	public Canvas canvas;

	private ICompositor compositor;
	private List<GameObject> layers = new List<GameObject>();

	private Texture2D tempTexture;

	[Header("Buttons")]
	public Button aboutPageButton;
	// capture buttons
	public Button captureFrameMenuButton;
	public Button switchCameraButton;
	public Button photoCaptureButton;
	public Button selectGalleryButton;
	// crop buttons
	public Button cropPhotoButton;
	public Button cropExitButton;
	// edit buttons
	public Button editFrameMenuButton;
	public Button photoSaveButton;
	public Button stickerMenuButton;
	public Button paintingStickerMenuButton;
	public Button deleteStickersButton;
	// other buttons
	public Button photoExitButton;
	public Button stickerRandButton;
	public Button flashButton;

	public Mode mode;
	public enum Mode {  Init, 			// when the app loads, before camera starts
						CapturePhoto, 	// camera mode, preview is visible
						CropPhoto, 		// after photo captured, 
						EditPhoto, 		// after photo cropped, show edit options
						SharePhoto, 	// show dialog for saving/sharing
						CompositePhoto 	// calls composite 
	};

	/*
	PERMISSIONS DIALOG TEXTS

		IOS
			Gallery Permission: This is required to import or save images with the app.



	*/

	// stickers
	[Header("Stickers")]
	public int currentSticker;		// current sticker to display
	public GameObject stickersPlaced;// game object parent of dynamically added stickers
	public GameObject stickerMenu;	// stickerMenu gameobject
	bool stickerMenuVisible;	// whether stickerMenu should be visible

	// "painting" stickers
	[Header("Painting Stickers")]
	public int currentPaintingSticker;		// current sticker to display
	public GameObject paintingStickersPlaced;// game object parent of dynamically added stickers
	public GameObject paintingStickerMenu;	// stickerMenu gameobject
	bool paintingStickerMenuVisible;	// whether stickerMenu should be visible
	public bool paintingStickerMode = false; // whether or not we can paint

	// frames
	[Header("Frames")]
	public int currentFrame = 0;	// current frame to display
	public GameObject frame;		// Frame gameobject
	public GameObject frameMenu;	// frameMenu gameobject
	bool frameMenuVisible;	// whether frameMenu should be visible

	[Header("Pages")]
	// About Page
	public GameObject aboutPage;
	bool aboutPageVisible;
	// moving photos on about page
	public GameObject aboutPagePhotos;
	RectTransform aboutPagePhotosRT;
	int startPosition = -2248;
	int loopPosition = -200;
	Coroutine co;
	bool coRunning;

	// Share Page
	public GameObject sharePage;
	bool sharePageVisible; 
	bool imageSaved;
	public int numberOfSaves = 0;							// number of times they've saved
	public int hasChosenToSurvey = 0;						// whether or not they have chose to take survey (beta testers)
	public int hasChosenToRate = 0;							// whether or not they have chose to rate
	public int hasReceivedSaveImageWarning = 0; 			// a warning that reminds them what the save button does
	public int hasReceivedSavePermissionsNotification = 0; 	// have they received gallery permissions notification?

	[Header("Cropping")]
	public float aspectRatio = 0.5625f;
	public AspectRatioFitter imageFitter;
	public int photoCropped = 0;
	public Slider cropSlider;

	public void SetMode(Mode _mode){
		mode = _mode;
		//if (DEBUG) Debug.Log ("SetMode() -> mode = " + mode);

		if (mode == Mode.Init) {
			// reset aspect ratio and fitter
			imageFitter.aspectRatio = 0.5625f;
			preview.uvRect = new Rect(0, 0, 1, 1);
			// get prefs
			numberOfSaves = PlayerPrefs.GetInt ("numberOfSaves", 0);
			hasChosenToSurvey = PlayerPrefs.GetInt ("hasChosenToSurvey", 0);
			hasChosenToRate = PlayerPrefs.GetInt ("hasChosenToRate", 0);
			hasReceivedSaveImageWarning = PlayerPrefs.GetInt ("hasReceivedSaveImageWarning", 0);
			hasReceivedSavePermissionsNotification = PlayerPrefs.GetInt ("hasReceivedSavePermissionsNotification", 0);
			// hide menus / pages on launch
			ShowHideStickerMenu(false);
			ShowHidePaintingStickerMenu(false);
			ShowHideFrameMenu(false);
			ShowHideAboutPage(false);
			ShowHideSharePage(false);
			// set UI
			SetCaptureButtons(false);
			SetCropControls(false);
			SetEditButtons(false);
			paintingStickerMode = false;
			AppController.FingersGestureAllowed = false;
			imageSaved = false;
			// make sure no memory left from previous composite is being used
			DisposeCompositor();
			SetAllTexturesTransparent ();
		} else if (mode == Mode.CapturePhoto) {
			// set UI
			SetCaptureButtons(true);
			SetCropControls(false);
			SetEditButtons(false);
			paintingStickerMode = false;
			AppController.FingersGestureAllowed = false;
			// fix preview display aspect ratio
			// this caused weird effects
			//UpdatePreviewAspectRatio(0);
		} else if (mode == Mode.CropPhoto) {
			// set UI
			SetCaptureButtons(false);
			SetCropControls(true);
			SetEditButtons(false);
			paintingStickerMode = false;
			AppController.FingersGestureAllowed = false;
		} else if (mode == Mode.EditPhoto) {
			// set UI
			SetCaptureButtons(false);
			SetCropControls(false);
			SetEditButtons(true);
			paintingStickerMode = false;
			AppController.FingersGestureAllowed = true;
		} else if (mode == Mode.SharePhoto) {
			numberOfSaves++;
			//ResetRatingListeners (); //testing
			//if (DEBUG) Debug.Log("numberOfSaves = "+ numberOfSaves +" // hasChosenToSurvey = "+hasChosenToSurvey +" // hasChosenToRate = "+hasChosenToRate);
			PlayerPrefs.SetInt ("numberOfSaves", numberOfSaves);
			if (numberOfSaves > 2 && numberOfSaves % 10 == 1 && hasChosenToSurvey != 1) {
				//SurveyPrompt ();
			}
			if (numberOfSaves > 4 && numberOfSaves % 10 == 1 && hasChosenToRate >= 0 && hasChosenToRate < 2) {
				RateAppPrompt ();
			}
		}
	}
	void ResetRatingListeners(){
		PlayerPrefs.SetInt ("numberOfSaves",0);
		numberOfSaves = PlayerPrefs.GetInt ("numberOfSaves",0);

		PlayerPrefs.SetInt ("hasChosenToSurvey",0);
		hasChosenToSurvey = PlayerPrefs.GetInt ("hasChosenToSurvey", 0);

		PlayerPrefs.SetInt ("hasChosenToRate",0);
		hasChosenToRate = PlayerPrefs.GetInt ("hasChosenToRate", 0);
		
		PlayerPrefs.SetInt ("hasGivenGalleryPermissions",0);
		hasReceivedSavePermissionsNotification = PlayerPrefs.GetInt ("hasReceivedSavePermissionsNotification", 0);
	}
	void SetAllTexturesTransparent(){
		// replace all textures with a default
		frame.GetComponent<RawImage> ().texture = Resources.Load("Stickers/sticker_000") as Texture2D;
		stickerPrefab.GetComponent<RawImage> ().texture = Resources.Load("Stickers/sticker_000") as Texture2D;
		paintingStickerPrefab.GetComponent<RawImage> ().texture = Resources.Load("Stickers/sticker_000") as Texture2D;
	}
	// set CaptureButtons active/inactive
	void SetCaptureButtons(bool state){
		captureFrameMenuButton.gameObject.SetActive(state);
		switchCameraButton.gameObject.SetActive(state);
		photoCaptureButton.gameObject.SetActive(state);
		selectGalleryButton.gameObject.SetActive(state);
		aboutPageButton.gameObject.SetActive(state);
		//flashButton.gameObject.SetActive(state);
	}
	// set SetCropControls active/inactive
	void SetCropControls(bool state){
		//if (DEBUG) Debug.Log ("SetCropControls() called");
		cropPhotoButton.gameObject.SetActive(state);
		cropExitButton.gameObject.SetActive(state);
		cropSlider.gameObject.SetActive (state);
	}
	// set EditButtons active/inactive
	void SetEditButtons(bool state){
		editFrameMenuButton.gameObject.SetActive(state);
		photoExitButton.gameObject.SetActive(state);
		photoSaveButton.gameObject.SetActive(state);
		stickerMenuButton.gameObject.SetActive(state);
		paintingStickerMenuButton.gameObject.SetActive(state);
		deleteStickersButton.gameObject.SetActive(state);
	}

	// stickers
	public void ShowHideStickerMenu(bool state){
		paintingStickerMode = false;
		AppController.FingersGestureAllowed = !state;
		stickerMenuVisible = state;
		stickerMenu.gameObject.SetActive(stickerMenuVisible);
		AppController.FingersGestureInProgress = 0; // just in case
		SetEditButtons (!state);
		//if (DEBUG) Debug.Log ("ShowHideStickerMenu() -> stickerMenuVisible = " + stickerMenuVisible);
	}
	public void SwitchSticker(int i){
		if (stickerMenuVisible == true)
			ShowHideStickerMenu (false);
		currentSticker = i;
		AddSticker (currentSticker);
		//if (DEBUG) Debug.Log ("SwitchSticker() -> currentSticker = " + i);
	}

	// "painting" stickers
	public void ShowHidePaintingStickerMenu(bool state){
		paintingStickerMenuVisible = state;
		AppController.FingersGestureAllowed = !state;
		paintingStickerMenu.gameObject.SetActive(paintingStickerMenuVisible);
		SetEditButtons (!state);
		AppController.FingersGestureInProgress = 0; // just in case
		//if (DEBUG) Debug.Log ("ShowHidePaintingStickerMenu() -> paintingStickerMenuVisible = " + paintingStickerMenuVisible);
	}
	public void SwitchPaintingSticker(int i){
		if (paintingStickerMenuVisible == true)
			ShowHidePaintingStickerMenu (false);
		currentPaintingSticker = i;
		paintingStickerMode = true;
		AppController.FingersGestureInProgress = 0; // just in case
		//AddPaintingSticker (currentPaintingSticker);
		//if (DEBUG) Debug.Log ("SwitchPaintingSticker() -> currentPaintingSticker = " + i);
	}

	// frames
	public void ShowHideFrameMenu(bool state){
		paintingStickerMode = false;
		AppController.FingersGestureAllowed = !state;
		frameMenuVisible = state;
		frameMenu.gameObject.SetActive(frameMenuVisible);
		//SetCaptureButtons (!frameMenuVisible);
		//if (DEBUG) Debug.Log ("ShowHideFrameMenu() -> frameMenuVisible = " + frameMenuVisible);
	}
	public void SwitchFrame(int _i){
		if (frameMenuVisible == true)
			ShowHideFrameMenu (false);
		currentFrame = _i;
		// load frame - new method to avoid loading in all frames
		frame.GetComponent<RawImage> ().texture = Resources.Load("Frames/Frame-"+ currentFrame.ToString("00") +"_1024h") as Texture2D;
		// rename frame
		frame.name = "frame_" +  currentFrame.ToString("00");
		//if (DEBUG) Debug.Log ("SwitchFrame() -> currentFrame = " + currentFrame);
	}
		
	// About Page
	public void ShowHideAboutPage(bool state){
		aboutPageVisible = state;
		aboutPage.gameObject.SetActive(aboutPageVisible);
		// if page is visible start moving photos
		if (aboutPageVisible) {
			co = StartCoroutine ("MoveFunction");
			coRunning = true;
		}
		// otherwise (if running) stop it
		else if (coRunning) StopCoroutine(co);
		if (mode == Mode.CapturePhoto)
			SetCaptureButtons (!aboutPageVisible);
		//if (DEBUG) Debug.Log ("ShowHideAboutPage() -> aboutPageVisible = " + aboutPageVisible);
	}
	// moving photos on About Page
	IEnumerator MoveFunction() {
		while (true) {
			float x = aboutPagePhotosRT.anchoredPosition.x;
			if (x < startPosition || x >= loopPosition) {
				x = startPosition;
			} else {
				x += 35 * Time.deltaTime;
			}
			//if (DEBUG) Debug.Log ("MoveFunction() --> x =" + x);
			Vector3 temp = new Vector3(x,aboutPagePhotosRT.anchoredPosition.y,0);
			aboutPagePhotosRT.anchoredPosition = temp;
			// to stop coroutine
			//yield break;
			// Otherwise, continue next frame
			yield return null;
		}
	}


	// Share Page
	public void ShowHideSharePage(bool state){
		sharePageVisible = state;
		sharePage.gameObject.SetActive(sharePageVisible);
		if (state == false)
			// remove texture from sharePreview
			sharePreview.GetComponent<RawImage> ().texture = null;
		//if (DEBUG) Debug.Log ("ShowHideSharePage() -> sharePageVisible = " + sharePageVisible);
	}

	public NatCamPreview sharePreview;
	public Image saveCheck;
	public Image shareInstagramCheck;
	public Image shareTwitterCheck;
	public Image shareOtherCheck;




	#region --App Management--

	private bool started = false;
	private bool paused = false;

	// 1. when the app begins
	void Awake(){
		//if (DEBUG) Debug.Log ("Owen: OwenCam: Awake() called");
		SetMode (Mode.Init);
		SetMode (Mode.CapturePhoto);

		// add frame here (instead of other places) so there is only one instance of it in layers
		layers.Add (frame);

		// for moving the aboutPagePhotos
		aboutPagePhotosRT = aboutPagePhotos.GetComponent<RectTransform> ();
		// for cropping photos
		cropSlider.onValueChanged.AddListener(delegate { CropSliderValueChanged(); });
	}

	// 2. when app starts (or user returns to app)
	void OnApplicationPause (bool pauseStatus) { // This gets called when the app is paused and resumed
		//print ("OwenCam -> OnApplicationPause()");
		if (!started) {
			started = true;
		} else {
			paused = true;
		}
	}
	// 3. after it starts
	public override void OnStart () {
		//print ("OwenCam -> OnStart()");
		// If the app is not paused, forward the event to MiniCam
		if (!paused) {
			base.OnStart();
			//focuser.StartTracking();	// Start tracking focus gestures - THIS SHOULD BE IN PARENT CLASS, CHECK SOMETIME ON PHONE
		}
		// Reset the flag
		paused = false;

		// this was stretching the image for some reason
		// checking mode when returning to app before adjusting aspect ratio fixed it
		if (mode == Mode.CapturePhoto)
			UpdatePreviewAspectRatio(0);
		else if (mode == Mode.EditPhoto)
			UpdatePreviewAspectRatio(.5625f);
	}
	#endregion


	/*
	 * // testing
	int i = 1;
	void Update(){

		// for testing updates to layers onscreen and in saved file
		if (Input.GetKeyDown (KeyCode.Tab)) {
			//if (DEBUG) Debug.Log ("TAB");

			if (layers.Count > 0) {
				foreach (GameObject layer in layers) {
					layer.transform.rotation = Quaternion.Euler (layer.transform.eulerAngles * ++i);
					layer.transform.localScale *= 1.01f;
					if (DEBUG) Debug.Log ("layer.GetComponent<RectTransform> ().rect.width = " + layer.GetComponent<RectTransform> ().rect.width + " // layer.transform.localScale = "+ layer.transform.localScale);
				}
			}
		}
	}
*/





	#region --Operations--

	public override void CapturePhoto () {
		base.CapturePhoto();
	}
	#endregion


	#region --Callbacks--


	// called after user takes photo
	protected override void OnPhoto (Texture2D photo, Orientation orientation) {

		// 1. Put it in the preview first, then add to compositor

		// Base
		base.OnPhoto(photo, orientation);

		// Free any previous compositor
		DisposeCompositor();

		// Setup to create a new compositor based on the original size, etc.
		bool rotated = (int)(orientation & Orientation.Rotation_270) % 2 == 1, 
			 mirrored = (orientation & Orientation.Mirror) == Orientation.Mirror;
		// Add the base layer
		Vector2 offset = Vector2.zero,
			scale = mirrored ? new Vector2(-1, 1) : Vector2.one;


		// create the compositor
		compositor = new RenderCompositor (rotated ? photo.height : photo.width, rotated ? photo.width : photo.height);



		if (DEBUG)
			Debug.Log ("OnPhoto() +++ 1 +++ photo.width: " + photo.width + " // " + "photo.height: " + photo.height +
			" // compositor.width: " + compositor.width + " // " + "compositor.height: " + compositor.height +
			" // rotated: " + rotated + " // mirrored: " + mirrored + " // " + "orientation: " + orientation + " // " + "scale: " + scale + " // offset: " + offset);// + 
			//" // _aspectRatio: " + _aspectRatio);
	
		// from desktop testing
		// OnPhoto() +++ 1 +++ photo.width: 1280 // photo.height: 720 // compositor.width: 720 // compositor.height: 405 // rotated: False // mirrored: False // orientation: Rotation_0 // scale: (1.0, 1.0)

		// from iphone straight up
		// OnPhoto() +++ 1 +++ photo.width: 720 // photo.height: 1280 // compositor.width: 720 // compositor.height: 405 // rotated: False // mirrored: False // orientation: Rotation_0 // scale: (1.0, 1.0)
		// from iphone -90 deg rotated
		// OnPhoto() +++ 1 +++ photo.width: 720 // photo.height: 1280 // compositor.width: 720 // compositor.height: 405 // rotated: False // mirrored: False // orientation: Rotation_0 // scale: (1.0, 1.0)


		// If the image is rotated, then offset it so that it covers the left and top of the composite
		// If we don't do this, we will be left with a space on either side of the layer (because Compositor rotates about the layer's centre)
		if (rotated) offset += 0.5f * new Vector2 (photo.height - photo.width, photo.width - photo.height);
		// add photo as first layer to set width / height
		compositor.AddLayer (new Layer(photo, offset, (int)orientation * -90f, scale));




		// 2. Then check the aspect ratio

		// final check for aspect ratio
		float _aspectRatio;
		if (rotated) {
			_aspectRatio = ReturnAspectRatio (photo.height, photo.width);
		} else {
			_aspectRatio = ReturnAspectRatio (photo.width, photo.height);
		}
		// update aspect ratio
		imageFitter.aspectRatio = _aspectRatio;


		if (DEBUG) Debug.Log ("OnPhoto() +++ 2 +++ photo.width: " + photo.width + " // " + "photo.height: " + photo.height +
			" // compositor.width: " + compositor.width + " // " + "compositor.height: " + compositor.height + 
			" // rotated: " + rotated + " // mirrored: " + mirrored + " // " + "orientation: " + orientation + " // " + "scale: " + scale + " // offset: " + offset + 
			" // _aspectRatio: " + _aspectRatio);


		// 3. Next step...

		// testing: check crop in Unity Editor
		//StartCropMode();

		// jump to finish
		StartEditMode ();
	}







	// user loads from gallery
	public void LoadImageFromGallery(){
		#if UNITY_IOS
		// add listener
		IOSCamera.OnImagePicked += OnImageFromGallery_IOS;
		IOSCamera.Instance.PickImage(ISN_ImageSource.Album);
		#elif UNITY_ANDROID
		AndroidCamera.Instance.OnImagePicked += OnImageFromGallery_ANDROID;
		AndroidCamera.Instance.GetImageFromGallery();
		#endif
	}
	#if UNITY_IOS
	// run after user picks image, before edit mode
	private void OnImageFromGallery_IOS (IOSImagePickResult result) {
		IOSCamera.OnImagePicked -= OnImageFromGallery_IOS; // unsubscribe
		//if (DEBUG) Debug.Log ("OnImageFromGallery_IOS() called");
		if(result.IsSucceeded) {
			// display texture
			preview.texture = result.Image;
			// fix preview display aspect ratio
			UpdatePreviewAspectRatio(0);
			// at this point we have a texture in preview, so start crop
			StartCropMode ();
		}
	}
	#elif UNITY_ANDROID
	private void OnImageFromGallery_ANDROID(AndroidImagePickResult result) {
		AndroidCamera.Instance.OnImagePicked -= OnImageFromGallery_ANDROID; // unsubscribe
		//if (DEBUG) Debug.Log ("OnImageFromGallery_ANDROID() called");
		if (result.IsSucceeded) {
			AN_PoupsProxy.showMessage ("Image Pick Result", "Succeeded, path: " + result.ImagePath);
			// display texture
			preview.texture = result.Image;
			// fix preview display aspect ratio
			UpdatePreviewAspectRatio(0);
			// at this point we have a texture in preview, so start crop
			StartCropMode ();
		} else {
			AN_PoupsProxy.showMessage ("Image Pick Result", "Couldn't get image from gallery");
		}	
	}
	#endif





	// start crop mode
	void StartCropMode(){
		
		// is aspect ratio 9:16 / .5625?
		//if (aspectRatio != .5625f) {
		// changing to account for very small differences from .5625
		if (aspectRatio < .56f || aspectRatio > .57f) {
			if (DEBUG) Debug.Log ("StartCropMode() +++ Aspect Ratio ("+ aspectRatio.ToString() +") is NOT CORRECT, initiate crop");
			
			// change to crop photo mode
			SetMode(Mode.CropPhoto);

			// now determine the min/max values of the crop slider so that the image left/right bounds are constrained to the 9:16 screen



			// 0.5625	9:16	HD vert
			// 0.75		3:4		VGA vert	1944:2592, 960:1280
			// 1		1:1 	square
			// 1.33..	4:3		VGA horiz
			// 1.5		3:2 ???
			// 1.77..	16:9	HD horiz

			// sorry, magic numbers		   16:9,   4:3,   1:1	  3:4	9:16 (actually just going to skip crop if this happens)
			float[] cropSliderValues = { .3415f, .2855f, .2188f, .11f, .00001f };

			float cropSliderMax = 0f;
			if (aspectRatio > .5f && aspectRatio < .6f)
				cropSliderMax = cropSliderValues [4];
			else if (aspectRatio > .7f && aspectRatio < .8f) 
				cropSliderMax = cropSliderValues [3];
			else if (aspectRatio > .9f && aspectRatio < 1.1f)
				cropSliderMax = cropSliderValues [2];
			else if (aspectRatio > 1.3f && aspectRatio < 1.4f)
				cropSliderMax = cropSliderValues [1];
			else if (aspectRatio > 1.7f && aspectRatio < 1.8f)
				cropSliderMax = cropSliderValues [0];
			else {
				// add a default
				// make slider zero so not stretching occurs
				OnCropImageButton ();
			}
			// update min/max values of slider
			cropSlider.minValue = -cropSliderMax;
			cropSlider.maxValue = cropSliderMax;
			cropSlider.value = 0;

			if (DEBUG) Debug.Log ("StartCropMode() +++ cropSlider.minValue = "+ cropSlider.minValue +" // cropSlider.maxValue = "+ cropSlider.maxValue );

		} else {
			if (DEBUG) Debug.Log ("StartCropMode() +++ Aspect Ratio ("+ aspectRatio.ToString() +") is fine, skipping crop...");
			OnCropGalleryImage ();
		}
	}

	// callback from cropSlider
	public void CropSliderValueChanged(){
		preview.uvRect = new Rect(cropSlider.value, preview.uvRect.y, preview.uvRect.width, preview.uvRect.height);
		//if (DEBUG) Debug.Log ("preview.uvRect = " + preview.uvRect + " // cropSlider.value = "+ cropSlider.value);
	}
	// after user has adjusted position and clicked the crop icon
	public void OnCropImageButton(){
		// crop the texture
		preview.texture = CropPixelsToAspect(preview.texture as Texture2D, 0.5625f);
		// next step
		OnCropGalleryImage ();
	}	
	// crop a texture based on preferred aspect ratio
	public Texture2D CropPixelsToAspect(Texture2D originalTexture, float ratio){

		// get the aspect ratio to figure out new w/h
		int h = originalTexture.height; 				// keeping height the same
		int w = (int)(originalTexture.height * ratio); 	// new width = original height * aspectRatio

		int x = 0; // the centered X
		//x = (int)(originalTexture.width / 2) - (w/2); // (center of originalTexture) - horizontalOffset for new aspect ratio (only works for 1280 centered (unmoved))
		x = (int)(((originalTexture.width / 2) - (w/2)) + (preview.uvRect.x * originalTexture.width)); // the above + cropSlider offset * originalTexture.width (denormalized)
		int y = 0;

		if (DEBUG) Debug.Log ("CropPixels() +++ // w = " + w + ", h = " + h + ", x = "+ x + ", y = " + y + " // preview.uvRect = " + preview.uvRect + " // cropSlider.value = "+ cropSlider.value);

		// create a new texture of desired w/h
		Texture2D newTex = new Texture2D (w, h);
		// get pixels from original texture with matching rect
		Color[] pixels = originalTexture.GetPixels (x, y, w, h);
		// set pixels into new texture
		newTex.SetPixels (pixels);  
		// apply pixels
		newTex.Apply ();   
		return newTex;
	}
	// after gallery image has been cropped
	public void OnCropGalleryImage(){
		// Free any previous compositor
		DisposeCompositor();

		// image should now be 9:16 so reset imageFitter
		imageFitter.aspectRatio = 0.5625f;
		// also reset values of image in preview
		preview.uvRect = new Rect(0, 0, 1, 1);

		// is it rotated? assume NO for all gallery images
		bool rotated = false;
		/*
		if (preview.texture.width > preview.texture.height) {
			// photo is rotated
			rotated = true;
		}
		*/
		// create compositor
		compositor = new RenderCompositor (rotated ? preview.texture.height : preview.texture.width, rotated ? preview.texture.width : preview.texture.height);
		// Add the base layer
		Vector2 offset = Vector2.zero,
				scale = Vector2.one;

		//if (rotated) offset += 0.5f * new Vector2 (preview.texture.height - preview.texture.width, preview.texture.width - preview.texture.height);

		float rotation = 0f;
		if (rotated)
			rotation = -90f;

		//if (DEBUG) Debug.Log ("OnCropGalleryImage() +++ preview.texture.height = " + preview.texture.height + " // " + "preview.texture.width = " + preview.texture.width + " // rotated = " + rotated + " // " + "rotation = " + rotation + " // " + "scale = " + scale);

		// add photo as first layer to set width / height
		compositor.AddLayer (new Layer(preview.texture as Texture2D, offset, rotation, scale));

		// finish
		StartEditMode ();
	}


	// everything is done with capture photo (and crop if needed)
	// from Capture or Pick, so add frame and shift to EditMode
	void StartEditMode(){
		// set to edit mode
		SetMode(Mode.EditPhoto);
	}







	float ReturnAspectRatio(float _w, float _h){
		float _aspectRatio = ((float)_w / (float)_h);
		if (DEBUG) Debug.Log ("ReturnAspectRatio() --> width = " + _w + " // height = " + _h + " // _aspectRatio = " + _aspectRatio );
		return _aspectRatio;
	}
	void UpdatePreviewAspectRatio(float _aspectRatio){
		if (DEBUG) Debug.Log ("UpdatePreviewAspectRatio("+ _aspectRatio +") 1 called");
		// IF _aspectRatio == 0 THEN get the aspect ratio of the texture
		if (_aspectRatio == 0 && preview.texture.width > 0) {
			if (DEBUG) Debug.Log ("UpdatePreviewAspectRatio("+ _aspectRatio +") 2 called");
			aspectRatio = ReturnAspectRatio (preview.texture.width, preview.texture.height);
		} else {
			if (DEBUG) Debug.Log ("UpdatePreviewAspectRatio("+ _aspectRatio +") 3 called");
			// use a defined aspect ratio
			aspectRatio = _aspectRatio;
		}
		if (DEBUG) Debug.Log ("UpdatePreviewAspectRatio("+ aspectRatio +") 4 called");
		// update fitter 
		imageFitter.aspectRatio = aspectRatio;
	}



	#endregion

	#region --Input Handlers--

	// when user touches / clicks on screen
	public void OnPointerUp (BaseEventData eventData) {
		OnPointerUp (eventData as PointerEventData);
	}
	public void OnPointerUp (PointerEventData eventData) {
		if (DEBUG) Debug.Log("OnPointerUp() -> eventData.position = "+ eventData.position + " // mode = "+ mode +" // paintingStickerMode = "+ paintingStickerMode + " // OnPointerUp() -> AppController.FingersGestureInProgress = " + AppController.FingersGestureInProgress);

		// make sure we are in the right mode
		if (mode != Mode.EditPhoto) return;	
		if (paintingStickerMode != true) return;
		// make sure we aren't editing a regular sticker: RELIES ON EDITS TO FingersPanRotateScaleScript.cs *MOVED TO MY FOLDER
		// ALL INSTANCES OF: GestureRecognizerState.Began | GestureRecognizerState.Executing
		// SHOULD INCLUDE:   	AppController.FingersGestureInProgress = 1;
		// ALL INSTANCES OF: GestureRecognizerState.Ended
		// SHOULD INCLUDE:   	AppController.FingersGestureInProgress = 0;
		if (AppController.FingersGestureInProgress == 1) return;
		// debugging painting sticker issue
		if (DEBUG) Debug.Log("OnPointerUp() -> AppController.FingersGestureInProgress = " + AppController.FingersGestureInProgress);
		AddPaintingSticker (eventData);
	}
	public void AddPaintingSticker(PointerEventData eventData){
		// Get the point within the RectTransform of the click
		Vector2 rectPoint;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(preview.rectTransform, eventData.position, eventData.pressEventCamera, out rectPoint)) return;
		//if (DEBUG) Debug.Log ("eventData.position = " + eventData.position + "// " + canvas.planeDistance + " // =============== rectPoint = "+ rectPoint);

		// set random rotation
		var rotation = Random.Range(0f, 360f);

		// create GameObject
		GameObject layer;

		// store sticker texture in paintingStickerPrefab
		//paintingStickerPrefab.GetComponent<RawImage> ().texture = paintingStickersArray[currentPaintingSticker];
		paintingStickerPrefab.GetComponent<RawImage> ().texture = Resources.Load("PaintingStickers/pSticker_"+ currentPaintingSticker.ToString("00") ) as Texture2D;
	
		//if (DEBUG) Debug.Log ("paintingStickersArray.Length = " + paintingStickersArray.Length + " // paintingStickersArray["+i+"].name  = " + paintingStickersArray[i].name );

		// Instantiate the sprite
		layers.Add(layer = Instantiate (
			paintingStickerPrefab,
			Camera.main.ScreenToWorldPoint (new Vector3(eventData.position.x, eventData.position.y, canvas.planeDistance)),
			Quaternion.Euler(0f, 0f, rotation)
		));
		//if (DEBUG) Debug.Log ("layers.Count = " + layers.Count + " // layers["+ (layers.Count - 1) +"].name  = " + layers[  (layers.Count - 1)  ].name );

		// rename sticker
		layer.name = "pSticker_" +  currentPaintingSticker.ToString("000");

		// Parent under canvas
		//layer.transform.SetParent(paintingStickersPlaced.transform, true);
		layer.transform.SetParent(stickersPlaced.transform, true); // so they are all in order

		// set scale
		layer.transform.localScale = Vector3.one;

		// set random scale
		//var rScale = Random.Range(-0.7f,1.3f);
		//layer.transform.localScale = Vector3.one * rScale;

		//if (DEBUG) Debug.Log ( "n/" + layers.Count + " // rectPoint = "+ rectPoint + " // pixelPoint = xxx // rotation = "+ rotation + " // layer.transform.localScale = "+ layer.transform.localScale);

	}


	public void AddSticker(int index){
		// State checking
		if (mode != Mode.EditPhoto) return;

		// Get the point within the RectTransform of the click
		Vector2 rectPoint;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(preview.rectTransform, new Vector2(0,0), Camera.main, out rectPoint)) return;
		//if (DEBUG) Debug.Log ("eventData.position = " + eventData.position + "// " + canvas.planeDistance + " // rectPoint = "+ rectPoint);

		// set random rotation
		var rotation = Random.Range(0f, 360f);

		// create GameObject
		GameObject layer;

		// pick random index
		if (index == -1)
			currentSticker = Random.Range (1, 127);
		
		// add sticker texture to stickerPrefab, directly from Resources
		stickerPrefab.GetComponent<RawImage> ().texture = Resources.Load("Stickers/sticker_"+ currentSticker.ToString("000") ) as Texture2D;

		// Instantiate the sprite
		layers.Add(layer = Instantiate (
			stickerPrefab,
			Camera.main.ScreenToWorldPoint (new Vector3(Screen.width/2, Screen.height/2, canvas.planeDistance)),
			Quaternion.Euler(0f, 0f, rotation),
			stickerPrefab.transform
		));
		//if (DEBUG) Debug.Log ("layers.Count = " + layers.Count + " // layers["+ (layers.Count - 1) +"].name  = " + layers[  (layers.Count - 1)  ].name );

		// rename sticker
		layer.name = "sticker_" +  currentSticker.ToString("000");

		// set default localScale
		layer.transform.localScale = Vector3.one;

		// get size of texture 
		Vector2 textureSize = new Vector2(stickerPrefab.GetComponent<RawImage> ().texture.width, stickerPrefab.GetComponent<RawImage> ().texture.height);
		// get RectTransform
		RectTransform rt = layer.GetComponent<RectTransform>();
		// set RectTransform size to prevent stretching of non-square textures) 
		rt.sizeDelta = textureSize;
		// if (DEBUG) Debug.Log ("textureSize = "+ textureSize);

		// Parent under canvas
		layer.transform.SetParent(stickersPlaced.transform, false);


		// set random scale
		//var rScale = Random.Range(-0.7f,1.3f);
		//layer.transform.localScale = Vector3.one * rScale;

		//if (DEBUG) Debug.Log ( "n/" + layers.Count + " // rectPoint = "+ rectPoint + " // pixelPoint = xxx // rotation = "+ rotation + " // layer.transform.localScale = "+ layer.transform.localScale);

	}


	#endregion




	// POPUP/DIALOG: User clicks Save in EditMode > Confirm save/finished editing?
	public void FinishEditingPhoto(){
		// commenting this out so it happens all the time
		//if (hasReceivedSaveImageWarning == 0) {
			if (Application.isEditor) {
				Composite ();
			} else {
				// crossplatform popup/dialog
				MNPopup popup = new MNPopup ("Continue to save?", "After you choose to save your image you’ll no longer be able to make any edits.");
				// options
				popup.AddAction ("No", () => { /*if (DEBUG) Debug.Log ("No action callback");*/ });  // do nothing
				popup.AddAction ("Yes", () => { //if (DEBUG) Debug.Log ("Yes action callback");
					if (DEBUG) Debug.Log ("Yes action callback");
					// set prefs
					hasReceivedSaveImageWarning = 1;
					PlayerPrefs.SetInt ("hasReceivedSaveImageWarning", hasReceivedSaveImageWarning);
					// composite layers
					Composite ();
				});
				popup.AddDismissListener (() => { /*if (DEBUG) Debug.Log ("dismiss listener");*/ }); // do nothing
				popup.Show ();
			}
		//} else {
		//	Composite ();
		//}
	}

	/*
	// POPUP: Confirm save/finished editing
	public void FinishEditingPhotoOLD(){
		#if UNITY_EDITOR
			Composite (); // go straigt to composite, no popups
		#endif
		#if UNITY_IOS
			if (hasReceivedSaveImageWarning == 0){
				// create dialog
				IOSDialog dialog = IOSDialog.Create("Continue to save?", "After you choose to save your image you’ll no longer be able to make any edits.");
				// add listener
				dialog.OnComplete += onFinishEditingPhotoClose_IOS;
			} else {
				Composite ();
			}
		#endif
		#if UNITY_ANDROID
			if (hasReceivedSaveImageWarning == 0){
				// create dialog
				AndroidDialog dialog = AndroidDialog.Create("Continue to save?", "After you choose to save your image you’ll no longer be able to make any edits.");
				// add listener
				dialog.ActionComplete += onFinishEditingPhotoClose_ANDROID;
			} else {
				Composite ();
			}
		#endif
	}
	#if UNITY_IOS
	private void onFinishEditingPhotoClose_IOS(IOSDialogResult result) {
		
		hasReceivedSaveImageWarning = 1;
		PlayerPrefs.SetInt ("hasReceivedSaveImageWarning", hasReceivedSaveImageWarning);

		//parsing result
		switch(result) {
			case IOSDialogResult.YES:
				//if (DEBUG) Debug.Log ("Yes button pressed");
				Composite ();
				break;
			case IOSDialogResult.NO:
				//if (DEBUG) Debug.Log ("No button pressed");
				// stay where you are
				break;
		}
	}
	#elif UNITY_ANDROID
	private void onFinishEditingPhotoClose_ANDROID(AndroidDialogResult result) {

		hasReceivedSaveImageWarning = 1;
		PlayerPrefs.SetInt ("hasReceivedSaveImageWarning", hasReceivedSaveImageWarning);

		//parsing result
		switch(result) {
			case AndroidDialogResult.YES:
				//if (DEBUG) Debug.Log ("Yes button pressed");
				Composite ();
				break;
			case AndroidDialogResult.NO:
				//if (DEBUG) Debug.Log ("No button pressed");
				// stay where you are
				break;
		}
	}
	#endif
	*/



	#region --Compositing--

	void Composite () {


		if (DEBUG) Debug.Log("Composite () --> layers.Count = "+ layers.Count );

		//if (layers.Count < 0) return;
		
		// on composite file let's save the stats
		string frameNum = "";
		string stickerNum = "";
		string pStickerNum = "";

		//int c = 0;
		foreach (var layer in layers) {

			if (DEBUG) Debug.Log("Composite () --> layers.Count = "+ layers.Count + " // layer = "+ layer + " // layer.name = "+ layer.name);

			// Find the position of the layer UI object relative to a background UI panel: Transform the UI object's canvas-space position to a screen position, 
			// then transform that screen position to the background panel relative position. The result of this operation should be a position coordinate that is  
			// relative to thebackground UI panel. Now, you want to transform this position to a composite space coordinate (assuming that the composite is the 
			// size of the background layer). To do so, simply scale the ratio of the coordinate and the background panel size by the composite size (coord / panelSize * compositeSize).

			Vector2 rectPoint;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(preview.rectTransform, RectTransformUtility.WorldToScreenPoint (Camera.main,new Vector2 (layer.transform.position.x, layer.transform.position.y)), Camera.main, out rectPoint)) return;

			// Offset (from left side)
			rectPoint += 0.5f * preview.rectTransform.rect.size;

			// Calculate the background-relative pixel coords // Make sure there's actually a texture on the RawImage
			var pixelPoint = new Vector2(compositor.width * rectPoint.x / preview.rectTransform.rect.width, compositor.height * rectPoint.y / preview.rectTransform.rect.height);

			//if (DEBUG) Debug.Log ( "compositor.width = "+ compositor.width +" // preview.rectTransform.rect.width = "+ preview.rectTransform.rect.width + " // preview.rectTransform.rect.height = "+ preview.rectTransform.rect.height );


			// Offset the pixel coords so that the image's center is positioned, not its bottom left
			var texture = layer.GetComponent<RawImage>().texture as Texture2D;
			pixelPoint -= 0.5f * new Vector2(texture.width, texture.height);

			/*
			if (DEBUG) Debug.Log ( c++ + "/" + layers.Count + " // layer.GetComponent<RectTransform> ().anchoredPosition = "+ layer.GetComponent<RectTransform> ().anchoredPosition +
				" // layer.transform.position = "+ layer.transform.position + " // rectPoint = "+ rectPoint + " // pixelPoint = "+ pixelPoint +
				" // canvas.scaleFactor " + canvas.scaleFactor
			);
			*/

			// THIS ONE IS TESTED AND LOOKS VERY EXACT ON MAC AND IPHONE
			/**/
			// Calculate the scale
			// We want the ratio of the layer's size to the composite size to be equal to the ratio of the layer sprite size to the background panel size
			// This is a better scaling term because it accounts for the size of the layer relative to the size of the composite
			var scale = Vector2.one * compositor.width / texture.width * layer.GetComponent<RectTransform> ().rect.width / preview.rectTransform.rect.width;

			/**/
			// my version
			// Scale
			//var scale = Vector2.one * layer.GetComponent<RectTransform> ().rect.width * canvas.scaleFactor / texture.width; // original

			// add scale
			if (layer.transform.localScale.x > 1.0f || layer.transform.localScale.x < 1.0f)	
				scale *= layer.transform.localScale.x;	// scale up if this has been changed
			
			//if (DEBUG) Debug.Log ( c + "/" + layers.Count + " // scale = "+ scale + " // layer.transform.localScale = " +layer.transform.localScale);

			// Rotation
			var rotation = layer.GetComponent<RectTransform>().eulerAngles.z;
			//if (DEBUG) Debug.Log ( c + "/" + layers.Count + " // rotation = "+ rotation );


			//if (DEBUG) Debug.Log ("layer.name = " + layer.name);
			if (layer.name.IndexOf ("pSticker") > -1) {
				pStickerNum = pStickerNum + layer.name.Substring(layer.name.Length - 3) + ",";
			} else if (layer.name.IndexOf ("sticker") > -1) {
				stickerNum = stickerNum + layer.name.Substring(layer.name.Length - 2) + ",";
			} else if (layer.name.IndexOf ("frame") > -1) {
				frameNum = frameNum + layer.name.Substring(layer.name.Length - 2) + ",";
			} 

			// Add layer
			compositor.AddLayer (new Layer (texture, pixelPoint, rotation, scale));
		}

		// report stat to Unity Analytics
		Analytics.CustomEvent("Photo Saved", new Dictionary<string, object>{
			{ "frameNum", frameNum },
			{ "stickerNum", stickerNum },
			{ "pStickerNum", pStickerNum }
		});
		//if (DEBUG) Debug.Log ("frameNum = " + frameNum);
		//if (DEBUG) Debug.Log ("stickerNum = " + stickerNum);
		//if (DEBUG) Debug.Log ("pStickerNum = " + pStickerNum);


		// share option after composite
		compositor.Composite (ShareOnComposite);
	}
	// After compositing load Share Page
	public void ShareOnComposite (Texture2D result) {
		if (DEBUG) Debug.Log ("ShareOnComposite() +++ preview.texture.height = " + preview.texture.height + " // " + "preview.texture.width = " + preview.texture.width  + 
									" // aspectRatio = " + (float)(preview.texture.height / preview.texture.width) );

		// display stuff
		SetMode (Mode.SharePhoto);
		ResetSaveChecks ();
		ShowHideSharePage (true);

		// pass the reference to the photo tex to larger scope (to free it later)
		tempTexture = result;

		// display texture on share page
		sharePreview.GetComponent<RawImage> ().texture = tempTexture;
	}
	void ResetSaveChecks(){
		saveCheck.gameObject.SetActive (false);
		shareInstagramCheck.gameObject.SetActive (false);
		shareTwitterCheck.gameObject.SetActive (false);
		shareOtherCheck.gameObject.SetActive (false);
	}

	/*
	// original OnComposite, changing to SavePhoto() only, with separate share options
	public void OnComposite (Texture2D result) {
		// Get the save mode
		var saveMode =
			#if UNITY_EDITOR
			SaveMode.SaveToAppDocuments;
			#else
			SaveMode.SaveToPhotoGallery;
			#endif
		// Save the photo
		NatCam.SavePhoto(
			result,
			saveMode,
			callback: (mode, path) => if (DEBUG) Debug.Log("Photo saved with mode " + mode + " to path: " + path)
		);
		DisposeCompositor();

		// display confirmation
		QuitPhoto ();
	}
	*/

	/**
	 *	Basic save to file. Works in editor, maybe in standalone...
	 */
	void SaveTextureToFile (Texture2D texture, string filename) {
		Debug.Log ("File saved at: " + filename);
		System.IO.File.WriteAllBytes (filename, texture.EncodeToPNG());
	}
	public void SavePhoto () {
		// get texture
		Texture2D result = sharePreview.GetComponent<RawImage> ().texture as Texture2D;

		// for testing
		if (Application.isEditor) {
			SaveTextureToFile (result, "texture_"+ System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") +".png" );
			saveCheck.gameObject.SetActive (true);
			imageSaved = true;
		} else {
			// now using UltimateMobile to save file because NatCam1.6 removed this function
			UM_Camera.Instance.OnImageSaved += OnImageSaved;
			UM_Camera.Instance.SaveImageToGalalry (result);
		}
	}
	void OnImageSaved (UM_ImageSaveResult result) {
		if(result.IsSucceeded) {
			//no image path for IOS
			//new MobileNativeMessage("Image Saved", result.imagePath);
			saveCheck.gameObject.SetActive (true);
			imageSaved = true;
		} else {
			//new MobileNativeMessage("Failed", "Image Save Failed");
			imageSaved = false;
		}
	}



	// Dispose the compositor
	private void DisposeCompositor(){
		if (compositor != null) {
			compositor.Dispose ();
			compositor = null;
		}
	}


	// delete all stickers, reset frame
	void DeleteStickersResetFrame(){
		if (DEBUG) Debug.Log ("DeleteStickersResetFrame() -> layers.Count = " + layers.Count);

		// replace texture in frame
		frame.GetComponent<RawImage> ().texture = Resources.Load("Stickers/sticker_000") as Texture2D;

		// Delete all layers
		if (layers.Count > 0) {

			// loop through (references to) all layers added
			foreach (GameObject layer in layers) {

				if (DEBUG) Debug.Log ("DeleteStickersResetFrame() -> foreach() layer.name = " + layer.name);

				// only do the following to stickers
				if (layer.name.Contains("sticker") || layer.name.Contains("Sticker")){

					// set to transparent
					layer.GetComponent<RawImage> ().texture = Resources.Load("Stickers/sticker_000") as Texture2D;

					// hide from the renderer
					layer.gameObject.SetActive (false);


				}
			}
			//layers.Clear ();
		}


		// remove unused assets, this line is what fixed all the leaky memory!!!
		// now causing a fatal error :-(
		//Resources.UnloadUnusedAssets ();


		AppController.RemoveAllTexturesFromMemory ();

		/*
		 * // THIS IS CAUSING AN ERROR IN THE BUILD ONLY :-(
		// find all loaded textures
		Texture2D[] ts = FindObjectsOfTypeAll(typeof(Texture2D)) as Texture2D[];
		if (DEBUG) Debug.Log("Number of all loaded Textures " + ts.Length);
		// loop through them
		foreach (Texture2D t in ts) {
			// unload those with frame or sticker in the name
			if (t.name.Contains ("Sticker_") || (t.name.Contains ("Frame-") && t.name.Contains ("1024"))) {
				if (DEBUG)
					Debug.Log ("t.name = " + t.name + " // t.width = " + t.width + " // t.height = " + t.height + " // t.ToString() = " + t.ToString () + " // t.GetInstanceID() = " + t.GetInstanceID ());
				Resources.UnloadAsset (t);
			}
		}
		*/


	}


	// quit all the frames, stickers, and restart camera
	public void QuitPhoto(){
		//if (DEBUG) Debug.Log ("QuitPhoto()");

		// remove stickers / reset frame
		DeleteStickersResetFrame();

		// reset tempTexture (this should release the photo texture for GC)
		if (tempTexture != null) {
			Texture2D.Destroy (tempTexture); 
			tempTexture = null;
		}


		/*	// DOING THIS NOW IN DeleteStickersResetFrame()
		 * 
		// loop for all random objects of Texture2D floating about
		Texture2D[] ts = FindObjectsOfType(typeof(Texture2D)) as Texture2D[];
		foreach (Texture2D t in ts) {
			if (DEBUG) Debug.Log ("t.name = "+ t.name +" // t.width = "+ t.width +" // t.height = "+ t.height +" // t.ToString() = "+ t.ToString() +" // t.GetInstanceID() = "+ t.GetInstanceID());

			//DestroyImmediate(t, true);
			//Resources.UnloadAsset(t);

			// and destroy
			//Destroy (t);
		}
		*/

		// dispose compositor
		DisposeCompositor();

		// Free / release / dispose the photo texture
		if (photo != null) {
			Texture2D.Destroy (photo); 
			photo = null;
		}

		// clear share preview
		sharePreview.GetComponent<RawImage> ().texture = null;

		// run the garbage collector
		System.GC.Collect();

		// reset everything
		SetMode (Mode.Init);
		// restart
		base.OnStart ();
		SetMode (Mode.CapturePhoto);
		// fix preview display aspect ratio
		UpdatePreviewAspectRatio(0);
	}

	public void RestartApp(){
		QuitPhoto ();
		SetMode (Mode.Init);
		SetMode (Mode.CapturePhoto);

	}


	#endregion








	#region --Sharing--

	// SHARE: INSTAGRAM

	public void ShareInstagram(){
		//if (DEBUG) Debug.Log ("ShareInstagram()");
		// get texture
		Texture2D tex = sharePreview.GetComponent<RawImage> ().texture as Texture2D;

		#if UNITY_IOS
		IOSSocialManager.Instance.InstagramPost(tex, "#mirawarri");
		// set callback
		IOSSocialManager.OnInstagramPostResult += HandleOnInstagramPostResult_IOS;
		#elif UNITY_ANDROID
		// universal method didn't work 
		// https://unionassets.com/ultimate-mobile/instagram-557
		//UM_ShareUtility.InstagramShare("#mirawarri", tex);


		// trying this instead
		// https://unionassets.com/android-native-plugin/instagram-sharing-199
		AndroidInstagramManager.Instance.Share(tex, "#mirawarri");
		// set callback
		AndroidInstagramManager.OnPostingCompleteAction += HandleOnInstagramPostResult_ANDROID;





		#endif
	}
	// callbacks
	#if UNITY_IOS
	void HandleOnInstagramPostResult_IOS (SA.Common.Models.Result result){
		if (result.Error.Code > 0) {
			return;
		} else if (result.IsSucceeded) {
			shareInstagramCheck.gameObject.SetActive (true); // show check
		} else {
			//if (DEBUG) Debug.Log("Posting failed with error code " + result.ToString());
		}
	}
	#elif UNITY_ANDROID
	/**/
	// These don't seem to be supported ?????
	void HandleOnInstagramPostResult_ANDROID (InstagramPostResult result){
		if (result == InstagramPostResult.RESULT_OK) {
			shareInstagramCheck.gameObject.SetActive (true); // show check
		} else {
			//if (DEBUG) Debug.Log("Posting failed with error code " + result.ToString());
		}
	}

	#endif


	// SHARE: TWITTER

	// created a twitter app under my account: 
	// https://apps.twitter.com/app/14010996/
	public void ShareTwitter(){
		//Debug.Log ("ShareTwitter()");

		// get texture
		Texture2D tex = sharePreview.GetComponent<RawImage> ().texture as Texture2D;

		#if UNITY_IOS
		IOSSocialManager.Instance.TwitterPost("#mirawarri","http://grettalouw.com/mirawarri",  tex);
		// set callback
		IOSSocialManager.OnTwitterPostResult += HandleOnTwitterPostResult_IOS;
		#elif UNITY_ANDROID

		// universal
		// https://unionassets.com/ultimate-mobile/twitter-556
		UM_ShareUtility.TwitterShare("#mirawarri", tex);


		// android-native (didn't work)
		// https://unionassets.com/android-native-plugin/twitter-167
		//AndroidSocialGate.StartShareIntent("Post to Twitter", " #mirawarri http://grettalouw.com/mirawarri",  tex, "twi");
	

		// I guess there is no callback, so skip the check
		//shareTwitterCheck.gameObject.SetActive (true);
		#endif
	}
	void HandleOnTwitterPostResult_IOS (SA.Common.Models.Result result){
		//Debug.Log ("HandleOnTwitterPostResult_IOS()");
		if(result.IsSucceeded) {
			//Debug.Log ("HandleOnTwitterPostResult_IOS() --> result.IsSucceeded = "+ result.IsSucceeded);
			IOSNativePopUpManager.showMessage("Post to Twitter", "Post Success!");
			// show check
			shareTwitterCheck.gameObject.SetActive (true);
		} else {
			//IOSNativePopUpManager.showMessage("Post to Twitter", "Post Failed :( Error code: " + result.Error.Code);
		}
	}


	// SHARE: OTHER

	public void ShareOther(){
		//if (DEBUG) Debug.Log ("ShareOther()");
		// get texture
		Texture2D tex = sharePreview.GetComponent<RawImage> ().texture as Texture2D;

		#if UNITY_IOS
		IOSSocialManager.Instance.ShareMedia(" #mirawarri", tex);
		#elif UNITY_ANDROID
		// this one worked sometimes. Did not copy images into email though
		// https://unionassets.com/android-native-plugin/native-sharing-164
		//AndroidSocialGate.StartShareIntent("Share your image", "Check out Mirawarri!", tex);

		// this universal one wants a string as the second param
		// https://unionassets.com/ultimate-mobile/default-sharing-dialog-560
		UM_ShareUtility.ShareMedia("Share your image", "Check out Mirawarri!", tex);
		#endif

		// show check
		//shareOtherCheck.gameObject.SetActive (true);
	}


	#endregion // END SHARING





	#region --PopupsAndDialogs--


	// POPUP/DIALOG: User clicks "X" in EDIT MODE > Confirm exit
	public void QuitPhotoEditMode(){
		if (DEBUG) Debug.Log ("QuitPhotoEditMode()");
		#if UNITY_EDITOR
		QuitPhoto ();
		#else
		// crossplatform popup/dialog
		MNPopup popup = new MNPopup ("Close image without saving?", "Are you sure you want to quit without saving your image first?");
		// options
		popup.AddDismissListener (() => { /*if (DEBUG) Debug.Log ("dismiss listener");*/ }); // do nothing
		popup.AddAction ("No", () => { /*if (DEBUG) Debug.Log ("No action callback");*/ });  // do nothing
		// options
		popup.AddAction ("Yes", () => { //if (DEBUG) Debug.Log ("Yes action callback");
			QuitPhoto ();
		});
		popup.Show ();
		#endif
	}
	/*
	// OLD
	public void QuitPhotoEditModeOLD(){
		#if UNITY_EDITOR
		QuitPhoto ();
		#else
		// create dialog
		IOSDialog dialog = IOSDialog.Create("Close image without saving", "Are you sure you want to quit without saving your image first?");
		// add listener
		dialog.OnComplete += onQuitPhotoEditModeClose;
		#endif
	}
	private void onQuitPhotoEditModeClose(IOSDialogResult result) {
		// test
		//IOSNativePopUpManager.showMessage("Result", result.ToString() + " button pressed");

		//parsing result
		switch(result) {
		case IOSDialogResult.YES:
			//if (DEBUG) Debug.Log ("Yes button pressed");
			QuitPhoto ();
			break;
		case IOSDialogResult.NO:
			//if (DEBUG) Debug.Log ("No button pressed");
			// stay where you are
			break;
		}
	}
	*/




	// POPUP/DIALOG: User clicks "X" on SHARE PAGE > Confirm quit w/o saving?
	public void QuitPhotoSharePage(){
		#if UNITY_EDITOR
		// exit, hide share page
		QuitPhoto ();
		ShowHideSharePage (false);
		#else
		if (imageSaved == false){
			// crossplatform popup/dialog
			MNPopup popup = new MNPopup ("Close image without saving?", "Are you sure you want to quit without saving your image first?");
			// options
			popup.AddAction ("No", () => { /*if (DEBUG) Debug.Log ("No action callback");*/ });  // do nothing
			popup.AddAction ("Yes", () => { //if (DEBUG) Debug.Log ("Yes action callback");
				// exit, hide share page
				QuitPhoto ();
				ShowHideSharePage (false);
			});
			popup.AddDismissListener (() => { /*if (DEBUG) Debug.Log ("dismiss listener");*/ }); // do nothing
			popup.Show ();
		} else {
			// exit, hide share page
			QuitPhoto ();
			ShowHideSharePage (false);
		}
		#endif
	}
	/*
	// OLD
	public void QuitPhotoSharePageOLD(){
		#if UNITY_EDITOR
		QuitPhoto ();
		ShowHideSharePage (false);
		#elif UNITY_IOS
		if (imageSaved == false){
			// create dialog
			IOSDialog dialog = IOSDialog.Create("Close image without saving", "Are you sure you want to quit without saving your image first?");
			// add listener
			dialog.OnComplete += onQuitPhotoSharePageDialogClose_IOS;
		} else {
			QuitPhoto ();
			ShowHideSharePage (false);
		}
		#elif UNITY_ANDROID
		if (imageSaved == false){
			// create dialog
			AndroidDialog dialog = AndroidDialog.Create("Close image without saving", "Are you sure you want to quit without saving your image first?");
			// add listener
			dialog.OnComplete += onQuitPhotoSharePageDialogClose_ANDROID;
		} else {
			QuitPhoto ();
			ShowHideSharePage (false);
		}
		#endif
	}
	#if UNITY_IOS
	private void onQuitPhotoSharePageDialogClose_IOS(IOSDialogResult result) {
		switch(result) {
			case IOSDialogResult.YES:
				//if (DEBUG) Debug.Log ("Yes button pressed");
				QuitPhoto ();
				ShowHideSharePage (false);
				break;
			case IOSDialogResult.NO:
				//if (DEBUG) Debug.Log ("No button pressed");
				break; // stay where you are
		}
	}
	#elif UNITY_ANDROID
	private void onQuitPhotoSharePageDialogClose_ANDROID(AndroidDialogResult result) {
		switch(result) {
			case AndroidDialogResult.YES:
				//if (DEBUG) Debug.Log ("Yes button pressed");
				QuitPhoto ();
				ShowHideSharePage (false);
				break;
			case AndroidDialogResult.NO:
				//if (DEBUG) Debug.Log ("No button pressed");
				break; // stay where you are
		}
	}
	#endif
	*/



	// POPUP/DIALOG: User clicks TRASH CAN (EditMode) > Confirm delete stickers
	public void DeleteStickersButton(){
		if (DEBUG) Debug.Log ("DeleteStickersButton() -> layers.Count = " + layers.Count);
		if (layers.Count > 0){
			if (Application.isEditor)
				DeleteStickersResetFrame ();
			else {
				// crossplatform popup/dialog
				MNPopup popup = new MNPopup ("Remove all stickers", "Are you sure you want to delete all stickers on this photo?");
				// options
				popup.AddDismissListener (() => { /*if (DEBUG) Debug.Log ("dismiss listener");*/ }); // do nothing
				popup.AddAction ("No", () => { /*if (DEBUG) Debug.Log ("No action callback");*/ });  // do nothing
				popup.AddAction ("Yes", () => { //if (DEBUG) Debug.Log ("Yes action callback");
					// remove all stickers, reset frame
					DeleteStickersResetFrame ();
				});
				popup.Show ();
			}
		} 
	}
	/*
	// OLD
	public void DeleteStickersButtonOLD(){

		#if UNITY_EDITOR
		DeleteStickersResetFrame ();
		#elif UNITY_IOS
		// create dialog
		IOSDialog dialog = IOSDialog.Create("Remove all stickers", "Are you sure you want to delete all stickers on this photo?");
		// add listener
		dialog.OnComplete += onDeleteStickersButtonClose;
		#endif

	}
	private void onDeleteStickersButtonClose(IOSDialogResult result) {
		// test
		//IOSNativePopUpManager.showMessage("Result", result.ToString() + " button pressed");

		//parsing result
		switch(result) {
		case IOSDialogResult.YES:
			//if (DEBUG) Debug.Log ("Yes button pressed");
			DeleteStickersResetFrame ();
			break;
		case IOSDialogResult.NO:
			//if (DEBUG) Debug.Log ("No button pressed");
			// stay where you are
			break;
		}
	}
	*/
	




	// POPUP/DIALOG: User clicks Save Button on Share Screen > Receives notification they have to set this permission
	public void SavePhotoPermissionsCheck(){
		//if (DEBUG) Debug.Log ("SavePhotoPermissionsCheck()");

		if (hasReceivedSavePermissionsNotification == 1 || Application.isEditor)
			SavePhoto ();
		else {
			// crossplatform popup/dialog
			MNPopup popup = new MNPopup ("Saving requires gallery permissions", 
				"Please be sure to allow Mirawarri to access your photo album to save photos. You can change this in your device settings.");
			// options
			popup.AddDismissListener (() => { /*if (DEBUG) Debug.Log ("dismiss listener");*/ }); // do nothing
			//popup.AddAction ("No", () => { /*if (DEBUG) Debug.Log ("No action callback");*/ });  // do nothing
			popup.AddAction ("Continue", () => { //if (DEBUG) Debug.Log ("Yes action callback");
				SavePhoto();
			});
			popup.Show ();
			
			hasReceivedSavePermissionsNotification = 1;
			PlayerPrefs.SetInt ("hasReceivedSavePermissionsNotification", hasReceivedSavePermissionsNotification);
		}

	}


	public void ResetRatingListenersButton(){
		ResetRatingListeners ();
	}


	public void RateAppButton(){

		string appleUrl = "itms-apps://itunes.apple.com/app/id1240220547?action=write-review"; 
		string androidUrl = "market://details?id="+Application.identifier;

		#if UNITY_IOS
		// add listener
		Application.OpenURL (appleUrl);
		#elif UNITY_ANDROID
		Application.OpenURL (androidUrl);
		#endif

	}

	// POPUP/DIALOG: Promt user to rate app
	public void RateAppPrompt(){

		//https://stackoverflow.com/questions/3124080/app-store-link-for-rate-review-this-app

		// samples
		//itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id=YOUR_APP_ID&onlyLatestVersion=true&pageNumber=0&sortOrdering=1&type=Purple+Software
						
		string appleAppId = "1240220547"; // 
		string androidAppId = ""+Application.identifier;

		string appleUrl = "itms-apps://itunes.apple.com/app/id1240220547?action=write-review"; 
		string androidUrl = "market://details?id="+Application.identifier;

		if (DEBUG) Debug.Log ("RateAppPrompt() --> androidAppUrl = market://details?id=" + Application.identifier);

		// hasChosenToRate 
		// -1 == has declined
		//  0 == has never received
		//  1 == has chosen remind / or dismissed
		//  2 == has rated

		if (!Application.isEditor) {
			if (DEBUG) Debug.Log ("RateAppPrompt() --> CHECKING... hasChosenToRate = "+hasChosenToRate);
			if (hasChosenToRate == 0) {
				MNRateUsPopup rateUs = new MNRateUsPopup ("Rate Mirawarri", "If you like Mirawarri let us know!", "Rate Us", "Maybe Later", "No, Thanks");
				rateUs.SetAppleId (appleAppId);
				rateUs.SetAndroidAppUrl (androidAppId);
				rateUs.AddDeclineListener (() => { 
					if (DEBUG) Debug.Log ("rate us declined"); 
					hasChosenToRate = -1;
				});
				rateUs.AddRemindListener (() => { 
					if (DEBUG) Debug.Log ("remind me later"); 
					hasChosenToRate = 1;
				});
				rateUs.AddRateUsListener (() => { 
					if (DEBUG) Debug.Log ("rate us!!!"); 
					hasChosenToRate = 2;
				});
				rateUs.AddDismissListener (() => { 
					if (DEBUG) Debug.Log ("rate us dialog dismissed :("); 
					hasChosenToRate = 1;
				});
				rateUs.Show ();
			}
			PlayerPrefs.SetInt ("hasChosenToRate", hasChosenToRate);
		}
	}






	// POPUP/DIALOG: Promt user to complete survey
	public void SurveyPrompt(){
		//if (DEBUG) Debug.Log ("SurveyPrompt()");
	
		if (!Application.isEditor){

			// crossplatform popup/dialog
			MNPopup popup = new MNPopup ("Hello beta tester!", "Would you like to take a short survey?");
			// options
			popup.AddDismissListener (() => { /*if (DEBUG) Debug.Log ("dismiss listener");*/ }); // do nothing
			popup.AddAction ("No", () => { /*if (DEBUG) Debug.Log ("No action callback");*/ });  // do nothing
			popup.AddAction ("Yes", () => { //if (DEBUG) Debug.Log ("Yes action callback");
				hasChosenToSurvey = 1;
				PlayerPrefs.SetInt ("hasChosenToSurvey", hasChosenToSurvey);
				Application.OpenURL("https://docs.google.com/forms/d/1L0TbDqyyTk4tb-wY--mjMY7TGrbPQJf7Xs1OQT65rNw/edit");
			});
			popup.Show ();
		}

	}
	/*
	// OLD
	// prompt user to rate app / complete survey
		public void SurveyPromptOLD(){
		if (!Application.isEditor) {
			// create dialog
			IOSDialog dialog = IOSDialog.Create ("Hello beta tester!", "Would you like to take a short survey?");
			// add listener
			dialog.OnComplete += onSurveyPromptClose;
		}
	}
	private void onSurveyPromptClose(IOSDialogResult result) {
		//parsing result
		switch(result) {
			case IOSDialogResult.YES:
				//if (DEBUG) Debug.Log ("Yes button pressed");
				hasChosenToSurvey = 1;
				PlayerPrefs.SetInt ("hasChosenToSurvey", hasChosenToSurvey);
				Application.OpenURL("https://docs.google.com/forms/d/1L0TbDqyyTk4tb-wY--mjMY7TGrbPQJf7Xs1OQT65rNw/edit");
				break;
			case IOSDialogResult.NO:
				//if (DEBUG) Debug.Log ("No button pressed");
				// stay where you are
				break;
		}
	}
	*/



	#endregion

}
