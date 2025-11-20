# PointerReleasedEventTrigger

Trigger that listens for the `InputElement.PointerReleasedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerReleasedEventTrigger>
            <InvokeCommandAction Command="{Binding PointerReleasedCommand}" />
        </PointerReleasedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
