# IRenderTargetBitmapRenderHost

`IRenderTargetBitmapRenderHost` defines a render host that can update an underlying `RenderTargetBitmap`.

## Methods

| Method | Description |
| --- | --- |
| `void Render()` | Requests that the render host renders its content to the bitmap. |

## Usage

This interface is implemented by behaviors like `RenderTargetBitmapBehavior` and `StaticRenderTargetBitmapBehavior` to allow external controls or actions to trigger a render update.
