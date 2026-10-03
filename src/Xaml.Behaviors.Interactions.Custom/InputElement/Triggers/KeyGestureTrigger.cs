// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Trigger that listens for a key gesture.
/// </summary>
public partial class KeyGestureTrigger : RoutedEventTriggerBase<KeyEventArgs>
{

    /// <summary>
    /// Gets or sets the gesture that will fire the trigger.
    /// </summary>
    [StyledProperty]
    public partial KeyGesture? Gesture { get; set; }

    /// <summary>
    /// Gets or sets whether the trigger reacts on key down or key up.
    /// </summary>
    [StyledProperty(DefaultValue = KeyGestureTriggerFiredOn.KeyDown)]
    public partial KeyGestureTriggerFiredOn FiredOn { get; set; }

    static KeyGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<KeyGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override RoutedEvent<KeyEventArgs> RoutedEvent =>
        FiredOn == KeyGestureTriggerFiredOn.KeyUp
            ? InputElement.KeyUpEvent
            : InputElement.KeyDownEvent;

    /// <inheritdoc />
    protected override void Handler(object? sender, KeyEventArgs e)
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
