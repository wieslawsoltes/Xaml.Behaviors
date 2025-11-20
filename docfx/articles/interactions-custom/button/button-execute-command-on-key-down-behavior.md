# ButtonExecuteCommandOnKeyDownBehavior

This behavior allows a button's command to be executed when a specific key is pressed, regardless of where focus is within the window (it attaches to the visual root). This is useful for setting up global shortcuts that trigger specific UI buttons.

### Properties
*   `Key`: The key that triggers the command.
*   `Gesture`: A `KeyGesture` that triggers the command.

### Example

```xml
<Button Command="{Binding SaveCommand}" Content="Save">
    <Interaction.Behaviors>
        <!-- Pressing Ctrl+S anywhere will trigger this button's command -->
        <ButtonExecuteCommandOnKeyDownBehavior Gesture="Ctrl+S" />
    </Interaction.Behaviors>
</Button>
```
