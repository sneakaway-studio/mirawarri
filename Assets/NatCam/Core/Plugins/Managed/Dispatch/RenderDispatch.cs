/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Dispatch {

    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    #if NATCAM_CORE
    using Core.Utilities;
    #endif

    #if NATCAM_CORE
    [CoreDoc(88)]
    #endif
    public sealed class RenderDispatch : MainDispatch {

        #region --Ctor--

        /// <summary>
        /// Creates a dispatcher that will execute delegates on the render thread
        /// </summary>
        #if NATCAM_CORE
        [CoreDoc(89)]
        #endif
        public RenderDispatch () : base () {
            base.Dispatch(() => GL.IssuePluginEvent(NatCamRenderDelegate(), 0), true);
            Debug.Log("NatCam Dispatch: Initialized render dispatch");
        }
        #endregion


        #region --Client API--

        /// <summary>
        /// DO NOT USE
        /// </summary>
        #if NATCAM_CORE
        [CoreDoc(90)]
        #endif
        public override void Dispatch (Action action, bool repeat) {}
        #endregion


        #region --Native Interop--

        #if UNITY_IOS
        [DllImport("__Internal")]
        #else
        [DllImport("NatCamRenderDispatch")]
        #endif
        private static extern IntPtr NatCamRenderDelegate ();
        #endregion
    }
}