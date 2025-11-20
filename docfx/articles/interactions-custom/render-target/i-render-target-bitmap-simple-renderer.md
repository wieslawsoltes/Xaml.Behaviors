# IRenderTargetBitmapSimpleRenderer

`IRenderTargetBitmapSimpleRenderer` provides a method used by `StaticRenderTargetBitmapBehavior` to draw onto a render target.

## Methods

| Method | Description |
| --- | --- |
| `void Render(DrawingContext context)` | Draws into the provided `DrawingContext`. |

## Usage

Implement this interface to provide custom drawing logic for `StaticRenderTargetBitmapBehavior`.

```csharp
public class MySimpleRenderer : IRenderTargetBitmapSimpleRenderer
{
    public void Render(DrawingContext context)
    {
        // Drawing logic here
        context.DrawEllipse(Brushes.Blue, null, new Point(50, 50), 25, 25);
    }
}
```
