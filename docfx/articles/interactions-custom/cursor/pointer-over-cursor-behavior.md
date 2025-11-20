# PointerOverCursorBehavior

Changes the cursor of the associated `InputElement` when the pointer enters its bounds and restores it (clears the local value) when the pointer exits.

### Properties
- `Cursor`: The cursor to display when the pointer is over the control.

### Usage Example

```xml
<Border Background="LightGray" Width="100" Height="100">
    <Interaction.Behaviors>
        <PointerOverCursorBehavior Cursor="Hand" />
    </Interaction.Behaviors>
</Border>
```
