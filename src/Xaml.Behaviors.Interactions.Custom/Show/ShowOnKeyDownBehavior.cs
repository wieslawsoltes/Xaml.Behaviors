// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
using KeyGesture = Microsoft.UI.Xaml.Input.KeyboardAccelerator;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that allows to show control on key down event.
/// </summary>
/// <remarks>
/// On Uno Platform the key is a <c>Windows.System.VirtualKey</c> and the gesture a WinUI
/// <c>KeyboardAccelerator</c> (key and modifiers).
/// </remarks>
public partial class ShowOnKeyDownBehavior : ShowBehaviorBase
{

    /// <summary>
    /// Gets or sets the key. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Key? Key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial KeyGesture? Gesture { get; set; }

    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable that removes the event handler.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        if (AssociatedObject is not { } element)
        {
            return DisposableAction.Empty;
        }

        element.AddHandler(InputElement.KeyDownEvent, AssociatedObject_KeyDown, EventRoutingStrategy);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(InputElement.KeyDownEvent, AssociatedObject_KeyDown));
#else
        var dispose = AssociatedObject?
            .AddDisposableRoutedEventHandler(
                InputElement.KeyDownEvent, 
                AssociatedObject_KeyDown, 
                EventRoutingStrategy);

        if (dispose is not null)
        {
            return dispose;
        }
        
        return DisposableAction.Empty;
#endif
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
