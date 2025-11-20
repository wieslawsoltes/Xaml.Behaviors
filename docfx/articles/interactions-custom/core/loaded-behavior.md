# LoadedBehavior

`LoadedBehavior<T>` is an abstract base class for behaviors that need to react when the `Loaded` event is raised on the associated `Control`.

### Usage

Inherit from this class and implement the `OnLoadedOverride` method.

```csharp
public class MyLoadedBehavior : LoadedBehavior<Control>
{
    protected override IDisposable OnLoadedOverride()
    {
        // Handle loaded event
        return Disposable.Empty;
    }
}
```
