# DragOverEventTrigger

Trigger that listens for the `DragDrop.DragOverEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <DragOverEventTrigger>
            <InvokeCommandAction Command="{Binding DragOverCommand}" />
        </DragOverEventTrigger>
    </Interaction.Behaviors>
</Border>
```
