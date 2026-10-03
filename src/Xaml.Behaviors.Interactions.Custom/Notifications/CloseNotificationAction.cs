// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if !UNO
using Avalonia.Controls.Notifications;
using Avalonia.Metadata;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Action that closes the specified <see cref="NotificationCard"/>.
/// </summary>
public partial class CloseNotificationAction : Interactivity.StyledElementAction
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(Content = true)]
    public partial NotificationCard? NotificationCard { get; set; }

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
