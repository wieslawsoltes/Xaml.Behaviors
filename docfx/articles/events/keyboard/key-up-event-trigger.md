# KeyUpEventTrigger

Trigger that listens for the `InputElement.KeyUpEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <KeyUpEventTrigger>
            <InvokeCommandAction Command="{Binding KeyUpCommand}" />
        </KeyUpEventTrigger>
    </Interaction.Behaviors>
</TextBox>
```
