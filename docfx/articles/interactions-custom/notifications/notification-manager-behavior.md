# NotificationManagerBehavior

This behavior attaches to a `Control` (typically a `TopLevel` like `Window` or `UserControl` inside a window) and initializes a `WindowNotificationManager`. It exposes the manager via the `NotificationManager` property, which can be bound to a ViewModel.

### Properties

- `NotificationManager`: The initialized `INotificationManager` instance. (Read-only binding source).

### Usage Example

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <NotificationManagerBehavior NotificationManager="{Binding NotificationManager, Mode=OneWayToSource}" />
    </Interaction.Behaviors>
</Window>
```
