# DropEventTrigger

Trigger that listens for the `DragDrop.DropEvent`.

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
