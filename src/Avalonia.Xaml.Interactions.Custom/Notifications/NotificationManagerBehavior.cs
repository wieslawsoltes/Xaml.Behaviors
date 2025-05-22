using System;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides a <see cref="INotificationManager"/> for the associated <see cref="Control"/>.
/// </summary>
public class NotificationManagerBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="NotificationManager"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<INotificationManager?> NotificationManagerProperty =
        AvaloniaProperty.Register<NotificationManagerBehavior, INotificationManager?>(nameof(NotificationManager));

    /// <summary>
    /// Gets the <see cref="INotificationManager"/> instance. This is an avalonia property.
    /// </summary>
    public INotificationManager? NotificationManager
    {
        get => GetValue(NotificationManagerProperty);
        private set => SetValue(NotificationManagerProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is TopLevel topLevel)
        {
            NotificationManager = new WindowNotificationManager(topLevel);
        }
        else
        {
            if (AssociatedObject?.GetVisualRoot() is TopLevel visualRootTopLevel)
            {
                NotificationManager = new WindowNotificationManager(visualRootTopLevel);
            }
        }

        return DisposableAction.Create(() => NotificationManager = null);
    }
}
