# IncrementViewModelPropertyAction

Increments a numeric view model property when invoked.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PropertyName | `string` | Gets or sets the name of the property to change. |
| Delta | `double` | Gets or sets the value to add. Default is 1. |

## Usage

```xml
<Button Content="Increment">
    <Interaction.Actions>
        <EventTriggerBehavior EventName="Click">
            <IncrementViewModelPropertyAction PropertyName="Count" Delta="1" />
        </EventTriggerBehavior>
    </Interaction.Actions>
</Button>
```
