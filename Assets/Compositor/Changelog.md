## Compositor 1.0b2
+ Added PixelCompositor (though it is incomplete).
+ Added ICompositor.Width and ICompositor.Height getters.
+ Reimplemented RenderCompositor to use GL immediate commands.
+ Fixed bugs when rotating a layer being composited.

## Compositor 1.0b1
+ ICompositor interface that powers image compositing.
+ RenderCompositor class for GPU-accelerated compositing.
+ Support for offsetting layers.
+ Support for rotating layers.
+ Support for scaling layers on each axis.
+ CompositeCallback support for managing resources that are finished being used.