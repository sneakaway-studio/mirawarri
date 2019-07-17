/* 
*   Compositor
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace CompositorU {

    using UnityEngine;
    using System;

    #region --Callbacks--

    public delegate void CompositeCallback (Texture2D composite);
    #endregion
    

    #region --Value types--

    [Serializable]
    public struct Point {
        public int x, y;
		public static readonly Point zero = Vector2.zero;
        public Point (int x, int y) {
            this.x = x;
            this.y = y;
        }

		public static explicit operator Vector3 (Point p) {
			return new Vector2(p.x, p.y);
		}
		public static implicit operator Point (Vector2 p) {
			return new Point((int)p.x, (int)p.y);
		}
    }

	[Serializable]
	public struct Layer {
		public Texture2D texture;
        public Point offset;
		public Vector2 scale;
		public float rotation;
		public CompositeCallback callback;
		public static readonly CompositeCallback Release = tex => Texture2D.Destroy(tex);

		/// <summary>
		/// Create a new composition layer
		/// </summary>
		/// <param name="texture">Layer texture</param>
		/// <param name="offset">Offset of the layer's bottom left corner before any rotation is applied</param>
		/// <param name="rotation">Layer's rotation in degrees</param>
		/// <param name="scale">Layer's scale. To use natural scale, use (1, 1)<param>
		/// <param name="callback">Callback to be invoked once the layer has been composited. Use this for resource or memory management</summary>
		public Layer (Texture2D texture, Point offset, float rotation, Vector2 scale, CompositeCallback callback = null) {
			this.texture = texture;
			this.offset = offset;
			this.rotation = rotation;
			this.scale = scale;
			this.callback = callback;
		}
	}
	#endregion
}