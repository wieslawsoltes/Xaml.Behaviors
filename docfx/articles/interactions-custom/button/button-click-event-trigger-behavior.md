# ButtonClickEventTriggerBehavior

This trigger listens for the `Click` event on the associated `Button`. Unlike a standard `EventTriggerBehavior`, it includes built-in support for keyboard activation (Enter/Space) and allows you to specify required `KeyModifiers`.

### Properties
*   `KeyModifiers`: The modifier keys (e.g., Control, Shift) that must be pressed for the trigger to fire.

### Example

```xml
<Button Content="Ctrl+Click Me">
    <Interaction.Behaviors>
        <ButtonClickEventTriggerBehavior KeyModifiers="Control">
            <CallMethodAction TargetObject="{Binding}" MethodName="OnCtrlClick" />
        </ButtonClickEventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
