# RemoveClassAction

Removes a specified class name from the `Classes` collection of a `StyledElement`.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `ClassName` | `string` | The name of the class to remove. |
| `StyledElement` | `StyledElement` | The target element to remove the class from. If not set, the associated object is used. |

### Example

```xml
<Button Content="Remove Class">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <RemoveClassAction ClassName="my-class" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
