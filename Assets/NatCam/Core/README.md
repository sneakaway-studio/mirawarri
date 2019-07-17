# NatCam Core
NatCam Core provides a clean, functional, and amazingly performant API for accessing and controlling device cameras. NatCam is built on platform-specific implementations of the NatCam Specification (`INatCam` and `INatCamDevice` interfaces).

Using NatCam is as simple as calling:
```csharp
NatCam.Play(DeviceCamera.RearCamera);
```

The preview is started and the preview texture becomes available in the `NatCam.OnStart` event. This event is usually used to display the preview texture on a surface:
```csharp
NatCam.OnStart += () => material.mainTexture = NatCam.Preview;
```

NatCam features a full camera control pipeline for utilizing camera functionality such as focusing, zooming, exposure, and so on. To use this functionality, simply access the properties in the DeviceCamera class:
```csharp
DeviceCamera.RearCamera.ExposureBias = 1.3;
```

Cameras can be set as active using the `NatCam.Camera` property. When a camera is active, calls to `NatCam.Play` would cause the preview to start from that camera. When NatCam is playing, the active camera can be switched by setting `NatCam.Camera` to a different camera. This will automatically start the preview from the newly set camera (so there is no need to call `NatCam.Play`).
```csharp
// Switch cameras while the preview is playing
NatCam.Camera = DeviceCamera.FrontCamera;
```

NatCam also allows for high-resolution photo capture from the camera. To do so, simply call the `CapturePhoto` function with an appropriate callback and an orientation container (since the photo is not corrected for app orientation):
```csharp
NatCam.CapturePhoto(OnPhoto);

void OnPhoto (Texture2D photo, Orientation orientation) {
    // Do stuff...
    Texture2D.Destroy(photo); // Remember to release the texture so as to avoid memory leak
}
```

When taking photos on iOS and Android, you might notice that the captured photo is rotated. This is because mobile cameras always return photos in their 'natural' orientation, landscape left. As a result, you must correct for this rotation if you want to display it or use if for other purposes.

If you wish to display the captured photo, you can use the `NatCamPreview` component in the `NatCamU.Core.UI` namespace to correct for the rotation. Simply apply the component on a RawImage and call `Apply` with the captured photo and returned orientation:
```csharp
public NatCamPreview previewPanel; // Reference this in the Editor

void OnPhoto (Texture2D photo, Orientation orientation) {
    // Display the photo with the correct orientation
    previewPanel.Apply(photo, orientation);
}
```

I should note that `NatCamPreview` does not actually rotate the image to be upright; *it only displays it upright*. If you would like to physically rotate the image, then you can use the `Utilities.RotateImage` API's in the `NatCamU.Core.Utilities` namespace:
```csharp
void OnPhoto (Texture2D photo, Orientation orientation) {
    // Physically rotate the image
    Utilities.RotateImage(texture, orientation, null, null, (Texture2D rotated, Orientation orientation) => {
        // Display the rotated photo
        preview.texture = rotated;
    });
}
```

___

With the simplicity of NatCam Core, you have the power and speed to create interactive, responsive camera apps. Happy coding!

## Requirements
- On iOS, NatCam Core requires iOS 7 and up (it requires iOS 8 if you use `DeviceCamera.ExposureBias`).
- On Android, NatCam Core requires API level 18 and up.

## Tutorials
1. [Starting Off](https://medium.com/@olokobayusuf/natcam-tutorial-series-1-starting-off-dc3990f5dab6)
2. [Controls](https://medium.com/@olokobayusuf/natcam-tutorial-series-2-controls-d2e2d0738223)
3. [Photos](https://medium.com/@olokobayusuf/natcam-tutorial-series-3-photos-e28361b83cf8)
4. [Goodies](https://medium.com/@olokobayusuf/natcam-tutorial-series-x-goodies-3f4dcfac555b)
5. [MoodCam VR]()

## Notes
- On Android, Unity automatically requests camera permissions on app start. This cannot be changed without modifying Unity Android natively.
- On iOS, camera permissions are requested the first time the camera is opened.

## Quick Tips
- Please peruse the included scripting reference under NatCam>Scripting Reference in the Editor. You can also find the docs [here](http://docs.natcam.io).
- To discuss or report an issue, visit Unity forums [here](http://forum.unity3d.com/threads/natcam-device-camera-api.374690/).
- Check out more NatCam examples on Github [here](https://github.com/olokobayusuf?tab=repositories).
- Contact me at [olokobayusuf@gmail.com](mailto:olokobayusuf@gmail.com).
- See video tutorials [here](https://www.youtube.com/watch?v=6thfRz9vkyM&list=PL993yBWYjPgCiIkUlM3DJhOdcXNm9IVXh).

Thank you very much!