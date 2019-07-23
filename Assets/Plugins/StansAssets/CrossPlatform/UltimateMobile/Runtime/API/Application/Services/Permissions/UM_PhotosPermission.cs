using System;
using SA.Android.Manifest;
using SA.iOS.Photos;

namespace  SA.CrossPlatform.App
{
    internal class UM_PhotosPermission : UM_Permission  {

        protected override AuthorizationStatus IOSAuthorization
        {
            get
            {
                var status = ISN_PHPhotoLibrary.AuthorizationStatus;
                switch (status)
                {
                    case ISN_PHAuthorizationStatus.Authorized:
                        return AuthorizationStatus.Granted;
                    default:
                        return AuthorizationStatus.Denied;
                }
            }
        }

        protected override void IOSRequestAccess(Action<AuthorizationStatus> callback)
        {
            ISN_PHPhotoLibrary.RequestAuthorization((status) => {
                if(status == ISN_PHAuthorizationStatus.Authorized) {
                    callback.Invoke(AuthorizationStatus.Granted);
                } else {
                    callback.Invoke(AuthorizationStatus.Denied);
                }
            });
        }

        protected override AMM_ManifestPermission[] AndroidPermissions
        {
            get { return new []{ AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE, AMM_ManifestPermission.READ_EXTERNAL_STORAGE }; }
        }
    }

}


