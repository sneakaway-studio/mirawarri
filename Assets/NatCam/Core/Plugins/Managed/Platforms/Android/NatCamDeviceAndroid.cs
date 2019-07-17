/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;

namespace NatCamU.Core.Platforms {

    public class NatCamDeviceAndroid : INatCamDevice {

        #region --Properties--
        public bool IsRearFacing (int camera) {
            return this[camera].Call<bool>("isRearFacing");
        }

        public bool IsFlashSupported (int camera) {
            return this[camera].Call<bool>("isFlashSupported");
        }

        public bool IsTorchSupported (int camera) {
            return this[camera].Call<bool>("isTorchSupported");
        }

        public float HorizontalFOV (int camera) {
            return this[camera].Call<float>("horizontalFOV");
        }

        public float VerticalFOV (int camera) {
            return this[camera].Call<float>("verticalFOV");
        }

        public float MinExposureBias (int camera) {
            return this[camera].Call<float>("minExposureBias");
        }

        public float MaxExposureBias (int camera) {
            return this[camera].Call<float>("maxExposureBias");
        }

        public float MaxZoomRatio (int camera) {
            return this[camera].Call<float>("maxZoomRatio");
        }
        #endregion


        #region --Getters--
        public void GetPreviewResolution (int camera, out int width, out int height) {
            width = height = 0;
            AndroidJavaObject jRet = this[camera].Call<AndroidJavaObject>("getPreviewResolution");
            if (jRet.GetRawObject().ToInt32() != 0) {
                int[] res = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jRet.GetRawObject());
                width = res[0]; height = res[1];
            }
        }

        public void GetPhotoResolution (int camera, out int width, out int height) {
            width = height = 0;
            AndroidJavaObject jRet = this[camera].Call<AndroidJavaObject>("getPhotoResolution");
            if (jRet.GetRawObject().ToInt32() != 0) {
                int[] res = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jRet.GetRawObject());
                width = res[0]; height = res[1];
            }
        }

        public float GetFramerate (int camera) {
            return this[camera].Call<float>("getFramerate");
        }
        
        public float GetExposure (int camera) {
            return this[camera].Call<float>("getExposure");
        }

        public int GetExposureMode (int camera) {
            return this[camera].Call<int>("getExposureMode");
        }

        public int GetFocusMode (int camera) {
            return this[camera].Call<int>("getFocusMode");
        }

        public int GetFlash (int camera) {
            return this[camera].Call<int>("getFlash");
        }

        public int GetTorch (int camera) {
            return this[camera].Call<int>("getTorch");
        }
        
        public float GetZoom (int camera) {
            return this[camera].Call<float>("getZoom");
        }
        #endregion


        #region --Setters--
        public void SetPreviewResolution (int camera, int width, int height) {
            this[camera].Call("setResolution", width, height);
        }

        public void SetPhotoResolution (int camera, int width, int height) {
            this[camera].Call("setPhotoResolution", width, height);
        }

        public void SetFramerate (int camera, float framerate) {
            this[camera].Call("setFramerate", framerate);
        }

        public void SetFocus (int camera, float x, float y) {
            this[camera].Call("setFocus", x, y);
        }

        public void SetExposure (int camera, float bias) {
            this[camera].Call("setExposure", (int)bias);
        }

        public void SetExposureMode (int camera, int state) {
            this[camera].Call("setExposureMode", state);
        }

        public void SetFocusMode (int camera, int state) {
            this[camera].Call("setFocusMode", state);
        }

        public void SetFlash (int camera, int state) {
            this[camera].Call("setFlash", state);
        }

        public void SetTorch (int camera, int state) {
            this[camera].Call("setTorch", state);
        }
        public void SetZoom (int camera, float ratio) {
            this[camera].Call("setZoom", ratio);
        }
        #endregion
        

        #region --Interop--

        private readonly AndroidJavaClass natcamdevice;

        public NatCamDeviceAndroid () {
            natcamdevice = new AndroidJavaClass("com.yusufolokoba.natcam.NatCamDevice");
        }

        public AndroidJavaObject this [int index] {
            get {
                return natcamdevice.CallStatic<AndroidJavaObject>("getCamera", index);
            }
        }
        #endregion
    }
}