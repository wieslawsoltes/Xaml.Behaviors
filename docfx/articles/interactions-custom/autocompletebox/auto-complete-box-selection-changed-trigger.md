# AutoCompleteBoxSelectionChangedTrigger

This is a specialized `EventTrigger` designed specifically for the `AutoCompleteBox.SelectionChanged` event. It simplifies the XAML by not requiring you to specify the `EventName` string manually.

### Example

```xml
<AutoCompleteBox ItemsSource="{Binding Suggestions}">
    <Interaction.Behaviors>
        <AutoCompleteBoxSelectionChangedTrigger>
            <InvokeCommandAction Command="{Binding OnSelectionChangedCommand}" 
                                    CommandParameter="{Binding $self.SelectedItem}" />
        </AutoCompleteBoxSelectionChangedTrigger>
    </Interaction.Behaviors>
</AutoCompleteBox>
```
