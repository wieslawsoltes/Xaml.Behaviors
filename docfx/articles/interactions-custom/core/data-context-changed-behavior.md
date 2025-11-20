# DataContextChangedBehavior

`DataContextChangedBehavior<T>` is an abstract base class for behaviors that need to react when the `DataContextChanged` event is raised on the associated `StyledElement`.

### Usage

Inherit from this class and implement the `OnDataContextChangedEventOverride` method.

```csharp
public class MyDataContextBehavior : DataContextChangedBehavior<Control>
{
    protected override IDisposable OnDataContextChangedEventOverride()
    {
        // Handle DataContext changed
        return Disposable.Empty;
    }
}
```
