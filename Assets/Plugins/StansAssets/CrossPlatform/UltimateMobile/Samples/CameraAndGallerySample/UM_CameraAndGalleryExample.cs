using System.IO;
using SA.CrossPlatform.App;
using SA.CrossPlatform.UI;
using SA.Foundation.Utility;
using UnityEngine;
using UnityEngine.UI;

public class UM_CameraAndGalleryExample : MonoBehaviour
{
   [SerializeField] Image m_Sprite = null;
   [SerializeField] RawImage m_Image = null;

   [Header("Camera")]
   [SerializeField] Button m_CaptureImage = null;
   [SerializeField] Button m_CaptureVideo = null;

   [Header("Gallery")]
   [SerializeField] Button m_PickImage = null;
   [SerializeField] Button m_PickVideo = null;
   [SerializeField] Button m_SaveScreenshot = null;
   [SerializeField] Button m_SaveBlackImage = null;

   private const int k_MaxThumbnailSize = 1024;
  
   void Awake()
   {
      AddFitter(m_Image.gameObject);
      AddFitter(m_Sprite.gameObject);
      
      m_CaptureVideo.onClick.AddListener(TakeVideo);
      m_CaptureImage.onClick.AddListener(TakePicture);

      m_PickImage.onClick.AddListener(PickImage);
      m_PickVideo.onClick.AddListener(PickVideo);
      m_SaveScreenshot.onClick.AddListener(SaveScreenshot);
      m_SaveBlackImage.onClick.AddListener(SaveImage);
   }
   
   private void AddFitter(GameObject go) 
   {
      var fitter = go.AddComponent<AspectRatioFitter>();
      fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
      fitter.aspectRatio = 1;
   }

   private void TakeVideo()
   {
      var cameraService = UM_Application.CameraService;
      cameraService.TakeVideo(k_MaxThumbnailSize,PrintResult);
   }
   
   private void TakePicture()
   {
      var cameraService = UM_Application.CameraService;
      cameraService.TakePicture(k_MaxThumbnailSize,PrintResult);
   }

   private void SaveScreenshot()
   {
      var gallery = UM_Application.GalleryService;
      gallery.SaveScreenshot("example_scene.png", (result) =>
      {
         if (result.IsSucceeded)
         {
            Debug.Log("Screenshot saved.");
         } 
         else 
         {
            Debug.Log("Failed to save a screenshot: " + result.Error.FullMessage);
         }
      });
   }

   private void SaveImage()
   {
      //Generating sample red texture with 32x32 resolution
      var gallery = UM_Application.GalleryService;
      var sampleBlackTexture = SA_IconManager.GetIcon(Color.black, 32, 32);
      gallery.SaveImage(sampleBlackTexture, "sample_black_image.png", (result) =>
      {
         if (result.IsSucceeded)
         {
            Debug.Log("Image saved.");
         } 
         else 
         {
            Debug.Log("Failed to save an Image: " + result.Error.FullMessage);
         }
      });
   }

   private void PickImage() {
      var gallery = UM_Application.GalleryService;
      gallery.PickImage(k_MaxThumbnailSize, PrintResult);
   }

   private void PickVideo() {
      var gallery = UM_Application.GalleryService;
      gallery.PickVideo(k_MaxThumbnailSize, PrintResult);
   }

   private void PrintResult(UM_MediaResult result)
   {
      if (result.IsSucceeded)
      {
            var media = result.Media;
            var mediaThumbnail = media.Thumbnail;
            Debug.Log("Thumbnail width: " + mediaThumbnail.width + " / height: " + mediaThumbnail.height);
            Debug.Log("media.Type: " + media.Type);
            Debug.Log("media.Path: " + media.Path);

            UM_DialogsUtility.ShowMessage("Success", "Image Info \n " +
                                                  "Thumbnail width: " + mediaThumbnail.width + " / height: " + mediaThumbnail.height + " \n " +
                                                  "media.Type: " + media.Type + " \n " +
                                                  "media.Path: " + media.Path);

            ApplyImageToGUI(mediaThumbnail);
            //path can be null when we taking a picture directly from a camera.
            //ios will not save it on a disk
            if(!string.IsNullOrEmpty(media.Path)) {
                byte[] movieBytes = File.ReadAllBytes(media.Path);
                Debug.Log("picked file bytes size : " + movieBytes.Length);
            }
        }
      else
      {
         UM_DialogsUtility.ShowMessage("Failed", result.Error.FullMessage);
      }
   }
   
   private void ApplyImageToGUI(Texture2D image) {

      if (m_Image.texture != null)
      {
         DestroyImmediate(m_Image.texture, true);
         DestroyImmediate(m_Sprite.sprite, true);
      }
      
      var aspectRatio =  (float)image.width / (float)image.height;

      m_Image.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
      m_Sprite.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;

      //m_image is a UnityEngine.UI.RawImage
      m_Image.texture = image;

      //m_sprite is a UnityEngine.UI.Image
      m_Sprite.sprite = image.ToSprite();
   }
}
