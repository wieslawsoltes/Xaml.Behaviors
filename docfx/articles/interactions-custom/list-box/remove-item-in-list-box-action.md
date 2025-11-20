# RemoveItemInListBoxAction

Removes a specific item from the `ListBox`'s `ItemsSource`. The `ItemsSource` must implement `IList` or a compatible interface.

### Properties

- `Item`: The item to remove.

### Usage Example

```xml
<Button Content="Remove Selected">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <RemoveItemInListBoxAction TargetObject="{Binding ElementName=MyListBox}" 
                                       Item="{Binding SelectedItem, ElementName=MyListBox}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
