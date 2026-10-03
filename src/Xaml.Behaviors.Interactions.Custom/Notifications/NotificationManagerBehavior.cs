// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Provides a <see cref="INotificationManager"/> for the associated <see cref="Control"/>.
/// </summary>
public partial class NotificationManagerBehavior : AttachedToVisualTreeBehavior<Control>
{

    /// <summary>
    /// Gets the <see cref="INotificationManager"/> instance. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial INotificationManager? NotificationManager { get; private set; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is TopLevel topLevel)
        {
            NotificationManager = new WindowNotificationManager(topLevel);
        }
        else
        {
            if (TopLevel.GetTopLevel(AssociatedObject) is TopLevel visualRootTopLevel)
            {
                NotificationManager = new WindowNotificationManager(visualRootTopLevel);
            }
        }

        return DisposableAction.Create(() => NotificationManager = null);
    }
}
