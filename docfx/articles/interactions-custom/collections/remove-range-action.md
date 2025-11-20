# RemoveRangeAction

Removes a collection of items from a target `IList`.

### Properties
*   `Target`: The `IList` to remove items from.
*   `Items`: The `IEnumerable` of items to remove.

### Example

```xml
<Button Content="Remove Selected">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <RemoveRangeAction Target="{Binding MyItems}" Items="{Binding SelectedItems}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
