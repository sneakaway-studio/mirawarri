/* // COMMENTING TO TRY TO SPEED THINGS UP, SCRIPT WORKS THO (I THINK)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *	Allows you to apply a Texture2D from a Spritesheet to a UI RawImage 
 *	1. Enable Read/Write enabled on the Spritesheet
 *	2. Attach to UI Image or RawImage objects in the scene  
 * 	3. Add a Sprite (from a Spritesheet) to the sprite placeholder on the script.
 * 	Works in Edit Mode: https://docs.unity3d.com/ScriptReference/ExecuteInEditMode.html
 *

[ExecuteInEditMode]
public class UseSpriteAsTexture : MonoBehaviour {

	Image image;
	RawImage ri;
	public Sprite sprite;

	void Awake(){
		Debug.Log("Editor causes this Awake()");
		UpdateTexture ();
	}
	void Start () {
	}
	void OnGUI() {
		Debug.Log("Editor causes this OnGUI()");
		UpdateTexture ();
	}

	void UpdateTexture(){
		// get RawImage component of UI object
		if (gameObject.GetComponent<RawImage> () != null) {
			ri = gameObject.GetComponent<RawImage> ();
			// this only inserts the whole spritesheet as the texture
			//ri.texture = sprite.texture;
			// use this function to get individual sprites as textures instead
			ri.texture = ConvertSpriteToTexture (sprite);
			Debug.Log ("ri.texture updated");
		} else if (gameObject.GetComponent<Image> () != null) {
			image = gameObject.GetComponent<Image> ();
			image.sprite = sprite;
		} else {
			// nothing
		}

	}

	// Add Texture2D from Sprite to UI RawImage
	// credit: http://answers.unity3d.com/answers/817900/view.html
	public static Texture2D textureFromSprite(Sprite sprite)
	{
		try
		{
			if(sprite.rect.width != sprite.texture.width)
			{
				Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
				Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
					(int)sprite.textureRect.y, 
					(int)sprite.textureRect.width, 
					(int)sprite.textureRect.height );
				newText.SetPixels(newColors);
				newText.Apply();
				return newText;
			} else
				return sprite.texture;
		}catch
		{
			return sprite.texture;
		}
	}

	// maybe more exact? http://answers.unity3d.com/answers/1231714/view.html
	Texture2D ConvertSpriteToTexture(Sprite sprite)
	{
		try
		{
			if (sprite.rect.width != sprite.texture.width)
			{
				Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
				Color[] colors = newText.GetPixels();
				Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
					(int)System.Math.Ceiling(sprite.textureRect.y),
					(int)System.Math.Ceiling(sprite.textureRect.width),
					(int)System.Math.Ceiling(sprite.textureRect.height));
				//Debug.Log(colors.Length+"_"+ newColors.Length);
				newText.SetPixels(newColors);
				newText.Apply();
				return newText;
			}
			else
				return sprite.texture;
		}catch
		{
			return sprite.texture;
		}
	}




}

*/