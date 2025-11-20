# ItemsControlContainerEventsBehavior

`ItemsControlContainerEventsBehavior` is an abstract base class that exposes container-related events from an `ItemsControl`. It simplifies the process of creating behaviors that need to respond to container lifecycle events.

## Events Handled

The behavior subscribes to the following events on the associated `ItemsControl`:

*   `PreparingContainer`
*   `ContainerPrepared`
*   `ContainerIndexChanged`
*   `ContainerClearing`

## Usage

This is a base class and cannot be used directly in XAML. Instead, create a class that inherits from `ItemsControlContainerEventsBehavior` and override the virtual methods to implement your custom logic.

```csharp
public class MyCustomContainerBehavior : ItemsControlContainerEventsBehavior
{
    protected override void OnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        // Custom logic when a container is prepared
        var container = e.Container;
        // ...
    }
}
```
