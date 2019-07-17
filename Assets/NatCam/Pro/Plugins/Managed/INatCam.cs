/* 
*   NatCam Pro
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System;
    using Pro;

    public partial interface INatCam {

        #region --Properties--
        bool SupportsRecording {get;}
        bool IsRecording {get;}
        #endregion

        #region --Client API--
        void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size);
        void StartRecording (Configuration configuration, SaveCallback callback);
        void StopRecording ();
        #endregion
    }
}