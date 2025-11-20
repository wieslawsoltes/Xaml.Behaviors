# DragLeaveEventTrigger

Trigger that listens for the `DragDrop.DragLeaveEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <DragLeaveEventTrigger>
            <InvokeCommandAction Command="{Binding DragLeaveCommand}" />
        </DragLeaveEventTrigger>
    </Interaction.Behaviors>
</Border>
```
