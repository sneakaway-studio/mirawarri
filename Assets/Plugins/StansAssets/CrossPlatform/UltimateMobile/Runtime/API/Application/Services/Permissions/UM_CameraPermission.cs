using System;
using SA.Android.Manifest;
using SA.iOS.AVFoundation;

namespace  SA.CrossPlatform.App
{
    internal class UM_CameraPermission : UM_Permission  {

        protected override AuthorizationStatus IOSAuthorization
        {
            get
            {
                var status = ISN_AVCaptureDevice.GetAuthorizationStatus(ISN_AVMediaType.Video);
                switch (status)
                {
                    case ISN_AVAuthorizationStatus.Authorized:
                        return AuthorizationStatus.Granted;
                    default:
                        return AuthorizationStatus.Denied;
                }
            }
        }

        protected override void IOSRequestAccess(Action<AuthorizationStatus> callback)
        {
            ISN_AVCaptureDevice.RequestAccess(ISN_AVMediaType.Video, (status) => {
                if(status == ISN_AVAuthorizationStatus.Authorized) {
                    callback.Invoke(AuthorizationStatus.Granted);
                } else {
                    callback.Invoke(AuthorizationStatus.Denied);
                }
            });
        }

        protected override AMM_ManifestPermission[] AndroidPermissions
        {
            get { return new []{ AMM_ManifestPermission.CAMERA, AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE, AMM_ManifestPermission.READ_EXTERNAL_STORAGE }; }
        }
    }

}


