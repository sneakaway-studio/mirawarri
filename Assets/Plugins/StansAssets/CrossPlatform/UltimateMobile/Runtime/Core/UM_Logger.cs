using SA.Android.Utilities;
using SA.iOS.Utilities;
using UnityEngine;

namespace SA.CrossPlatform {
	
	public static class UM_Logger  {
	
		public static void Log(object message) 
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				AN_Logger.Log(message);
			}
			else
			{
				ISN_Logger.Log(message);
			}
		}

		public static void LogWarning(object message) 
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				AN_Logger.LogWarning(message);
			}
			else
			{
				ISN_Logger.LogWarning(message);
			}
		}

		public static void LogError(object message)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				AN_Logger.LogError(message);
			}
			else
			{
				ISN_Logger.LogError(message);
			}
		}
	}
}