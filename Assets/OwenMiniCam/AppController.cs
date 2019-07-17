using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AppController : MonoBehaviour {

	// whether we are currently editing a regular sticker
	public static int FingersGestureInProgress = 0;
	// whether a menu is open (turn off sticker editing)
	public static bool FingersGestureAllowed = false;

	// Launch a webpage in a separat browser 
	public void LaunchUrl(string url){
		Application.OpenURL(url);
	}


	public static void RemoveAllTexturesFromMemory(){

		/*
		 * // THIS IS CAUSING AN ERROR IN THE BUILD ONLY :-(*/
		// find all loaded textures
		Texture2D[] ts = Resources.FindObjectsOfTypeAll(typeof(Texture2D)) as Texture2D[];
		Debug.Log("Number of all loaded Textures " + ts.Length);
		// loop through them
		foreach (Texture2D t in ts) {
			// unload those with frame or sticker in the name
			if (t.name.Contains ("Sticker_") || (t.name.Contains ("Frame-") && t.name.Contains ("1024"))) {
				//Debug.Log ("t.name = " + t.name + " // t.width = " + t.width + " // t.height = " + t.height + " // t.ToString() = " + t.ToString () + " // t.GetInstanceID() = " + t.GetInstanceID ());
				Resources.UnloadAsset (t);
			}
		}

	}


}
