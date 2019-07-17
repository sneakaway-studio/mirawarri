/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Dispatch {

    using UnityEngine;
    using System.Threading;
    #if NATCAM_CORE
    using Core.Utilities;
    #endif

    #if NATCAM_CORE
    [CoreDoc(86)]
    #endif
    public class MainDispatch : IDispatch {

        #region --Client API--

        /// <summary>
        /// Creates a dispatcher that will execute on the main thread
        /// </summary>
        #if NATCAM_CORE
        [CoreDoc(87)]
        #endif
        public MainDispatch () : base () {
            DispatchUtility.onFrame += Update;
            Debug.Log("NatCam Dispatch: Initialized main dispatch");
        }

        public override void Release () {
            DispatchUtility.onFrame -= Update;
            base.Release();
        }

        protected override void SafeRelease () {
            Dispatch(Release);
        }

        protected override void Update () {
            thread = Thread.CurrentThread;
            base.Update();
        }
        #endregion
    }
}