# ShowNotificationAction

Shows a notification using an `INotificationManager`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| NotificationManager | `INotificationManager` | Gets or sets the `INotificationManager` used to display the notification. |
| Notification | `INotification` | Gets or sets the `INotification` instance to show. |

## Usage

```xml
<Button Content="Show Notification">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowNotificationAction NotificationManager="{Binding #Window.NotificationManager}">
                <ShowNotificationAction.Notification>
                    <Notification Title="Hello" Message="World!" Type="Information" />
                </ShowNotificationAction.Notification>
            </ShowNotificationAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
