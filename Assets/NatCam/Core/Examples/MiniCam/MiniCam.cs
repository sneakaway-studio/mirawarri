/* 
*   NatCam
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCam.Examples
{

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class MiniCam : MonoBehaviour
    {

        [Header("Camera")]
        public bool useFrontCamera;

        [Header("UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;
        public Text flashText;
        public Button switchCamButton, flashButton;
        public Image checkIco, flashIco;

        private CameraDevice[] cameras;
        private int activeCamera = -1;
        private Texture previewTexture;
        private Texture2D photo;


        #region --Unity Messages--

        // Use this for initialization
        private void Start()
        {
            // Check permission
            cameras = CameraDevice.GetDevices();
            if (cameras == null)
            {
                Debug.Log("User has not granted camera permission");
                return;
            }
            // Pick camera
            for (var i = 0; i < cameras.Length; i++)
                if (cameras[i].IsFrontFacing == useFrontCamera)
                {
                    activeCamera = i;
                    break;
                }
            if (activeCamera == -1)
            {
                Debug.LogError("Camera is null. Consider using " + (useFrontCamera ? "rear" : "front") + " camera");
                return;
            }
            // Start preview
            cameras[activeCamera].StartPreview(OnStart);
        }
        #endregion


        #region --Callbacks--

        private void OnStart(Texture preview)
        {
            // Display the preview
            previewTexture = preview;
            rawImage.texture = preview;
            aspectFitter.aspectRatio = preview.width / (float)preview.height;
            // Set flash to auto
            cameras[activeCamera].FlashMode = FlashMode.Auto;
            UpdateFlashIcon();
        }

        private void OnPhoto(Texture2D photo)
        {
            // Save the photo
            this.photo = photo;
            // Display the photo
            rawImage.texture = photo;
            // Scale the panel to match aspect ratios
            aspectFitter.aspectRatio = photo.width / (float)photo.height;
            // Enable the check icon
            checkIco.gameObject.SetActive(true);
            // Disable the switch camera button
            switchCamButton.gameObject.SetActive(false);
            // Disable the flash button
            flashButton.gameObject.SetActive(false);
        }

        private void OnView()
        {
            // Disable the check icon
            checkIco.gameObject.SetActive(false);
            // Display the preview
            rawImage.texture = previewTexture;
            // Scale the panel to match aspect ratios
            aspectFitter.aspectRatio = previewTexture.width / (float)previewTexture.height;
            // Enable the switch camera button
            switchCamButton.gameObject.SetActive(true);
            // Enable the flash button
            flashButton.gameObject.SetActive(true);
            // Free the photo texture
            Texture2D.Destroy(photo); photo = null;
        }
        #endregion


        #region --UI Ops--

        public virtual void CapturePhoto()
        {
            // Divert control if we are checking the captured photo
            if (!checkIco.gameObject.activeInHierarchy)
                cameras[activeCamera].CapturePhoto(OnPhoto);
            // Check captured photo
            else OnView();
        }

        public void SwitchCamera()
        {
            cameras[activeCamera].StopPreview();
            activeCamera = (activeCamera + 1) % cameras.Length;
            cameras[activeCamera].StartPreview(OnStart);
        }

        public void ToggleFlashMode()
        {
            // Set the active camera's flash mode
            if (cameras[activeCamera].IsFlashSupported)
                switch (cameras[activeCamera].FlashMode)
                {
                    case FlashMode.Auto: cameras[activeCamera].FlashMode = FlashMode.On; break;
                    case FlashMode.On: cameras[activeCamera].FlashMode = FlashMode.Off; break;
                    case FlashMode.Off: cameras[activeCamera].FlashMode = FlashMode.Auto; break;
                }
            // Set the flash icon
            UpdateFlashIcon();
        }

        public void FocusCamera(BaseEventData e)
        {
            // Get the touch position in viewport coordinates
            var eventData = e as PointerEventData;
            RectTransform transform = eventData.pointerPress.GetComponent<RectTransform>();
            Vector3 worldPoint;
            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(transform, eventData.pressPosition, eventData.pressEventCamera, out worldPoint))
                return;
            var corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            var point = worldPoint - corners[0];
            var size = new Vector2(corners[3].x, corners[1].y) - (Vector2)corners[0];
            Vector2 relativePoint = new Vector2(point.x / size.x, point.y / size.y);
            // Set the focus point
            cameras[activeCamera].FocusPoint = relativePoint;
        }
        #endregion


        #region --Utility--

        private void UpdateFlashIcon()
        {
            // Set the icon
            bool supported = cameras[activeCamera].IsFlashSupported;
            flashIco.color = !supported || cameras[activeCamera].FlashMode == FlashMode.Off ? (Color)new Color32(120, 120, 120, 255) : Color.white;
            // Set the auto text for flash
            flashText.text = supported && cameras[activeCamera].FlashMode == FlashMode.Auto ? "A" : "";
        }
        #endregion
    }
}