# RoutedEventTriggerBaseOfT

`RoutedEventTriggerBase<T>` is an abstract base class for triggers that listen for a specific typed `RoutedEvent<T>` on the associated object.

### Usage

Inherit from this class and implement the `RoutedEvent` property.

```csharp
public class MyTypedEventTrigger : RoutedEventTriggerBase<KeyEventArgs>
{
    protected override RoutedEvent<KeyEventArgs> RoutedEvent => InputElement.KeyDownEvent;
}
```
