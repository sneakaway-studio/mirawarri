# Compositor API
Compositor is a lightweight utility API for compositing images quickly and efficiently in Unity.
Download the unitypackage [here](https://www.dropbox.com/s/qa8fhvkyo3gmrot/Compositor1.0b1.unitypackage?dl=1).

## Compositor
Compositor treats images as layers. First, you create a compositor:
```csharp
// Create a compositor with a size
var compositor = new RenderCompositor(width, height);
```
Then you add layers:
```csharp
// Add a layer with no offset, no rotation, and default size (1, 1)
compositor.AddLayer(new Layer(texture1, Vector2.zero, 0f, Vector2.one));
// Add a layer with an offset
compositor.AddLayer(new Layer(texture1, new Point(40, 70), 0f, Vector2.one));
// Add a layer, but free the layer texture immediately it has been composited
compositor.AddLayer(new Layer(texture1, Vector2.zero, 0f, Vector2.one, Layer.Release));
// Add a layer, and execute a callback once the layer has been composited // This is useful for texture resource management
compositor.AddLayer(new Layer(texture1, Vector2.zero, 0f, Vector2.one, layerTexture => OnCompositeLayer(layerTexture)));
```
Then composite, providing a callback to be invoked with the resulting texture:
```csharp
compositor.Composite(OnComposite);

void OnComposite (Texture2D result) {
    // Display it on a material
    material.mainTexture = result;
}
```
Finally, release the compositor's resources:
```csharp
compositor.Dispose();
```
Compositors implement the IDisposable interface, so you can use it in a `using` block:
```csharp
using (var compositor = new RenderCompositor(width, height)) {
    // Add layers...
    compositor.Composite(OnComposite);
} // The compositor is automatically freed at the end of this block
```

## RenderCompositor
*INCOMPLETE*

## PixelCompositor
*INCOMPLETE*

## Credits
- [Yusuf Olokoba](mailto:olokobayusuf@gmail.com)
- [Owen Mundy](omundy@gmail.com)