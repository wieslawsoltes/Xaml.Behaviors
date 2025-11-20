# WindowAction

An action that performs common window operations.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| ActionType | WindowActionType | The type of action to perform (Close, Minimize, Maximize, Restore, ToggleFullScreen). |

## Usage

```xml
<Button Content="Close">
    <Interaction.Actions>
        <EventTriggerBehavior EventName="Click">
            <WindowAction ActionType="Close" />
        </EventTriggerBehavior>
    </Interaction.Actions>
</Button>
```
