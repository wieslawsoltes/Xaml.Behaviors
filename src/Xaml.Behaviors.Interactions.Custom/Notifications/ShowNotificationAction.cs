// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows a notification using an <see cref="INotificationManager"/>.
/// </summary>
public class ShowNotificationAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="NotificationManager"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<INotificationManager?> NotificationManagerProperty =
        AvaloniaProperty.Register<ShowNotificationAction, INotificationManager?>(nameof(NotificationManager));

    /// <summary>
    /// Identifies the <seealso cref="Notification"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<INotification?> NotificationProperty =
        AvaloniaProperty.Register<ShowNotificationAction, INotification?>(nameof(Notification));

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
    /// Gets or sets the <see cref="INotification"/> instance to show. This is an avalonia property.
    /// </summary>
    public INotification? Notification
    {
        get => GetValue(NotificationProperty);
        set => SetValue(NotificationProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var manager = NotificationManager;
        var notification = Notification;

        if (manager is null || notification is null)
        {
            return false;
        }

        manager.Show(notification);
        return true;
    }
}
