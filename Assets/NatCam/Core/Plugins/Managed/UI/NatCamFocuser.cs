/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.UI {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using Utilities;

    [CoreDoc(195), RequireComponent(typeof(EventTrigger), typeof(Graphic))]
    public sealed class NatCamFocuser : MonoBehaviour, IPointerUpHandler {

        /// <summary>
        /// Are focus gestures being tracked?
        /// </summary>
        [CoreDoc(196)] public bool IsTracking {get; private set;}


        #region --Client API--

        /// <summary>
        /// Start tracking focus gestures on the UI panel that this is attached to
        /// </summary>
        /// <param name="focusMode">Focus mode to apply to the camera. Note that this must have the FocusMode.TapToFocus bit set for tap to focus to work</param>
        [CoreDoc(197)]
        public void StartTracking (FocusMode focusMode = FocusMode.TapToFocus) {
            if (!NatCam.Camera) return;
            NatCam.Camera.FocusMode = focusMode;
            this.IsTracking = true;
        }

        /// <summary>
        /// Stop tracking focus gestures
        /// </summary>
        [CoreDoc(198)]
        public void StopTracking () {
            IsTracking = false;
        }
        #endregion


        #region --UI Callbacks--

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
            if (IsTracking && NatCam.Camera) NatCam.Camera.SetFocus(Camera.main.ScreenToViewportPoint(eventData.pressPosition));
        }
        #endregion
    }
}