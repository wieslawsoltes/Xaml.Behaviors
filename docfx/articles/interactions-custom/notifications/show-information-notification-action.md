# ShowInformationNotificationAction

Shows an information notification using an `INotificationManager`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| NotificationManager | `INotificationManager` | Gets or sets the `INotificationManager` used to display the notification. |
| Title | `string` | Gets or sets the notification title. |
| Message | `string` | Gets or sets the notification message. |

## Usage

```xml
<Button Content="Show Info">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowInformationNotificationAction NotificationManager="{Binding #Window.NotificationManager}"
                                               Title="Info"
                                               Message="This is an information message." />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
