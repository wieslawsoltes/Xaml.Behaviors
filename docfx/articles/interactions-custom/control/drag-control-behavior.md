# DragControlBehavior

This behavior allows the user to drag the associated control (or a target control) around the canvas using the mouse. It works by modifying the `RenderTransform` of the control.

### Properties
*   `TargetControl`: The control to move. If not set, the associated object is moved.

### Example

```xml
<Canvas>
    <Border Background="Green" Width="50" Height="50">
        <Interaction.Behaviors>
            <DragControlBehavior />
        </Interaction.Behaviors>
    </Border>
</Canvas>
```
