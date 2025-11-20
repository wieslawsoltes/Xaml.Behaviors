# ToggleViewModelBooleanAction

Toggles a boolean view model property when invoked.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PropertyName | `string` | Gets or sets the name of the property to toggle. |

## Usage

```xml
<Button Content="Toggle">
    <Interaction.Actions>
        <EventTriggerBehavior EventName="Click">
            <ToggleViewModelBooleanAction PropertyName="IsChecked" />
        </EventTriggerBehavior>
    </Interaction.Actions>
</Button>
```
