/* 
*   NatCam
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCam.Internal {

    using AOT;
    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;

    public static class RenderDispatcher {

        public static void Dispatch (Action workload) {
            var delegateHandle = Marshal.GetFunctionPointerForDelegate((UnityRenderingEvent)DequeueRender); 
            GL.IssuePluginEvent(delegateHandle, ((IntPtr)GCHandle.Alloc(workload, GCHandleType.Normal)).ToInt32());
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnityRenderingEvent (int context);

        [MonoPInvokeCallback(typeof(UnityRenderingEvent))]
        private static void DequeueRender (int context) {
            GCHandle handle = (GCHandle)(IntPtr)context;
            Action target = handle.Target as Action;
            handle.Free();
            target();
        }
    }
}