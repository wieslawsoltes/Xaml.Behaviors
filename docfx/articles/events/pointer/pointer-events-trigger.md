# PointerEventsTrigger

Trigger that listens for multiple pointer events (`PointerPressed`, `PointerReleased`, `PointerMoved`, `PointerEntered`, `PointerExited`, `PointerCaptureLost`, `PointerWheelChanged`).

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerEventsTrigger>
            <InvokeCommandAction Command="{Binding PointerEventsCommand}" />
        </PointerEventsTrigger>
    </Interaction.Behaviors>
</Border>
```
