# FocusAutoCompleteBoxTextBoxBehavior

Sometimes, giving focus to the `AutoCompleteBox` container doesn't correctly propagate focus to the internal `TextBox` used for editing. This behavior listens for the `GotFocus` event on the `AutoCompleteBox` and explicitly sets focus to its inner `TextBox`.

### Example

```xml
<AutoCompleteBox ItemsSource="{Binding Suggestions}">
    <Interaction.Behaviors>
        <FocusAutoCompleteBoxTextBoxBehavior />
    </Interaction.Behaviors>
</AutoCompleteBox>
```
