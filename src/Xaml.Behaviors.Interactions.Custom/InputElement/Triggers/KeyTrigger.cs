// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that listens for key events and executes its actions when the
/// specified key or gesture is detected.
/// </summary>
public class KeyTrigger : RoutedEventTriggerBase<KeyEventArgs>
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
    /// Identifies the <seealso cref="Key"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Key?> KeyProperty =
        AvaloniaProperty.Register<KeyTrigger, Key?>(nameof(Key));

    /// <summary>
    /// Identifies the <seealso cref="Gesture"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<KeyTrigger, KeyGesture?>(nameof(Gesture));

    /// <summary>
    /// Identifies the <seealso cref="Event"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<FiredOn> EventProperty =
        AvaloniaProperty.Register<KeyTrigger, FiredOn>(nameof(Event), FiredOn.KeyDown);

    /// <summary>
    /// Gets or sets the key to listen for. This is an avalonia property.
    /// </summary>
    public Key? Key
    {
        get => GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the key gesture to match. This is an avalonia property.
    /// </summary>
    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }

    /// <summary>
    /// Gets or sets which key event fires the trigger. This is an avalonia property.
    /// </summary>
    public FiredOn Event
    {
        get => GetValue(EventProperty);
        set => SetValue(EventProperty, value);
    }

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
