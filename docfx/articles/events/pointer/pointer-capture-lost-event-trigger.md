# PointerCaptureLostEventTrigger

Trigger that listens for the `InputElement.PointerCaptureLostEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerCaptureLostEventTrigger>
            <InvokeCommandAction Command="{Binding PointerCaptureLostCommand}" />
        </PointerCaptureLostEventTrigger>
    </Interaction.Behaviors>
</Border>
```
