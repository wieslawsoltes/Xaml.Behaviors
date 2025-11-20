# HideOnKeyPressedBehavior

Hides a target control when a specific key is pressed while the associated control has focus.

### Properties
*   `TargetControl`: The control to hide.
*   `Key`: The key that triggers the hide action (default: Escape).

### Example

```xml
<TextBox>
    <Interaction.Behaviors>
        <HideOnKeyPressedBehavior TargetControl="{Binding #MyPopup}" Key="Escape" />
    </Interaction.Behaviors>
</TextBox>
```
