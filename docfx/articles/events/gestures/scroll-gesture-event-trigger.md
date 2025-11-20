# ScrollGestureEventTrigger

Trigger that listens for the `Gestures.ScrollGestureEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <ScrollGestureEventTrigger>
            <InvokeCommandAction Command="{Binding ScrollGestureCommand}" />
        </ScrollGestureEventTrigger>
    </Interaction.Behaviors>
</Border>
```
