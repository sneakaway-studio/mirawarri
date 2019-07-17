/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    using UnityEngine;
    using System;
    using Layers = System.Collections.Generic.List<Layer>;

    public sealed class PixelCompositor : ICompositor { // INCOMPLETE

        #region --Properties--
        public int width {get; private set;}
		public int height {get; private set;}
        #endregion


        #region --Op vars--
        private Color32[] composite;
        private Layers layers;
        private readonly bool alpha, immediate;
        #endregion


        #region --Client API--

        /// <summary>
		/// Create a pixel compositor
		/// </summary>
		/// <param name="width">Composite width</param>
		/// <param name="height">Composite height</param>
        /// <param name="transparency">Should transparency be supported? Leaving false can greatly improve performance</param>
        /// <param name="immediate">Should layers be composited immediately they are added? Leaving true could improve memory management</param>
        public PixelCompositor (int width, int height, bool transparency = true, bool immediate = true) {
            // Create the layers collection
			layers = new Layers();
            // Create the composite up front
            composite = new Color32[(this.width = width) * (this.height = height)];
            // Set options
            this.alpha = transparency;
            this.immediate = immediate;
        }

        /// <summary>
		/// Add a layer to be composited
		/// </summary>
		/// <param name="layer">Layer to be composited</param>
        public void AddLayer (Layer layer) {
            // Composite
			if (immediate) Composite(layer);
            // Add the layer
            else layers.Add(layer);
        }

        /// <summary>
		/// Composite layers
		/// </summary>
		/// <param name="callback">Callback to be invoked with the composite texture</param>
        public void Composite (CompositeCallback callback) {
            // Null checking
			if (callback == null) {
				Debug.LogError("Compositor: Callback must not be null");
				return;
			}
            if (composite == null) {
				Debug.LogError("Compositor: No layers to composite. You might have called Dispose() too early");
				return;
			}
            // If immediate, composite
            foreach (var layer in layers) Composite(layer);
            // Create a result texture
			var result = new Texture2D(width, height);
            // Load the composite data into the texture
			result.SetPixels32(composite);
			result.Apply();
            // Invoke callback
            callback(result);
        }
        #endregion


        #region --Operations--

        private void Composite (Layer layer) {
            // Translation, rotation, scale, alpha blending
        }
        #endregion


        #region --IDisposable--

        public void Dispose () {
            // Clear layers
            layers.Clear();
            // Force garbage collection
            composite = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }
        #endregion


        #region --Utility--

        #endregion
    }
}