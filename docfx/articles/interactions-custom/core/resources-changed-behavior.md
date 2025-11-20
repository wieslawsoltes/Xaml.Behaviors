# ResourcesChangedBehavior

`ResourcesChangedBehavior<T>` is an abstract base class for behaviors that need to react when the `ResourcesChanged` event is raised on the associated `StyledElement`.

### Usage

Inherit from this class and implement the `OnResourcesChangedEventOverride` method.

```csharp
public class MyResourcesBehavior : ResourcesChangedBehavior<Control>
{
    protected override IDisposable OnResourcesChangedEventOverride()
    {
        // Handle resources changed
        return Disposable.Empty;
    }
}
```
