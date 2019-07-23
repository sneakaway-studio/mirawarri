using System;
using SA.Android.App;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
using UnityEngine;

namespace SA.CrossPlatform.App
{
    internal abstract class UM_Permission : UM_IPermission
    {
        protected abstract AuthorizationStatus IOSAuthorization { get; }
        protected abstract void IOSRequestAccess(Action<AuthorizationStatus> callback);
        protected abstract AMM_ManifestPermission[] AndroidPermissions { get; }

        public AuthorizationStatus Authorization
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        var granted = true;
                        foreach (var permission in AndroidPermissions)
                        {
                            var state =  AN_PermissionsManager.CheckSelfPermission(permission);
                            if (state == AN_PackageManager.PermissionState.Denied)
                            {
                                granted = false;
                                break;
                            }
                        }

                        if (granted)
                        {
                            return AuthorizationStatus.Granted;
                        }
                        else
                        {
                            return AuthorizationStatus.Denied;
                        }
                        
                    case RuntimePlatform.IPhonePlayer:
                        return IOSAuthorization;
                    default:
                        return AuthorizationStatus.Granted;
                }
            }
        }

        public void RequestAccess(Action<AuthorizationStatus> callback)
        {
            if (Authorization == AuthorizationStatus.Granted)
            {
                callback.Invoke(AuthorizationStatus.Granted);
                return;
            }
            StartRequestAccessFlow(callback);
        }
        
        
        private void StartRequestAccessFlow(Action<AuthorizationStatus> callback)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    IOSRequestAccess(callback);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    AN_PermissionsUtility.TryToResolvePermission(AndroidPermissions, (granted) =>
                    {
                        if (granted)
                        {
                            callback.Invoke(AuthorizationStatus.Granted);
                        }
                        else
                        {
                            callback.Invoke(AuthorizationStatus.Denied);
                        }
                    });
                    break;
                default:
                    UM_EditorAPIEmulator.WaitForNetwork(() => { callback.Invoke(AuthorizationStatus.Granted); });
                    break;
            }
        }
    }
}