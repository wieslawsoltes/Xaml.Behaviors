# IRenderTargetBitmapRenderer

`IRenderTargetBitmapRenderer` provides a method used by `RenderTargetBitmapBehavior` to draw onto a render target.

## Methods

| Method | Description |
| --- | --- |
| `void Render(DrawingContext context, TimeSpan elapsed)` | Draws into the provided `DrawingContext`. `elapsed` is the time since the behavior was attached. |

## Usage

Implement this interface to provide custom drawing logic for `RenderTargetBitmapBehavior`.

```csharp
public class MyRenderer : IRenderTargetBitmapRenderer
{
    public void Render(DrawingContext context, TimeSpan elapsed)
    {
        // Drawing logic here
        context.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 100, 100));
    }
}
```
