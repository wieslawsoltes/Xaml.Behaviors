// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Shows a warning notification using an <see cref="INotificationManager"/>.
/// </summary>
public partial class ShowWarningNotificationAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the <see cref="INotificationManager"/> used to display the notification. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial INotificationManager? NotificationManager { get; set; }

    /// <summary>
    /// Gets or sets the notification title. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? Title { get; set; }

    /// <summary>
    /// Gets or sets the notification message. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? Message { get; set; }

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

        var notification = new Notification(Title ?? string.Empty, Message ?? string.Empty, NotificationType.Warning);
        manager.Show(notification);
        return true;
    }
}
