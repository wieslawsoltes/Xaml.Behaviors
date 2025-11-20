# ShowWarningNotificationAction

Shows a warning notification using an `INotificationManager`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| NotificationManager | `INotificationManager` | Gets or sets the `INotificationManager` used to display the notification. |
| Title | `string` | Gets or sets the notification title. |
| Message | `string` | Gets or sets the notification message. |

## Usage

```xml
<Button Content="Show Warning">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowWarningNotificationAction NotificationManager="{Binding #Window.NotificationManager}"
                                           Title="Warning"
                                           Message="Be careful!" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
