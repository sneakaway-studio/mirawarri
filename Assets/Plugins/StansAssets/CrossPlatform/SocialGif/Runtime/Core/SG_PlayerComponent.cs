using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SA.GIF {
	[RequireComponent(typeof(Image)), DisallowMultipleComponent]
	internal class PlayerComponent : MonoBehaviour {

		public Sprite[] sequence;

		private Image destination;
		private bool play = false;
		private float startTime = 0.0f;
		private int index = 0;
		private float diff = 0.0f;

		// Use this for initialization
		void Start () {
			destination = gameObject.GetComponent<Image> ();
		}
		
		// Update is called once per frame
		void Update () {
			if (play) {
				float dt = Time.realtimeSinceStartup - startTime;
				if (dt >= diff) {
					index = index >= sequence.Length - 1 ? 0 : index + 1;
					startTime = Time.realtimeSinceStartup;
				}

				destination.sprite = sequence [index];
			}
		}

		public void Play() {
			play = true;
			startTime = Time.realtimeSinceStartup;
		}

		public void SetParameters(int fps, Sprite[] sprites) {
			diff = 1.0f / fps;
			sequence = sprites;
		}
	}
}
