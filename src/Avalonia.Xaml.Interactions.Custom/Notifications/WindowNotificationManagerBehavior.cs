using System;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides a <see cref="INotificationManager"/> for the associated <see cref="Window"/>.
/// </summary>
public class WindowNotificationManagerBehavior : AttachedToVisualTreeBehavior<Window>
{
    /// <summary>
    /// Identifies the <seealso cref="NotificationManager"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<INotificationManager?> NotificationManagerProperty =
        AvaloniaProperty.Register<WindowNotificationManagerBehavior, INotificationManager?>(nameof(NotificationManager));

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
        NotificationManager = new WindowNotificationManager(AssociatedObject);
        return DisposableAction.Create(() => NotificationManager = null);
    }
}
