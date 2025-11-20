# KeyDownEventTrigger

Trigger that listens for the `InputElement.KeyDownEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <KeyDownEventTrigger>
            <InvokeCommandAction Command="{Binding KeyDownCommand}" />
        </KeyDownEventTrigger>
    </Interaction.Behaviors>
</TextBox>
```
