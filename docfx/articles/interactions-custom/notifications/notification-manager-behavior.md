# NotificationManagerBehavior

Provides a `INotificationManager` for the associated `Control`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| NotificationManager | `INotificationManager` | Gets the `INotificationManager` instance. |

## Usage

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="BehaviorsTestApplication.Views.MainWindow"
        x:Name="Window">
    <Interaction.Behaviors>
        <NotificationManagerBehavior NotificationManager="{Binding #Window.NotificationManager, Mode=OneWayToSource}" />
    </Interaction.Behaviors>
    <!-- ... -->
</Window>
```
