# AttachedToLogicalTreeBehavior

`AttachedToLogicalTreeBehavior<T>` is an abstract base class for behaviors that need to perform actions when the associated object is attached to the logical tree.

### Usage

Inherit from this class and implement the `OnAttachedToLogicalTreeOverride` method.

```csharp
public class MyLogicalTreeBehavior : AttachedToLogicalTreeBehavior<Control>
{
    protected override IDisposable OnAttachedToLogicalTreeOverride()
    {
        // Handle attachment to logical tree
        return Disposable.Empty;
    }
}
```
