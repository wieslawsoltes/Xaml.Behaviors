# SetViewModelPropertyAction

Sets a view model property to a specified value when invoked.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PropertyName | `string` | Gets or sets the name of the property to change. |
| Value | `object` | Gets or sets the value to assign. |

## Usage

```xml
<Button Content="Set Value">
    <Interaction.Actions>
        <EventTriggerBehavior EventName="Click">
            <SetViewModelPropertyAction PropertyName="Status" Value="Active" />
        </EventTriggerBehavior>
    </Interaction.Actions>
</Button>
```
