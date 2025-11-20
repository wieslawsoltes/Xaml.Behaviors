# AutoCompleteBoxOpenDropDownOnFocusBehavior

This behavior automatically opens the dropdown list of suggestions as soon as the `AutoCompleteBox` receives focus. This is useful for "combo box" style interactions where you want the user to see options immediately.

### Example

```xml
<AutoCompleteBox ItemsSource="{Binding Suggestions}">
    <Interaction.Behaviors>
        <AutoCompleteBoxOpenDropDownOnFocusBehavior />
    </Interaction.Behaviors>
</AutoCompleteBox>
```
