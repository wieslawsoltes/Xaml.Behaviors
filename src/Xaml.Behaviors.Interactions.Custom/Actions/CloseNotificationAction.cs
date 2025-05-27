// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls.Notifications;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Action that closes the specified <see cref="NotificationCard"/>.
/// </summary>
public class CloseNotificationAction : Avalonia.Xaml.Interactivity.StyledElementAction
{
    /// <summary>
    /// Gets or sets the notification card to close.
    /// </summary>
    public static readonly StyledProperty<NotificationCard?> NotificationCardProperty =
        AvaloniaProperty.Register<CloseNotificationAction, NotificationCard?>(nameof(NotificationCard));

    /// <summary>
    /// 
    /// </summary>
    public NotificationCard? NotificationCard
    {
        get => GetValue(NotificationCardProperty);
        set => SetValue(NotificationCardProperty, value);
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The associated object.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <returns>True if the notification was closed; otherwise, false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (NotificationCard is null)
        {
            return false;
        }

        NotificationCard.Close();
        return true;
    }
}
