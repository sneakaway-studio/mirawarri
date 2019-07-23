using System;
using System.Collections.Generic;

using UnityEngine;

using SA.Foundation.Templates;
using SA.iOS.UIKit;
using SA.iOS.AVFoundation;
using System.IO;

namespace SA.CrossPlatform.App
{
    internal class UM_IOSGalleryService : UM_AbstractGalleryService, UM_iGalleryService
    {
        public void PickImage(int imageSize, Action<UM_MediaResult> callback) 
        {
            UM_IOSCameraService.CaptureMedia(imageSize, UM_MediaType.Image, ISN_UIImagePickerControllerSourceType.Album, callback);
        }

        public void PickVideo(int thumbnailSize, Action<UM_MediaResult> callback) 
        {
            UM_IOSCameraService.CaptureMedia(thumbnailSize, UM_MediaType.Video, ISN_UIImagePickerControllerSourceType.Album, callback);
        }

       
        public override void SaveImage(Texture2D image, string fileName, Action<SA_Result> callback) 
        {
            ISN_UIImagePickerController.SaveTextureToCameraRoll(image, callback);
        }
    }
}