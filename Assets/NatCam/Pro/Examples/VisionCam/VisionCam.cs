/* 
*   NatCam Pro
*   Copyright (c) 2016 Yusuf Olokoba
*/

// Make sure to uncomment '#define OPENCV_API' in NatCam (Assets>NatCam>Pro>Plugins>Managed>NatCam.cs) and in OpenCVBehaviour
//#define OPENCV_API // Uncomment this to run this example properly

namespace NatCamU.Examples {

    using UnityEngine;
    using Core;
    using Pro;
    #if OPENCV_API
    using OpenCVForUnity;
    #endif

    public class VisionCam : OpenCVBehaviour {
        
        #if OPENCV_API

        public override void OnMatrix () {
            // Draw a diagonal line on our image
            Imgproc.line(matrix, new Point(0, 0), new Point(matrix.cols(), matrix.rows()), new Scalar(255, 0, 0, 255), 4);
            // Flush operations on the matrix
            FlushMatrix();
            // Display the result
            preview.texture = texture;
        }
        #endif
    }
}