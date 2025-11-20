# AddRangeAction

Adds a collection of items to a target `IList`.

### Properties
*   `Target`: The `IList` to add items to.
*   `Items`: The `IEnumerable` of items to add.

### Example

```xml
<Button Content="Add More Items">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <AddRangeAction Target="{Binding MyItems}" Items="{Binding NewItems}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
