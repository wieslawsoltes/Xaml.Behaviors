# AttachedToVisualTreeTriggerBase

`AttachedToVisualTreeTriggerBase<T>` is an abstract base class for triggers that need to perform actions or subscriptions when the associated object is attached to the visual tree.

## Methods

| Method | Description |
| --- | --- |
| `IDisposable OnAttachedToVisualTreeOverride()` | Called after the associated object is attached to the visual tree. Returns a disposable that will be disposed when the trigger is detached. |

## Usage

Inherit from this class to create a custom trigger that activates when attached to the visual tree.

```csharp
public class MyVisualTreeTrigger : AttachedToVisualTreeTriggerBase<Button>
{
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        // Perform setup or subscription
        return Disposable.Create(() => 
        {
            // Perform cleanup
        });
    }
}
```
