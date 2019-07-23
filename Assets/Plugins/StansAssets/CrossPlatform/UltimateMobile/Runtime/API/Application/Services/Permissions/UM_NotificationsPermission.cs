using System;
using SA.Android.Manifest;
using SA.iOS.UserNotifications;

namespace  SA.CrossPlatform.App
{
    internal class UM_NotificationsPermission : UM_Permission  {

        protected override AuthorizationStatus IOSAuthorization
        {
            get
            {
                return AuthorizationStatus.Denied;
            }
        }

        protected override void IOSRequestAccess(Action<AuthorizationStatus> callback)
        {
            int options = ISN_UNAuthorizationOptions.Alert | ISN_UNAuthorizationOptions.Sound;
            ISN_UNUserNotificationCenter.RequestAuthorization(options, (result) => {
                if (result.IsSucceeded) {
                    callback.Invoke(AuthorizationStatus.Granted);
                } else {
                    callback.Invoke(AuthorizationStatus.Denied);
                }
            });
        }

        protected override AMM_ManifestPermission[] AndroidPermissions
        {
            get { return new []{ AMM_ManifestPermission.WAKE_LOCK }; }
        }
    }

}


