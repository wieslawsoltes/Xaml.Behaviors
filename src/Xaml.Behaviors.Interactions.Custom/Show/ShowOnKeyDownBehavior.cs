// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows to show control on key down event.
/// </summary>
public class ShowOnKeyDownBehavior : ShowBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="Key"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Key?> KeyProperty =
        AvaloniaProperty.Register<ShowOnKeyDownBehavior, Key?>(nameof(Key));

    /// <summary>
    /// Gets or sets the key gesture used to trigger the behavior.
    /// </summary>
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<ShowOnKeyDownBehavior, KeyGesture?>(nameof(Gesture));

    /// <summary>
    /// Gets or sets the key. This is an avalonia property.
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

    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable that removes the event handler.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var dispose = AssociatedObject?
            .AddDisposableHandler(
                InputElement.KeyDownEvent, 
                AssociatedObject_KeyDown, 
                EventRoutingStrategy);

        if (dispose is not null)
        {
            return dispose;
        }
        
        return DisposableAction.Empty;
    }

    private void AssociatedObject_KeyDown(object? sender, KeyEventArgs e)
    {
        var haveKey = Key is not null && e.Key == Key;
        var haveGesture = Gesture is not null && Gesture.Matches(e);

        if (!haveKey && !haveGesture)
        {
            return;
        }
        
        Show();
    }
}
