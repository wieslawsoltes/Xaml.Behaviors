# StaticRenderTargetBitmapBehavior

`StaticRenderTargetBitmapBehavior` is a behavior that draws once into a `RenderTargetBitmap` and assigns it to the associated `Image`. Rendering can be manually triggered by calling `Render()`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PixelWidth | `int` | Gets or sets the pixel width of the bitmap. Default is 200. |
| PixelHeight | `int` | Gets or sets the pixel height of the bitmap. Default is 200. |
| Dpi | `Vector` | Gets or sets the DPI vector of the bitmap. Default is (96, 96). |
| Renderer | `IRenderTargetBitmapSimpleRenderer` | Gets or sets the renderer used to draw the bitmap. |

## Usage

```xml
<Image>
    <Interaction.Behaviors>
        <StaticRenderTargetBitmapBehavior PixelWidth="300" PixelHeight="300">
            <StaticRenderTargetBitmapBehavior.Renderer>
                <local:MySimpleRenderer />
            </StaticRenderTargetBitmapBehavior.Renderer>
        </StaticRenderTargetBitmapBehavior>
    </Interaction.Behaviors>
</Image>
```
