# LostFocusEventTrigger

Trigger that listens for the `InputElement.LostFocusEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Bubble`. |

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <LostFocusEventTrigger>
            <InvokeCommandAction Command="{Binding LostFocusCommand}" />
        </LostFocusEventTrigger>
    </Interaction.Behaviors>
</TextBox>
```
