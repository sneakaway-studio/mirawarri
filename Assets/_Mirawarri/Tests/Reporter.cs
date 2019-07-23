/* // COMMENTING TO TRY TO SPEED THINGS UP, SCRIPT WORKS THO (I THINK)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reporter : MonoBehaviour {


	float screenW, screenH, canvasW, canvasH, worldW, worldH, 
	guiW, guiH, guiScaledW, guiScaledH, viewportW, viewportH;
	Vector2 mousePositionScreen, mousePositionWorld, mousePositionGUI, mousePositionGUIScaled, 
			mousePositionViewport, mousePositionCanvas;
	string report;
	public Canvas canvas;


	public GUIStyle guiStyle = new GUIStyle(); 

	void OnGUI() {


		// SCREEN - The same as Input.mousePosition 
		// 0,0 = BOTTOM-LEFT
		// width,height = TOP-RIGHT
		screenH = Screen.height;	// screen height
		screenW = Screen.width;		// screen width
		mousePositionScreen = new Vector2(Input.mousePosition.x, Input.mousePosition.y);


		#region --I DON'T KNOW IF THESE ARE CORRECT--
		// GUI - Dimensions for whatever this is attached to
		// 0,0 = TOP-LEFT
		// width,height = BOTTOM-RIGHT
		// maybe should be canvas http://answers.unity3d.com/questions/889220/unity-46-ui-canvas-width-height.html
		RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
		guiW = rectTransform.rect.width;
		guiH = rectTransform.rect.height;
		mousePositionGUI = Event.current.mousePosition;

		// GUI Scaled
		guiScaledW = rectTransform.rect.width;
		guiScaledH = rectTransform.rect.height;
		mousePositionGUIScaled = Event.current.mousePosition;

		// CANVAS - 
		// 0,0 = BOTTOM-LEFT
		// 1,1 = TOP-RIGHT
		canvasW = canvas.pixelRect.width;
		canvasH = canvas.pixelRect.height;
		mousePositionCanvas = new Vector2(Input.mousePosition.x * canvas.scaleFactor / Screen.width, Input.mousePosition.y * canvas.scaleFactor / Screen.height);
		#endregion



		// VIEWPORT - Normalized and relative to the camera
		// https://docs.unity3d.com/ScriptReference/Camera.ScreenToViewportPoint.html
		// 0,0 = BOTTOM-LEFT
		// 1,1 = TOP-RIGHT
		viewportW = 1.0f;
		viewportH = 1.0f;
		mousePositionViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);

		// WORLD
		// 0,0 = MIDDLE,MIDDLE
		// e.g. -3,5 = TOP-LEFT, 3,-5 = BOTTOM-RIGHT
		worldH = Camera.main.orthographicSize * 2.0f;	// camera (a.k.a.) world) height
		worldW = worldH * Camera.main.aspect;			// camera (a.k.a.) world) width
		mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// update report
		report = "" +
			"\t size \t\t position \n" +
			"Screen \t"+ screenW + "," + screenH + "\t\t" + mousePositionScreen + "\n" +
		
			//"GUI \t"+ guiW +"," +guiH + "\t" + mousePositionGUI + "\n" +
			//"GUI Scaled \t"+ guiScaledW +"," +guiScaledH + "\t" + mousePositionGUIScaled + "\n" +

			//"Canvas \t" + canvasW  +"," + canvasH + "\t\t" + mousePositionCanvas + "\n" +
			//"Canvas \t scaleFactor = " + canvas.scaleFactor  + "\n" +

			"Viewport \t"+ viewportW +"," + viewportH + "\t\t" + mousePositionViewport + "\n" +
			"World \t" + worldW +","+ worldH + "\t\t" + mousePositionWorld + "\n" +
			"Total frames: " + Time.frameCount;
		
		//Debug.Log (report);

		GUI.Label(new Rect(10, 10, 400, 20), report, guiStyle); 


		//GUI.Button ( new Rect(200,200,200,100), "200,200,200,100" );
	}





}

*/