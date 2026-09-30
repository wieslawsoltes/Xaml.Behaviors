// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Shows a notification using an <see cref="INotificationManager"/>.
/// </summary>
public partial class ShowNotificationAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the <see cref="INotificationManager"/> used to display the notification. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial INotificationManager? NotificationManager { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="INotification"/> instance to show. This is an avalonia property.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial INotification? Notification { get; set; }

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
