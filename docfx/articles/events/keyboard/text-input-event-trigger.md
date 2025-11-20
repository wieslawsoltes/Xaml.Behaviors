# TextInputEventTrigger

Trigger that listens for the `InputElement.TextInputEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <TextInputEventTrigger>
            <InvokeCommandAction Command="{Binding TextInputCommand}" />
        </TextInputEventTrigger>
    </Interaction.Behaviors>
</TextBox>
```
