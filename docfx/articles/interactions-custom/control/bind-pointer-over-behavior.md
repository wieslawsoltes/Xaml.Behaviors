# BindPointerOverBehavior

This behavior exposes the read-only `IsPointerOver` property of a control as a two-way bindable property. This is useful when you need to bind a ViewModel property to the hover state of a control.

### Properties
*   `IsPointerOver`: The boolean property that reflects the control's pointer-over state.

### Example

```xml
<Border Background="Gray" Width="100" Height="100">
    <Interaction.Behaviors>
        <BindPointerOverBehavior IsPointerOver="{Binding IsMouseOverSquare, Mode=TwoWay}" />
    </Interaction.Behaviors>
</Border>
```
