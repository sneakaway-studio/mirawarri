/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;

namespace NatCamU.Core.UI {

    using UnityEngine.UI;
    using Utilities;

    [CoreDoc(199), RequireComponent(typeof(RawImage))]
    public sealed class NatCamPreview : MonoBehaviour {

        #region --Op vars--
        public RawImage image {get; private set;}
        private Material currMat, viewMat;
        #endregion


        #region --Unity Messaging--

        private void Awake () {
            image = GetComponent<RawImage>();
            currMat = image.material;
            viewMat = new Material(Shader.Find("Hidden/NatCam/Transform2D"));
            viewMat.SetFloat("_Zoom", 1f);
            image.material = viewMat;
        }

        private void OnDestroy () {
            if (image) image.material = currMat;
            Destroy(viewMat);
        }
        #endregion


        #region --Client API--

        /// <summary>
        /// Apply a texture with orientation and scale mode to the UI panel
        /// </summary>
        /// <param name="texture">Texture to be applied</param>
        /// <param name="orientation">Orientation to use for viewing the texture</param>
        /// <param name="scaleMode">Scale mode to properly display the texture</param>
        [CoreDoc(200)]
        public void Apply (Texture texture, Orientation orientation = Orientation.Rotation_0, ScaleMode scaleMode = ScaleMode.Fill) {
            if (!texture) return;
            image.texture = texture;
            // Orient
            image.materialForRendering.SetFloat("_Rotation", ((int)orientation & 7) * 0.5f);
            image.materialForRendering.SetFloat("_Mirror", (int)orientation >> 3);
            // Scale
            bool landscape = ((int)orientation & 7) % 2 == 0;
            var dimensions = new Vector2(landscape ? texture.width : texture.height, landscape ? texture.height : texture.width);
            Vector2
            screen = new Vector2(Screen.width, Screen.height) / image.canvas.scaleFactor,
            anchorMin = Vector2.Scale(image.rectTransform.anchorMin, screen),
            anchorMax = Vector2.Scale(image.rectTransform.anchorMax, screen),
            min = anchorMin + image.rectTransform.offsetMin,
            max = anchorMax + image.rectTransform.offsetMax,
            size = max - min;
            float aspect = dimensions.x / dimensions.y, viewAspect = size.x / size.y, width = 0f, height = 0f;
            switch (scaleMode) {
                case ScaleMode.ScaleWidth: height = size.y; width = height * aspect; break;
                case ScaleMode.ScaleHeight: width = size.x; height = width / aspect; break;
                case ScaleMode.Letterbox: if (aspect < viewAspect) goto case ScaleMode.ScaleWidth; else goto case ScaleMode.ScaleHeight;
				case ScaleMode.Fill: if (aspect > viewAspect) goto case ScaleMode.ScaleWidth; else goto case ScaleMode.ScaleHeight;
                case ScaleMode.None: width = size.x; height = size.y; break;
            }
            var position = image.rectTransform.position;
            image.rectTransform.offsetMin = Vector2.zero - anchorMin;
            image.rectTransform.offsetMax = new Vector2(width, height) - anchorMax;
            image.rectTransform.position = position;
        }
        #endregion
    }

    [CoreDoc(179)] public enum ScaleMode : byte {
		[CoreDoc(184)] None = 0,
		[CoreDoc(181)] ScaleWidth = 1,
		[CoreDoc(180)] ScaleHeight = 2,
		[CoreDoc(194)] Letterbox = 3,
		[CoreDoc(182)] Fill
	}
}