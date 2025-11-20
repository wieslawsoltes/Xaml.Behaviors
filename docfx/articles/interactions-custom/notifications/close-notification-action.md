# CloseNotificationAction

Action that closes the specified `NotificationCard`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| NotificationCard | `NotificationCard` | Gets or sets the notification card to close. |

## Usage

```xml
<Button Content="Close Notification">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <CloseNotificationAction NotificationCard="{Binding NotificationCard}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
