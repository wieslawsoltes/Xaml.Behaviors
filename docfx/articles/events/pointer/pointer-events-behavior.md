# PointerEventsBehavior

Behavior that handles multiple pointer events (`PointerPressed`, `PointerReleased`, `PointerMoved`, `PointerEntered`, `PointerExited`, `PointerCaptureLost`, `PointerWheelChanged`).

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RoutingStrategies | `RoutingStrategies` | Gets or sets the routing strategies used when subscribing to events. Default is `Tunnel | Bubble`. |

## Usage

```xml
<Border Background="LightGray">
    <Interaction.Behaviors>
        <PointerEventsBehavior />
    </Interaction.Behaviors>
</Border>
```
