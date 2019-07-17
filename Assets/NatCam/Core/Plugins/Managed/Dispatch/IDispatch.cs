/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Dispatch {

    using UnityEngine;
    using System;
    using System.Threading;
    #if NATCAM_CORE
    using Core.Utilities;
    #endif
    using Queue = System.Collections.Generic.List<System.Action>;
    
    #if NATCAM_CORE
    [CoreDoc(80)]
    #endif
    public abstract class IDispatch : IDisposable {

        #region --Op vars--
        public Thread thread {get; protected set;}
        protected Queue pending, executing;
        protected readonly object queueLock = new object();
        #endregion


        #region --Client API--

        /// <summary>
        /// Dispatch a delegate to be invoked
        /// </summary>
        /// <param name="action">The delegate to be invoked</param>
        /// <param name="repeating">Optional. Should delegate be invoked repeatedly?</param>
        #if NATCAM_CORE
        [CoreDoc(81)]
        #endif
        public virtual void Dispatch (Action action, bool repeating = false) {
            if (action == null) return;
            Action actionWrapper = action;
            if (repeating) actionWrapper = delegate () {
                action();
                lock (queueLock) pending.Add(actionWrapper);
            };
            lock (queueLock) pending.Add(actionWrapper);
        }
        
        /// <summary>
        /// Release the dispatcher
        /// </summary>
        #if NATCAM_CORE
        [CoreDoc(82)]
        #endif
        public virtual void Release () {
            lock (queueLock) {
                if (pending == null) return;
                pending.Clear(); pending = null;
            }
            Debug.Log("NatCam Dispatch: Released dispatcher");
        }

        void IDisposable.Dispose () {
            SafeRelease();
        }

        protected virtual void SafeRelease () {
            Release(); // Release
        }
        #endregion


        #region --Callbacks--

        protected virtual void Update () {
            executing.Clear();
            lock (queueLock) {
                executing.AddRange(pending);
                pending.Clear();
            }
            executing.ForEach(e => e());
        }
        #endregion


        #region --Ctor--

        protected IDispatch () {
            pending = new Queue();
            executing = new Queue();
        }
        #endregion
    }
}