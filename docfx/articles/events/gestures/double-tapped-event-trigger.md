# DoubleTappedEventTrigger

Trigger that listens for the `Gestures.DoubleTappedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <DoubleTappedEventTrigger>
            <InvokeCommandAction Command="{Binding DoubleTappedCommand}" />
        </DoubleTappedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
