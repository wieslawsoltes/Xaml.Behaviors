# PointerExitedEventTrigger

Trigger that listens for the `InputElement.PointerExitedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerExitedEventTrigger>
            <InvokeCommandAction Command="{Binding PointerExitedCommand}" />
        </PointerExitedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
