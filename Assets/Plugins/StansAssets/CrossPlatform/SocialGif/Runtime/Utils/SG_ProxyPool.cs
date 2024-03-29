using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA.GIF {
	internal static class ProxyPool  {

		private const string CLASS_NAME = "com.stansassets.gif.Bridge";

		#if UNITY_ANDROID
		private static Dictionary<string, AndroidJavaObject> pool =  new Dictionary<string, AndroidJavaObject>();
		#endif

		public static void ShareFacebook(string message, string path, string apiKey) {
			CallStatic (CLASS_NAME, "FacebookShare", message, path, apiKey);
		}

		public static void ShareTwitter(string message, string path) {
			CallStatic (CLASS_NAME, "TwitterShare", message, path);
		}

		public static void CallStatic(string className, string methodName, params object[] args) {
			#if UNITY_ANDROID

			if(Application.platform != RuntimePlatform.Android) {
				return;
			}
			Debug.Log("SA: Using proxy for class: " + className + " method:" + methodName);

			try {

				AndroidJavaObject bridge;
				if(pool.ContainsKey(className)) {
					bridge = pool[className];
				} else {
					bridge = new AndroidJavaObject(className);
					pool.Add(className, bridge);

				}

				AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				act.Call("runOnUiThread", new AndroidJavaRunnable(() => { bridge.CallStatic(methodName, args); }));


			} catch(System.Exception ex) {
				Debug.LogWarning(ex.Message);
			}
			#endif
		}

		#if UNITY_ANDROID
		public static ReturnType CallStatic<ReturnType>(string className, string methodName, params object[] args) {

			Debug.Log("SA: Using proxy for class: " + className + " method:" + methodName);

			try {

				AndroidJavaObject bridge;
				if(pool.ContainsKey(className)) {
					bridge = pool[className];
				} else {
					bridge = new AndroidJavaObject(className);
					pool.Add(className, bridge);

				}

				return bridge.CallStatic<ReturnType>(methodName, args);

			} catch(System.Exception ex) {
				Debug.LogWarning(ex.Message);
			}

			return default(ReturnType);

		}
		#endif

	}
}
