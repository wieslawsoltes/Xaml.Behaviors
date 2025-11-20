# FocusSelectedItemBehavior

Attaches to an `ItemsControl` (like `ListBox` or `ComboBox`) and automatically focuses the container of the selected item whenever the selection changes.

### Usage Example

```xml
<ListBox ItemsSource="{Binding Items}" SelectedItem="{Binding SelectedItem}">
    <Interaction.Behaviors>
        <FocusSelectedItemBehavior />
    </Interaction.Behaviors>
</ListBox>
```
