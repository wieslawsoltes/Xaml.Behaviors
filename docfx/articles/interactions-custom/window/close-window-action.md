# CloseWindowAction

Closes the associated or target window when executed.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TargetWindow | `Window` | Gets or sets the target window. |

## Usage

```xml
<Button Content="Close">
    <Interaction.Actions>
        <EventTriggerBehavior EventName="Click">
            <CloseWindowAction />
        </EventTriggerBehavior>
    </Interaction.Actions>
</Button>
```
