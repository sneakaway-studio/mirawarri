using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;

namespace SA.GIF {
	public sealed class SG_Player
	{

		private SG_Recorder Recorder;
		private PlayerComponent Player = null;

		public SG_Player(SG_Recorder recorder, Image destination)
		{
			Recorder = recorder;
			Player = destination.gameObject.GetComponent<PlayerComponent> ();
			if (Player == null) 
			{
				Player = destination.gameObject.AddComponent<PlayerComponent> ();
			}
		}

		public void Play() 
		{
			Assert.IsNotNull(Player);
			UpdatePlayerParams();
			Player.Play ();
		}

		private void UpdatePlayerParams()
		{
			var sprites = new Sprite[Recorder.Frames.Length];

			for (var i = 0; i < Recorder.Frames.Length; i++) 
			{
				var tex = new Texture2D (Recorder.Frames [i].width, Recorder.Frames [i].height);
				RenderTexture.active = Recorder.Frames [i];
				tex.ReadPixels (new Rect (0.0f, 0.0f, Recorder.Frames [i].width, Recorder.Frames [i].height), 0, 0);
				tex.Apply ();

				sprites[i] = Sprite.Create (tex, new Rect (0.0f, 0.0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
			}
			Player.SetParameters (Recorder.FPS, sprites);
		}
	}
}
