# MouseDragElementBehavior

`MouseDragElementBehavior` allows you to make any control draggable by the mouse. It works by manipulating the `RenderTransform` of the associated object.

## Properties

*   **`ConstrainToParentBounds`**: If set to `True`, the control cannot be dragged outside the bounds of its parent container.

## Usage

Attach the behavior directly to the control you want to make draggable.

```xml
<Border Background="LightGreen" Width="100" Height="100">
    <Interaction.Behaviors>
        <MouseDragElementBehavior ConstrainToParentBounds="True" />
    </Interaction.Behaviors>
    <TextBlock Text="Drag Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```

## MultiMouseDragElementBehavior

`MultiMouseDragElementBehavior` is similar but designed to work with multiple elements, potentially coordinating their movement (though its primary use case is similar to `MouseDragElementBehavior` but with slightly different implementation details regarding transforms).
