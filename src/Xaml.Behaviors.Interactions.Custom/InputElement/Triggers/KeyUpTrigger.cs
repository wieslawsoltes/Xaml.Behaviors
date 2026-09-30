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
/// 
/// </summary>
public partial class KeyUpTrigger : RoutedEventTriggerBase<KeyEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<KeyEventArgs> RoutedEvent 
        => InputElement.KeyUpEvent;

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial Key? Key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial KeyGesture? Gesture { get; set; }

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
