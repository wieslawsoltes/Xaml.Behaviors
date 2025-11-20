# ShowErrorNotificationAction

Shows an error notification using an `INotificationManager`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| NotificationManager | `INotificationManager` | Gets or sets the `INotificationManager` used to display the notification. |
| Title | `string` | Gets or sets the notification title. |
| Message | `string` | Gets or sets the notification message. |

## Usage

```xml
<Button Content="Show Error">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowErrorNotificationAction NotificationManager="{Binding #Window.NotificationManager}"
                                         Title="Error"
                                         Message="Something went wrong!" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
