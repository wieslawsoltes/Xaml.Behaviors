# FocusBehavior

Keeps a control focused while the `IsFocused` property is `true`. This behavior supports two-way binding, so if the control loses focus, the property will update to `false`.

### Properties
- `IsFocused`: A boolean indicating whether the control should be focused.

### Usage Example

```xml
<TextBox>
    <Interaction.Behaviors>
        <FocusBehavior IsFocused="{Binding IsSearchFieldFocused}" />
    </Interaction.Behaviors>
</TextBox>
```
