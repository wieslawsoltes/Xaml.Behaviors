# AttachedToLogicalTreeTriggerBase

`AttachedToLogicalTreeTriggerBase<T>` is an abstract base class for triggers that need to perform actions or subscriptions when the associated object is attached to the logical tree.

## Methods

| Method | Description |
| --- | --- |
| `IDisposable OnAttachedToLogicalTreeOverride()` | Called after the associated object is attached to the logical tree. Returns a disposable that will be disposed when the trigger is detached. |

## Usage

Inherit from this class to create a custom trigger that activates when attached to the logical tree.

```csharp
public class MyLogicalTreeTrigger : AttachedToLogicalTreeTriggerBase<Button>
{
    protected override IDisposable OnAttachedToLogicalTreeOverride()
    {
        // Perform setup or subscription
        return Disposable.Create(() => 
        {
            // Perform cleanup
        });
    }
}
```
