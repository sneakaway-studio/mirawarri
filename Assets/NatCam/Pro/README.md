# NatCam Pro
NatCam Pro builds upon NatCam Core by adding functionality that will be useful 
for more advanced applications:
- Preview data
- OpenCV preview matrix
- Preview frame
- Video recording
- Sources

## Preview Data
The camera stream data can be accessed using the PreviewBuffer API. The Preview Matrix and Preview Frame 
implementations are built upon this. To acces the data for the current frame, call `NatCam.PreviewBuffer(out IntPtr)` in 
the `NatCam.OnFrame` event. NatCam maintains a single handle to the preview buffer in unmanaged memory 
(on all platforms), so you must copy out the data if you intend to modify it. To do so, call `Marshal.Copy` or `memcpy`
depending on your development environment.
```csharp
public override void OnFrame () {
    // Declare buffer properties
    IntPtr buffer; int width, height, size;
    // Read the preview buffer
    NatCam.PreviewBuffer(out buffer, out width, out height, out size);
    // Do stuff with the native buffer
    ...
}
```

In C++ for instance:
```cpp
void* myCopy = new unsigned char[size];
memcpy(myCopy, ptr, size);
// Do stuff with your copy
```

## Preview Matrix
NatCam Professional comes with a lightweight OpenCVForUnity support wrapper. To use it, you must first uncomment 
this line at the top of NatCam.cs in NatCam Professional:
```csharp
#define OPENCV_API //Uncomment this to have access to the PreviewMatrix OpenCV API
```
`NatCam.PreviewMatrix` initializes (if necessary) and fills an `OpenCVForUnity.Mat` with the preview data for the current 
frame. You can then use the matrix for all your OpenCV processing.
```csharp
Mat matrix;
...

public override void OnPreviewUpdate () {
    // Get the preview matrix for this frame
    if (!NatCam.PreviewMatrix(ref matrix)) return;
    // Draw a diagonal line on our image
    Imgproc.line(matrix, new Point(0, 0), new Point(matrix.cols(), matrix.rows()), new Scalar(255, 0, 0, 255), 4);
    // Update our destination texture with the line drawn above
    Utils.matToTexture2D(matrix, texture, colors);
    // Display the result
    preview.texture = texture;
}
```

## Preview Frame
`NatCam.PreviewFrame` gives you access to the current preview frame from the camera. You can use it to take _screenshots_.
```csharp
Texture2D frame;
...

for (int i = 0; i < 10; i++) {
    // Take a screenshot
    NatCam.PreviewFrame(ref frame);
    // Show it
    preview.texture = frame;
    // Wait for half a second
    yield return new WaitForSeconds(0.5f);
}
```

## Video Recording
NatCam supports video recording directly from the camera stream. Videos are recorded with the H.264 video codec (MPEG4-AVC). Once recording is finished, the path to the video is provided to the callback that was supplied when `NatCam.StartRecording` was called.
```csharp
// Start recording
NatCam.StartRecording(OnVideo);
// Do other stuff or wait a while
...
// Stop recording
NatCam.StopRecording();

void OnVideo (string path) {
    // Log
    Debug.Log("Recorded video to path: "+path);
}
```

Recording can only be started **when the camera preview is running**. This means that calling `StartRecording` immediately after calling `Play` is a invalid operation. If you would like to start recording immediately the camera preview starts, call `StartRecording` in the `OnStart` event.

When recording, some events are specially handled. Cameras cannot be changed when recording. When the app is suspended by the user, recording is immediately stopped and the callback is not guaranteed to be called then. If the device runs out of memory, then the respective native recording API will be left to handle it as NatCam does not monitor memory usage.

## Sources
If you need to make changes to the API to better suit your application, please send me an email and I'll share the native sources with you. Please note that the sources for NatCam dependencies like NatCamRenderDispatch and NatCamFastRead **will not be shared** as they are closed source. But all other sources will be shared. I should also note that **we do not provide support for custom builds of NatCam**, so I will not be able to provide any support if you make changes to NatCam.

## Tutorials
1. [Videos](https://medium.com/@olokobayusuf/natcam-tutorial-series-4-videos-c533f3ffb1e2)
2. [Preview Data](https://medium.com/@olokobayusuf/natcam-tutorial-series-5-preview-data-9ac36eafd1f0)
3. [Preview Data: BlurCam](https://medium.com/@olokobayusuf/natcam-tutorial-series-6-blurcam-app-e6c2e7b60db2)
4. [OpenCV]()

## Notes
- On Android, NatCam Pro requires API Level 18 and up.
