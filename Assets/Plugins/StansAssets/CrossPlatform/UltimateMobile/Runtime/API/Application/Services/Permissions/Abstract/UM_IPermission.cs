using System;

namespace SA.CrossPlatform.App
{
    public interface UM_IPermission
    {
        /// <summary>
        /// Requests the user’s permission, if needed.
        ///
        /// After the user grants permission, the system remembers the choice for future use in your app, 
        /// but the user can change this choice at any time using the Settings app.
        /// If user denied your app access, this choice will also be remembered and attempt to call
        /// RequestAccess again will not display any permission prompt and failed result will be fired immediately. 
        /// </summary>
        /// <param name="callback">Callback fired upon determining your app’s authorization to access requested permission.</param>
        void RequestAccess(Action<AuthorizationStatus> callback);
    }
}