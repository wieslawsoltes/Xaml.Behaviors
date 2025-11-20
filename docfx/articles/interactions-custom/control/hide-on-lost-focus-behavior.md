# HideOnLostFocusBehavior

Hides a target control when the associated control loses focus. This is commonly used for custom dropdowns or popups that should close when the user clicks away.

### Properties
*   `TargetControl`: The control to hide.

### Example

```xml
<TextBox>
    <Interaction.Behaviors>
        <HideOnLostFocusBehavior TargetControl="{Binding #SuggestionList}" />
    </Interaction.Behaviors>
</TextBox>
```
