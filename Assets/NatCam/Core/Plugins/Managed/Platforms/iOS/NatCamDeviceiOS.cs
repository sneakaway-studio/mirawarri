/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    public class NatCamDeviceiOS : INatCamDevice {

        #region --Properties--
        public bool IsRearFacing (int camera) {
            return camera.IsRearFacing();
        }

        public bool IsFlashSupported (int camera) {
            return camera.IsFlashSupported();
        }

        public bool IsTorchSupported (int camera) {
            return camera.IsTorchSupported();
        }

        public float HorizontalFOV (int camera) {
            return camera.HorizontalFOV();
        }

        public float VerticalFOV (int camera) {
            return camera.VerticalFOV();
        }

        public float MinExposureBias (int camera) {
            return camera.MinExposureBias();
        }

        public float MaxExposureBias (int camera) {
            return camera.MaxExposureBias();
        }

        public float MaxZoomRatio (int camera) {
            return camera.MaxZoomRatio();
        }
        #endregion


        #region --Getters--
        public void GetPreviewResolution (int camera, out int width, out int height) {
            camera.GetPreviewResolution(out width, out height);
        }

        public void GetPhotoResolution (int camera, out int width, out int height) {
            camera.GetPhotoResolution(out width, out height);
        }

        public float GetFramerate (int camera) {
            return camera.GetFramerate();
        }
        
        public float GetExposure (int camera) {
            return camera.GetExposure();
        }
        public int GetExposureMode (int camera) {
            return camera.GetExposureMode();
        }
        public int GetFocusMode (int camera) {
            return camera.GetFocusMode();
        }
        public int GetFlash (int camera) {
            return camera.GetFlash();
        }
        public int GetTorch (int camera) {
            return camera.GetTorch();
        }
        public float GetZoom (int camera) {
            return camera.GetZoom();
        }
        #endregion


        #region --Setters--
        
        public void SetPreviewResolution (int camera, int width, int height) {
            camera.SetPreviewResolution(width, height);
        }

        public void SetPhotoResolution (int camera, int width, int height) {
            camera.SetPhotoResolution(width, height);
        }

        public void SetFramerate (int camera, float framerate) {
            camera.SetFramerate(framerate);
        }

        public void SetFocus (int camera, float x, float y) {
            camera.SetFocus(x, y);
        }

        public void SetExposure (int camera, float bias) {
            camera.SetExposure(bias);
        }

        public void SetExposureMode (int camera, int state) {
            camera.SetExposureMode(state);
        }

        public void SetFocusMode (int camera, int state) {
            camera.SetFocusMode(state);
        }

        public void SetFlash (int camera, int state) {
            camera.SetFlash(state);
        }

        public void SetTorch (int camera, int state) {
            camera.SetTorch(state);
        }
        public void SetZoom (int camera, float ratio) {
            camera.SetZoom(ratio);
        }
        #endregion
    }
}