# CanvasDragBehavior

`CanvasDragBehavior` enables dragging of child controls within a `Canvas`. It updates the `Canvas.Left` and `Canvas.Top` attached properties of the dragged element.

## Usage

Attach the behavior to the `Canvas` that contains the items you want to drag.

```xml
<Canvas Width="400" Height="400" Background="LightGray">
    <Interaction.Behaviors>
        <CanvasDragBehavior />
    </Interaction.Behaviors>

    <Rectangle Canvas.Left="50" Canvas.Top="50" Width="50" Height="50" Fill="Red" />
    <Rectangle Canvas.Left="150" Canvas.Top="150" Width="50" Height="50" Fill="Blue" />
</Canvas>
```

When you click and drag any of the rectangles, they will move within the Canvas.

## Notes

*   The behavior handles pointer events on the `Canvas` and determines which child was clicked.
*   It automatically updates the `Canvas.Left` and `Canvas.Top` properties.
