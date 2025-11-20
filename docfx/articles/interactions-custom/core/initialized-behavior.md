# InitializedBehavior

`InitializedBehavior<T>` is an abstract base class for behaviors that need to react when the `Initialized` event is raised on the associated `StyledElement`.

### Usage

Inherit from this class and implement the `OnInitializedEventOverride` method.

```csharp
public class MyInitializedBehavior : InitializedBehavior<Control>
{
    protected override IDisposable OnInitializedEventOverride()
    {
        // Handle initialized event
        return Disposable.Empty;
    }
}
```
