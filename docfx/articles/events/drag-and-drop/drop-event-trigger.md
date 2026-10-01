# DropEventTrigger

Trigger that listens for the `DragDrop.DropEvent`.
The trigger also receives handled events, so it fires when a drop handler (for example `ContextDropBehavior`) on the
same element handles the drag.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <DropEventTrigger>
            <InvokeCommandAction Command="{Binding DropCommand}" />
        </DropEventTrigger>
    </Interaction.Behaviors>
</Border>
```
