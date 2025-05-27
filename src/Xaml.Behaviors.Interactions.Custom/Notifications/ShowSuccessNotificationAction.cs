// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows a success notification using an <see cref="INotificationManager"/>.
/// </summary>
public class ShowSuccessNotificationAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="NotificationManager"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<INotificationManager?> NotificationManagerProperty =
        AvaloniaProperty.Register<ShowSuccessNotificationAction, INotificationManager?>(nameof(NotificationManager));

    /// <summary>
    /// Identifies the <seealso cref="Title"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ShowSuccessNotificationAction, string?>(nameof(Title));

    /// <summary>
    /// Identifies the <seealso cref="Message"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<ShowSuccessNotificationAction, string?>(nameof(Message));

    /// <summary>
    /// Gets or sets the <see cref="INotificationManager"/> used to display the notification. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public INotificationManager? NotificationManager
    {
        get => GetValue(NotificationManagerProperty);
        set => SetValue(NotificationManagerProperty, value);
    }

    /// <summary>
    /// Gets or sets the notification title. This is an avalonia property.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the notification message. This is an avalonia property.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var manager = NotificationManager;
        if (manager is null)
        {
            return false;
        }

        var notification = new Notification(Title ?? string.Empty, Message ?? string.Empty, NotificationType.Success);
        manager.Show(notification);
        return true;
    }
}
