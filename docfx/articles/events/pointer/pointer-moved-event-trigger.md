# PointerMovedEventTrigger

Trigger that listens for the `InputElement.PointerMovedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerMovedEventTrigger>
            <InvokeCommandAction Command="{Binding PointerMovedCommand}" />
        </PointerMovedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
