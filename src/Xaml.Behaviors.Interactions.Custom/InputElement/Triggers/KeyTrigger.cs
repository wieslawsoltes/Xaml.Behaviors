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
/// A trigger that listens for key events and executes its actions when the
/// specified key or gesture is detected.
/// </summary>
public partial class KeyTrigger : RoutedEventTriggerBase<KeyEventArgs>
{
    /// <summary>
    /// Specifies which keyboard event will fire the trigger.
    /// </summary>
    public enum FiredOn
    {
        /// <summary>
        /// Trigger on the <see cref="InputElement.KeyDownEvent"/>.
        /// </summary>
        KeyDown,

        /// <summary>
        /// Trigger on the <see cref="InputElement.KeyUpEvent"/>.
        /// </summary>
        KeyUp
    }

    /// <inheritdoc />
    protected override RoutedEvent<KeyEventArgs> RoutedEvent
        => Event == FiredOn.KeyDown ? InputElement.KeyDownEvent : InputElement.KeyUpEvent;

    /// <summary>
    /// Gets or sets the key to listen for. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Key? Key { get; set; }

    /// <summary>
    /// Gets or sets the key gesture to match. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial KeyGesture? Gesture { get; set; }

    /// <summary>
    /// Gets or sets which key event fires the trigger. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = FiredOn.KeyDown)]
    public partial FiredOn Event { get; set; }

    /// <inheritdoc />
    protected override void Handler(object? sender, KeyEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        var isKeySet = IsSet(KeyProperty);
        var isGestureSet = IsSet(GestureProperty);
        var key = Key;
        var gesture = Gesture;
        var haveKey = key is not null && isKeySet && e.Key == key;
        var haveGesture = gesture is not null && isGestureSet && gesture.Matches(e);

        if ((!isKeySet && !isGestureSet)
            || haveKey
            || haveGesture)
        {
            Execute(e);
        }
    }
}
