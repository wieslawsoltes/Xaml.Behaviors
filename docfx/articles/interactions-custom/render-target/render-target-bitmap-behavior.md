# RenderTargetBitmapBehavior

`RenderTargetBitmapBehavior` is a behavior that draws continuously into a `RenderTargetBitmap` and assigns it to the associated `Image`. It uses a timer to trigger rendering.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PixelWidth | `int` | Gets or sets the pixel width of the bitmap. Default is 200. |
| PixelHeight | `int` | Gets or sets the pixel height of the bitmap. Default is 200. |
| Dpi | `Vector` | Gets or sets the DPI vector of the bitmap. Default is (96, 96). |
| Renderer | `IRenderTargetBitmapRenderer` | Gets or sets the renderer used to draw the bitmap. |

## Usage

```xml
<Image>
    <Interaction.Behaviors>
        <RenderTargetBitmapBehavior PixelWidth="300" PixelHeight="300">
            <RenderTargetBitmapBehavior.Renderer>
                <local:MyRenderer />
            </RenderTargetBitmapBehavior.Renderer>
        </RenderTargetBitmapBehavior>
    </Interaction.Behaviors>
</Image>
```
