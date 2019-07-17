/* 
*   NatCam Pro
*   Copyright (c) 2017 Yusuf Olokoba
*/

// Make sure to uncomment '#define OPENCV_API' below and in NatCam.cs
//#define OPENCV_API

namespace NatCamU.Pro {

    using UnityEngine;
    using Core;
    using Core.Utilities;
    #if OPENCV_API
    using OpenCVForUnity;
    #endif

    [ProDoc(217)]
    public abstract class OpenCVBehaviour : NatCamBehaviour {
        
        #if OPENCV_API

        #region --Op vars--

        /// <summary>
        /// Get the matrix texture
        /// </summary>
        [ProDoc(218)]
        public Texture2D texture {get; private set;}
        /// <summary>
        /// Get the preview matrix
        /// </summary>
        [ProDoc(219)]
        public Mat matrix {get { return _matrix;}}
        private Mat _matrix;
        private Color32[] colors; // Should we expose this?
        #endregion


        #region --Operations--

        public override void OnStart () {} // Don't do anything in OnStart

        public override void OnFrame () {
            // Get the preview matrix and call OnMatrix
            if (NatCam.PreviewMatrix(ref _matrix)) OnMatrix();
        }

        /// <summary>
		/// Method called on every frame that the camera preview matrix updates
        /// This method should be overridden by subclasses for processing the matrix
		/// </summary>
        [ProDoc(220), ProCode(33)]
        public abstract void OnMatrix ();

        protected override void OnDisable () {
            // Release mat
            if (_matrix != null) _matrix.Dispose (); _matrix = null;
            if (texture) Texture2D.Destroy(texture); texture = null;
            colors = null;
            // Base
            base.OnDisable();
        }

        /// <summary>
        /// Flush all operations on the preview matrix and update the matrix texture with the results.
        /// For best performance, invoke this method after all operations on the preview matrix have been issued.
        /// This function can be overridden.
        /// </summary>
        [ProDoc(221), ProCode(33)]
        public virtual void FlushMatrix () {
            // Null checking
            if (_matrix == null) return;
            // Size checking
            if (texture && (texture.width != _matrix.cols() || texture.height != _matrix.rows())) {
                Texture2D.Destroy(texture);
                texture = null;
                colors = null;
            }
            // Create the texture
            texture = texture ?? new Texture2D(_matrix.cols(), _matrix.rows(), TextureFormat.ARGB32, false, false);
            // Create pixel buffer
            colors = colors ?? new Color32[_matrix.cols() * _matrix.rows()];
            // Update the texture
            Utils.matToTexture2D(_matrix, texture, colors);
        }
        #endregion
        #endif
    }
}