# ShowNotificationAction

Shows a notification with customizable title, message, and type.

### Properties

- `Title`: The title of the notification.
- `Message`: The content of the notification.
- `NotificationType`: The type of notification (`Information`, `Success`, `Warning`, `Error`).
- `NotificationManager`: The `INotificationManager` to use. If not set, it attempts to resolve one from the visual tree.
