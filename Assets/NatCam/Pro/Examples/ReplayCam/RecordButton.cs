﻿/* 
*   NatCam Pro
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	[RequireComponent(typeof(EventTrigger))]
	public class RecordButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		public ReplayCam replayCam;
		public Image button, countdown;
		private bool pressed;
		private const float MaxRecordingTime = 6f; // Seconds

		private void Start () {
			Reset ();
		}

		private void Reset () {
			// Reset fill amounts
			button.fillAmount = 1.0f;
			countdown.fillAmount = 0.0f;
		}

		void IPointerDownHandler.OnPointerDown (PointerEventData eventData) {
			// Start counting
			StartCoroutine (Countdown ());
		}

		void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
			// Reset pressed
			pressed = false;
		}

		private IEnumerator Countdown () {
			pressed = true;
			// First wait a short time to make sure it's not a tap
			yield return new WaitForSeconds(0.2f);
			if (!pressed) yield break;
			#if NATCAM_PRO || NATCAM_PROFESSIONAL
			// Start recording
			replayCam.StartRecording();
			// Animate the countdown
			float startTime = Time.time, ratio = 0f;
			while (pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f) {
				countdown.fillAmount = ratio;
				button.fillAmount = 1f - ratio;
				yield return null;
			}
			// Reset
			Reset();
			// Stop recording
			replayCam.StopRecording();
			#endif
		}
	}
}