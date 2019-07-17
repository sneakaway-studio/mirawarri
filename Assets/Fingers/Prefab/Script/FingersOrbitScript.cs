using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    public class FingersOrbitScript : MonoBehaviour
    {
        [Tooltip("The transform to orbit around.")]
        public Transform OrbitTarget;

        [Tooltip("The object to orbit around OrbitTarget.")]
        public Transform Orbiter;

        [Tooltip("The minimium distance to zoom towards to the orbit target.")]
        [Range(0.1f, 100.0f)]
        public float MinZoomDistance = 5.0f;

        [Tooltip("The maximum distance to zoom away from the orbit target.")]
        [Range(0.1f, 1000.0f)]
        public float MaxZoomDistance = 1000.0f;

        [Tooltip("The speed at which to orbit using x delta pan gesture values. Negative or positive values will cause orbit in the opposite direction.")]
        [Range(-10.0f, 10.0f)]
        public float OrbitXSpeed = -2.0f;

        [Tooltip("The speed at which to orbit using y delta pan gesture values. Negative or positive values will cause orbit in the opposite direction.")]
        [Range(-10.0f, 10.0f)]
        public float OrbitYSpeed = -2.0f;

        [Tooltip("Whether to allow orbit while zooming.")]
        public bool AllowOrbitWhileZooming = true;
        private bool allowOrbitWhileZooming;

        private ScaleGestureRecognizer scaleGesture;
        private PanGestureRecognizer panGesture;
        private TapGestureRecognizer tapGesture;

        public event System.Action OrbitTargetTapped;

        private void Start()
        {
            // create a scale gesture to zoom orbiter in and out
            scaleGesture = new ScaleGestureRecognizer();
            scaleGesture.Updated += ScaleGesture_Updated;

            // pan gesture
            panGesture = new PanGestureRecognizer();
            panGesture.MaximumNumberOfTouchesToTrack = 2;
            panGesture.Updated += PanGesture_Updated;

            // create a tap gesture that only executes on the target, note that this requires a physics ray caster on the camera
            tapGesture = new TapGestureRecognizer();
            tapGesture.Updated += TapGesture_Updated;
            tapGesture.PlatformSpecificView = OrbitTarget;

            FingersScript.Instance.AddGesture(scaleGesture);
            FingersScript.Instance.AddGesture(panGesture);
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void Update()
        {
            if (allowOrbitWhileZooming != AllowOrbitWhileZooming)
            {
                allowOrbitWhileZooming = AllowOrbitWhileZooming;
                if (allowOrbitWhileZooming)
                {
                    scaleGesture.AllowSimultaneousExecution(panGesture);
                }
                else
                {
                    scaleGesture.DisallowSimultaneousExecution(panGesture);
                }
            }
        }

        private void TapGesture_Updated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                Debug.Log("Orbit target tapped!");
                if (OrbitTargetTapped != null)
                {
                    OrbitTargetTapped.Invoke();
                }
            }
        }

        private void PanGesture_Updated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            // if gesture is not executing, exit function
            if (gesture.State != GestureRecognizerState.Executing)
            {
                return;
            }

            // orbit the target in either direction depending on pan gesture delta x and y
            if (OrbitXSpeed != 0.0f)
            {
                Orbiter.RotateAround(OrbitTarget.transform.position, Vector3.up, panGesture.DeltaX * OrbitXSpeed);
            }
            if (OrbitYSpeed != 0.0f)
            {
                Orbiter.RotateAround(OrbitTarget.transform.position, Orbiter.transform.right, panGesture.DeltaY * OrbitYSpeed);
            }
        }

        private void ScaleGesture_Updated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            // if gesture is not executing, exit function
            if (gesture.State != GestureRecognizerState.Executing)
            {
                return;
            }

            // point oribiter at target
            Orbiter.transform.LookAt(OrbitTarget.transform);

            // get the current distance from the target
            float currentDistanceFromTarget = Vector3.Distance(Orbiter.transform.position, OrbitTarget.transform.position);

            // invert the scale so that smaller scales actually zoom out and larger scales zoom in
            float scale = 1.0f + (1.0f - scaleGesture.ScaleMultiplier);

            // multiply by scale, clamping to min and max
            currentDistanceFromTarget = Mathf.Clamp(currentDistanceFromTarget * scale, MinZoomDistance, MaxZoomDistance);

            // position orbiter away from the target at the new distance
            Orbiter.transform.position = Orbiter.transform.forward * (-currentDistanceFromTarget);
        }
    }
}