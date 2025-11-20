# DragEnterEventTrigger

Trigger that listens for the `DragDrop.DragEnterEvent`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <DragEnterEventTrigger>
            <InvokeCommandAction Command="{Binding DragEnterCommand}" />
        </DragEnterEventTrigger>
    </Interaction.Behaviors>
</Border>
```
