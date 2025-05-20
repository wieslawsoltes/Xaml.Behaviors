using System;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for a key gesture.
/// </summary>
public class KeyGestureTrigger : RoutedEventTriggerBase<KeyEventArgs>
{
    /// <summary>
    /// Identifies the <see cref="Gesture"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<KeyGestureTrigger, KeyGesture?>(nameof(Gesture));

    /// <summary>
    /// Identifies the <see cref="FiredOn"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<KeyGestureTriggerFiredOn> FiredOnProperty =
        AvaloniaProperty.Register<KeyGestureTrigger, KeyGestureTriggerFiredOn>(nameof(FiredOn),
            defaultValue: KeyGestureTriggerFiredOn.KeyDown);

    /// <summary>
    /// Gets or sets the gesture that will fire the trigger.
    /// </summary>
    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the trigger reacts on key down or key up.
    /// </summary>
    public KeyGestureTriggerFiredOn FiredOn
    {
        get => GetValue(FiredOnProperty);
        set => SetValue(FiredOnProperty, value);
    }

    static KeyGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<KeyGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is Interactive interactive)
        {
            var routedEvent = FiredOn == KeyGestureTriggerFiredOn.KeyUp
                ? InputElement.KeyUpEvent
                : InputElement.KeyDownEvent;

            return interactive.AddDisposableHandler(routedEvent, Handler, EventRoutingStrategy);
        }

        return DisposableAction.Empty;
    }

    private void Handler(object? sender, KeyEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        var gesture = Gesture;
        if (gesture is null || gesture.Matches(e))
        {
            Execute(e);
        }
    }
}

/// <summary>
/// Specifies when a <see cref="KeyGestureTrigger"/> should be fired.
/// </summary>
public enum KeyGestureTriggerFiredOn
{
    /// <summary>
    /// Trigger on key down.
    /// </summary>
    KeyDown,
    /// <summary>
    /// Trigger on key up.
    /// </summary>
    KeyUp
}
