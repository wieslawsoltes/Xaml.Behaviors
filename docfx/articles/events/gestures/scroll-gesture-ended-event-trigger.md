# ScrollGestureEndedEventTrigger

Trigger that listens for the `Gestures.ScrollGestureEndedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <ScrollGestureEndedEventTrigger>
            <InvokeCommandAction Command="{Binding ScrollGestureEndedCommand}" />
        </ScrollGestureEndedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
