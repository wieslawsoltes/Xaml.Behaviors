# GotFocusEventTrigger

Trigger that listens for the `InputElement.GotFocusEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Bubble`. |

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <GotFocusEventTrigger>
            <InvokeCommandAction Command="{Binding GotFocusCommand}" />
        </GotFocusEventTrigger>
    </Interaction.Behaviors>
</TextBox>
```
