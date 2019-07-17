/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    using System;

    public interface ICompositor : IDisposable {

        #region --Properties--
        int width {get;}
        int height {get;}
        #endregion

        #region --Client API--

        /// <summary>
		/// Add a layer to be composited
		/// </summary>
		/// <param name="layer">Layer to be composited</param>
        void AddLayer (Layer layer);
        
        /// <summary>
		/// Add a layer to be composited
		/// </summary>
		/// <param name="layer">Layer to be composited</param>
        void Composite (CompositeCallback callback);
        #endregion
    }
}