# AutoScrollDuringDragBehavior

`AutoScrollDuringDragBehavior` automatically scrolls a `ScrollViewer` when a drag operation (initiated by other behaviors in this package) reaches the edge of the viewport.

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
