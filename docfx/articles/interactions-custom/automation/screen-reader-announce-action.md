# ScreenReaderAnnounceAction

The `ScreenReaderAnnounceAction` is an action that requests a screen reader announcement.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Message | string | The message to announce. |

## Usage

```xml
<Button Content="Announce">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ScreenReaderAnnounceAction Message="Hello World" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

## Remarks

This action uses `TopLevel.RequestScreenReaderAnnouncement` API which is available in Avalonia 11.1 and later. If the API is not available on the current platform or version, the action will do nothing.
