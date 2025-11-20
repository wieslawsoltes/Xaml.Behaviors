# WriteableBitmapRenderBehavior

Creates a `WriteableBitmap` and updates it using a renderer on a timer.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PixelWidth | `int` | Gets or sets the width of the bitmap in pixels. Default is 256. |
| PixelHeight | `int` | Gets or sets the height of the bitmap in pixels. Default is 256. |
| Renderer | `IWriteableBitmapRenderer` | Gets or sets the renderer used to update the bitmap. |
| Bitmap | `WriteableBitmap` | Gets the created bitmap. |

## Usage

```xml
<Image>
    <Interaction.Behaviors>
        <WriteableBitmapRenderBehavior PixelWidth="512" PixelHeight="512" Renderer="{Binding MyRenderer}" />
    </Interaction.Behaviors>
</Image>
```
