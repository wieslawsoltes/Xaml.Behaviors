# TappedEventTrigger

Trigger that listens for the `Gestures.TappedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <TappedEventTrigger>
            <InvokeCommandAction Command="{Binding TappedCommand}" />
        </TappedEventTrigger>
    </Interaction.Behaviors>
</Border>
```
