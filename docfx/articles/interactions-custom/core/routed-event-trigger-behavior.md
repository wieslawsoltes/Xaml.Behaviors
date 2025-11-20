# RoutedEventTriggerBehavior

The `RoutedEventTriggerBehavior` listens for a specified `RoutedEvent` on a source object (or the associated object if no source is specified) and executes its actions when that event is raised.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `RoutedEvent` | `RoutedEvent` | The routed event to listen for. |
| `RoutingStrategies` | `RoutingStrategies` | The routing strategies to listen for (e.g., Bubble, Tunnel, Direct). Default is `Direct | Bubble`. |
| `SourceInteractive` | `Interactive` | The source object to listen to. If not set, the associated object is used. |

### Example

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <RoutedEventTriggerBehavior RoutedEvent="{x:Static Button.ClickEvent}">
            <InvokeCommandAction Command="{Binding OnClickCommand}" />
        </RoutedEventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
