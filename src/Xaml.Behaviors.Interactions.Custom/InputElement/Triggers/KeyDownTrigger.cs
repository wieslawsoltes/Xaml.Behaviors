// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class KeyDownTrigger : RoutedEventTriggerBase<KeyEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<KeyEventArgs> RoutedEvent 
        => InputElement.KeyDownEvent;

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<Key?> KeyProperty =
        AvaloniaProperty.Register<KeyDownTrigger, Key?>(nameof(Key));

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<KeyDownTrigger, KeyGesture?>(nameof(Gesture));

    /// <summary>
    /// 
    /// </summary>
    public Key? Key
    {
        get => GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
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
