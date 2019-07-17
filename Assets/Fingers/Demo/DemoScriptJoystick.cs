﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
	public class DemoScriptJoystick : MonoBehaviour
	{
		[Tooltip("Fingers Joystick Script")]
		public FingersJoystickScript JoystickScript;

		[Tooltip("Object to move with the joystick")]
		public GameObject Mover;

		[Tooltip("Units per second to move the square with joystick")]
		public float Speed = 250.0f;

		private void Start()
		{
			JoystickScript.JoystickExecuted = JoystickExecuted;
		}

		private void JoystickExecuted(FingersJoystickScript script, Vector2 amount)
		{
			Vector3 pos = Mover.transform.position;
			pos.x += (amount.x * Speed * Time.deltaTime);
			pos.y += (amount.y * Speed * Time.deltaTime);
			Mover.transform.position = pos;
		}
	}
}
