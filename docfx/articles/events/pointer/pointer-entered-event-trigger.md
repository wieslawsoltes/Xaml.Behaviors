# PointerEnteredEventTrigger

Trigger that listens for the `InputElement.PointerEnteredEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerEnteredEventTrigger>
            <InvokeCommandAction Command="{Binding PointerEnteredCommand}" />
        </PointerEnteredEventTrigger>
    </Interaction.Behaviors>
</Border>
```
