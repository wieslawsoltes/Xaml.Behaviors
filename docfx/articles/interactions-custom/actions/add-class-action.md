# AddClassAction

Adds a specified class name to the `Classes` collection of a `StyledElement`. This is useful for triggering styles or animations defined in your XAML.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `ClassName` | `string` | The name of the class to add. |
| `StyledElement` | `StyledElement` | The target element to add the class to. If not set, the associated object is used. |
| `RemoveIfExists` | `bool` | If set to `True`, the class will be removed if it already exists before being added again. This is useful for restarting animations. |

### Example

```xml
<Button Content="Add Class">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <AddClassAction ClassName="my-class" RemoveIfExists="True" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
