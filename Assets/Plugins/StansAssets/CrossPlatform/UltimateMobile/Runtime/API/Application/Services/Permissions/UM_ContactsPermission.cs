using System;
using SA.Android.Manifest;
using SA.iOS.Contacts;

namespace  SA.CrossPlatform.App
{
    internal class UM_ContactsPermission : UM_Permission  {

        protected override AuthorizationStatus IOSAuthorization
        {
            get
            {
                var status =  ISN_CNContactStore.GetAuthorizationStatus(ISN_CNEntityType.Contacts);
                switch (status)
                {
                    case ISN_CNAuthorizationStatus.Authorized:
                        return AuthorizationStatus.Granted;
                    default:
                        return AuthorizationStatus.Denied;
                }
            }
        }

        protected override void IOSRequestAccess(Action<AuthorizationStatus> callback)
        {
            ISN_CNContactStore.RequestAccess(ISN_CNEntityType.Contacts, (result) => {
                if (result.IsSucceeded) {
                    callback.Invoke(AuthorizationStatus.Granted);
                } else {
                    callback.Invoke(AuthorizationStatus.Denied);
                }
            });
        }

        protected override AMM_ManifestPermission[] AndroidPermissions
        {
            get { return new []{ AMM_ManifestPermission.READ_CONTACTS }; }
        }
    }

}


