/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

using UnityEngine;
using UnityEngine.UI;

namespace CompositorU.Examples {

	public class CompositeLayers : MonoBehaviour {

		public RawImage rawImage;
		public Layer[] layers;

		// Use this for initialization
		void Start () {
			// Create a compositor
			using (var compositor = new RenderCompositor(layers[0].texture.width, layers[0].texture.height)) {
				// Add layers
				foreach (var layer in layers) compositor.AddLayer (layer);
				// Composite and display the result
				compositor.Composite (result => rawImage.texture = result);
			}
		}
	}
}