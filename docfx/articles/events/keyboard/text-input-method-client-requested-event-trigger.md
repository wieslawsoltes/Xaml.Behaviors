# TextInputMethodClientRequestedEventTrigger

Trigger that listens for the `InputElement.TextInputMethodClientRequestedEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <TextInputMethodClientRequestedEventTrigger>
            <InvokeCommandAction Command="{Binding TextInputMethodClientRequestedCommand}" />
        </TextInputMethodClientRequestedEventTrigger>
    </Interaction.Behaviors>
</TextBox>
```
