# ShowSuccessNotificationAction

Shows a success notification using an `INotificationManager`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| NotificationManager | `INotificationManager` | Gets or sets the `INotificationManager` used to display the notification. |
| Title | `string` | Gets or sets the notification title. |
| Message | `string` | Gets or sets the notification message. |

## Usage

```xml
<Button Content="Show Success">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowSuccessNotificationAction NotificationManager="{Binding #Window.NotificationManager}"
                                           Title="Success"
                                           Message="Operation completed successfully!" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
