# PointerWheelChangedEventTrigger

Trigger that listens for the `InputElement.PointerWheelChangedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerWheelChangedEventTrigger>
            <InvokeCommandAction Command="{Binding PointerWheelChangedCommand}" />
        </PointerWheelChangedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
