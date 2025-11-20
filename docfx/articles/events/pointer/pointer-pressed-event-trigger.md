# PointerPressedEventTrigger

Trigger that listens for the `InputElement.PointerPressedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerPressedEventTrigger>
            <InvokeCommandAction Command="{Binding PointerPressedCommand}" />
        </PointerPressedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
