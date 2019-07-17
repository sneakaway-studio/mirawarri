/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

	using UnityEngine;
	using System;
	using JobQueue = System.Collections.Generic.List<System.Action>;
	using Layers = System.Collections.Generic.List<Layer>;

	public sealed class RenderCompositor : ICompositor {

		#region --Properties--
		public int width {
			get {
				return composite.width;
			}
		}
		public int height {
			get {
				return composite.height;
			}
		}
		#endregion


		#region --Op vars--
		private RenderTexture composite;
		private Material material;
		private GraphicsQueue commandQueue;
		private Layers layers;
		#endregion


		#region --Client API--

		/// <summary>
		/// Create a GPU-accelerated compositor
		/// </summary>
		/// <param name="width">Composite width</param>
		/// <param name="height">Composite height</param>
		public RenderCompositor (int width, int height) {
			// Create the material
			material = new Material (Shader.Find ("Hidden/RenderCompositor"));
			// Create the graphic command queue
			commandQueue = new GraphicsQueue();
			// Create the layers collection
			layers = new Layers();
			// Create the composite // Antialiasing please ;)
			composite = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 8);
			// Clear it
			commandQueue.Enqueue(() => {
				Graphics.SetRenderTarget(composite);
				GL.Clear(true, true, Color.black);
			});
		}

		/// <summary>
		/// Add a layer to be composited
		/// </summary>
		/// <param name="layer">Layer to be composited</param>
		public void AddLayer (Layer layer) {
			// Add the layer
			layers.Add(layer);
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
			// Count checking
			if (layers.Count == 0) {
				Debug.LogError("Compositor: No layers provided");
				return;
			}
			// Composite all layers
			foreach (var layer in layers) Composite(layer);
			// Readback
			Readback(callback);
		}
		#endregion


		#region --Operations--

        private void Composite (Layer layer) {
			commandQueue.Enqueue(() => {
				// Set the render target
				Graphics.SetRenderTarget(composite);
				// Load the ortho projection
				GL.PushMatrix();
				GL.LoadPixelMatrix(0, composite.width, 0, composite.height);
				// Set the material texture
				material.SetTexture("_MainTex", layer.texture);
				// Set the material pass
				if (!material.SetPass(0)) {
					Debug.LogError("Compositor: Failed to activate compositor material to composite layer");
					return;
				}
				// Apply the transformations // Note that the multiplication is done in reverse order
				var extent = new Vector2(layer.texture.width, layer.texture.height) * 0.5f;
				GL.MultMatrix(
					Matrix4x4.Translate((Vector3)layer.offset) *
					Matrix4x4.Translate(extent) *
					Matrix4x4.Scale(layer.scale) *
					Matrix4x4.TRS(Vector2.zero, Quaternion.AngleAxis(layer.rotation, Vector3.forward), Vector2.one) *
					Matrix4x4.Translate(-extent)
				);
				// Draw the quad
				GL.Begin(GL.QUADS);
				GL.TexCoord2(0f, 0f);
				GL.Vertex3(0f, 0f, 0);
				GL.TexCoord2(0f, 1f);
				GL.Vertex3(0f, layer.texture.height, 0);
				GL.TexCoord2(1f, 1f);
				GL.Vertex3(layer.texture.width, layer.texture.height, 0);
				GL.TexCoord2(1f, 0f);
				GL.Vertex3(layer.texture.width, 0f, 0);
				GL.End();
				// Restore camera projection
				GL.PopMatrix ();
            });
        }

        private void Readback (CompositeCallback callback) {
			commandQueue.Enqueue(() => {
				// Create a result texture
				var result = new Texture2D(composite.width, composite.height);
				RenderTexture current = RenderTexture.active;
				RenderTexture.active = composite;
				// Read back
				result.ReadPixels(new Rect(0, 0, composite.width, composite.height), 0, 0);
				result.Apply();
				RenderTexture.active = current;
				// Invoke callback
				callback(result);
			});
		}
        #endregion


        #region --IDisposable--

		public void Dispose () {
			// Enqueue to guarantee that any previous calls to Composite and Readback are completed
			commandQueue.Enqueue(() => {
				// Free the composite texture
				RenderTexture.ReleaseTemporary(composite);
				// Free the material
				Material.Destroy(material);
				// Dispose the command queue
				commandQueue.Dispose();
				// Clear the layers
				layers.Clear();
			});
		}
		#endregion


        #region --Utility--
		
		/// <summary>
		/// This is a helper class for invoking Graphics jobs at the right time.
		/// On some platforms (like Metal), calls to the Graphics API will fail unless performed
		/// in the onPreRender/onPostRender event.
		/// </summary>
		private sealed class GraphicsQueue : IDisposable {

			private JobQueue jobs;

			public GraphicsQueue () {
				// Create the queue
				jobs = new JobQueue();
				// Register for the post render event
				Camera.onPostRender += Update;
			}

			public void Enqueue (Action job) {
				jobs.Add(job);
			}

			public void Dispose () {
				// Unregister from the post render event
				Camera.onPostRender -= Update;
			}

			private void Update (Camera unused) {
				// Invoke all jobs
				jobs.ForEach(job => job());
				// Clear the queue
				jobs.Clear();
			}
		}
		#endregion
    }
}