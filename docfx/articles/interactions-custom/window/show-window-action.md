# ShowWindowAction

Shows the specified window when executed.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Window | `Window` | Gets or sets the window instance to show. |

## Usage

```xml
<Button Content="Show Window">
    <Interaction.Actions>
        <EventTriggerBehavior EventName="Click">
            <ShowWindowAction>
                <Window Title="New Window" Width="400" Height="300" />
            </ShowWindowAction>
        </EventTriggerBehavior>
    </Interaction.Actions>
</Button>
```
