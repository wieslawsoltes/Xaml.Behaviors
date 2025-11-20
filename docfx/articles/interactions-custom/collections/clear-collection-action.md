# ClearCollectionAction

Removes all items from a target `IList`.

### Properties
*   `Target`: The `IList` to clear.

### Example

```xml
<Button Content="Clear All">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ClearCollectionAction Target="{Binding MyItems}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
