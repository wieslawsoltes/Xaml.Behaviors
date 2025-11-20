# AutoScrollDuringDragBehavior

`AutoScrollDuringDragBehavior` automatically scrolls a `ScrollViewer` when a drag operation (initiated by other behaviors in this package) reaches the edge of the viewport.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| EdgeDistance | `double` | Gets or sets the distance from the edge that triggers scrolling. Default is 20. |
| ScrollDelta | `double` | Gets or sets the amount scrolled when triggered. Default is 10. |

## Usage

Attach this behavior to the `ScrollViewer` or the control containing the scrollable area.

```xml
<ScrollViewer>
    <Interaction.Behaviors>
        <AutoScrollDuringDragBehavior />
    </Interaction.Behaviors>
    
    <ItemsControl>
        <!-- Draggable items here -->
    </ItemsControl>
</ScrollViewer>
```
