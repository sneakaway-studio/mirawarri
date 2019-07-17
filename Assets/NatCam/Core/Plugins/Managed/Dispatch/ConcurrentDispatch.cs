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
    [CoreDoc(83)]
    #endif
    public sealed class ConcurrentDispatch : IDispatch {

        #region --Op vars--
        private EventWaitHandle waitHandle = new AutoResetEvent(false);
        private bool running;
        private readonly object runFence = new object();
        #endregion


        #region --Ctor--

        /// <summary>
        /// Creates a dispatcher that will execute on a worker thread
        /// </summary>
        #if NATCAM_CORE
        [CoreDoc(84)]
        #endif
        public ConcurrentDispatch () : base () {
            lock (runFence) running = true;
            thread = new Thread(Update);
            thread.Start();
            DispatchUtility.onFrame += Notify;
            Debug.Log("NatCam Dispatch: Initialized concurrent dispatch");
        }
        #endregion


        #region --Client API--

        /// <summary>
        /// Release the dispatcher and free its worker thread
        /// </summary>
        #if NATCAM_CORE
        [CoreDoc(85)]
        #endif
        public override void Release () {
            DispatchUtility.onFrame -= Notify;
            lock (runFence) running = false;
            waitHandle.Set();
            thread.Join();
            base.Release();
        }
        #endregion


        #region --Callbacks--

        protected override void Update () {
            for (;;) {
                waitHandle.WaitOne();
                base.Update();
                lock (runFence) if (!running) break;
            }
            executing.Clear(); executing = null;
        }

        private void Notify () {
            waitHandle.Set();
        }
        #endregion
    }
}