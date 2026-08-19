# RoutedEventTrigger

The `RoutedEventTrigger` (and its variants `RoutedEventTriggerBase`, `RoutedEventTriggerBehavior`) allows you to execute actions in response to any routed event.

### Properties
- `RoutedEvent`: The routed event to listen for (e.g., `Button.ClickEvent`).
- `RoutingStrategies`: The routing strategy to use (Tunnel, Bubble, Direct).

When a `RoutedEventTriggerBehavior` is attached directly to a `TopLevel`, its routed-event subscription and action bindings remain active through the top-level's closed event. They are released when the behavior is actually detached. Other controls continue to subscribe only while attached to the visual tree.

```xml
<Window>
  <Interaction.Behaviors>
    <RoutedEventTriggerBehavior RoutedEvent="{x:Static Window.WindowClosedEvent}">
      <InvokeCommandAction Command="{Binding WindowClosedCommand}" />
    </RoutedEventTriggerBehavior>
  </Interaction.Behaviors>
</Window>
```
