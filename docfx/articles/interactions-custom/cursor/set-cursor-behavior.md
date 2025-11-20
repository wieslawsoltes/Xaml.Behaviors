# SetCursorBehavior

Sets the cursor of the associated `InputElement` when it is attached to the visual tree and clears it when detached.

### Properties
- `Cursor`: The cursor to apply.

### Usage Example

```xml
<Border Background="LightBlue" Width="100" Height="100">
    <Interaction.Behaviors>
        <SetCursorBehavior Cursor="Help" />
    </Interaction.Behaviors>
</Border>
```
