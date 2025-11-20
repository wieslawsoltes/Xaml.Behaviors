# AttachedToVisualTreeBehavior

`AttachedToVisualTreeBehavior<T>` is an abstract base class for behaviors that need to perform actions when the associated object is attached to the visual tree.

### Usage

Inherit from this class and implement the `OnAttachedToVisualTreeOverride` method.

```csharp
public class MyVisualTreeBehavior : AttachedToVisualTreeBehavior<Control>
{
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        // Handle attachment to visual tree
        return Disposable.Empty;
    }
}
```
