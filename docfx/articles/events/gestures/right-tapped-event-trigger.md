# RightTappedEventTrigger

Trigger that listens for the `Gestures.RightTappedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <RightTappedEventTrigger>
            <InvokeCommandAction Command="{Binding RightTappedCommand}" />
        </RightTappedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
