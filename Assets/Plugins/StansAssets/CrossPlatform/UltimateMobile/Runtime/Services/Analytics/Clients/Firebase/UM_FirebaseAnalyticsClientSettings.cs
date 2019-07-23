using System;

namespace SA.CrossPlatform.Analytics
{
    [Serializable]
    public class UM_FirebaseAnalyticsClientSettings
    {
        /// <summary>
        /// Sets the duration of inactivity that terminates the current session. 
        /// The default value is (30 minutes).
        /// </summary>
        public int SessionTimeoutDuration = 30;
    }
}