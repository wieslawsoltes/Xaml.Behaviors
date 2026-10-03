// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
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
/// A behavior that listens for a <see cref="Button.ClickEvent"/> event on its source and executes its actions when that event is fired.
/// </summary>
public partial class ButtonClickEventTriggerBehavior : StyledElementTrigger<Button>
{
    private KeyModifiers _savedKeyModifiers = KeyModifiers.None;

    /// <summary>
    /// Gets or sets the required key modifiers to execute <see cref="Button.ClickEvent"/> event handler. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial KeyModifiers KeyModifiers { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.Click += AssociatedObject_OnClick;
            AssociatedObject.AddHandler(InputElement.KeyDownEvent, Button_OnKeyDown, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.KeyUpEvent, Button_OnKeyUp, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.Click -= AssociatedObject_OnClick;
            AssociatedObject.RemoveRoutedEventHandler(InputElement.KeyDownEvent, Button_OnKeyDown);
            AssociatedObject.RemoveRoutedEventHandler(InputElement.KeyUpEvent, Button_OnKeyUp);
        }
    }

    private void AssociatedObject_OnClick(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (AssociatedObject is not null && KeyModifiers == _savedKeyModifiers)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
        }
    }

    private void Button_OnKeyDown(object? sender, KeyEventArgs e)
    {
        _savedKeyModifiers = e.KeyModifiers;
    }

    private void Button_OnKeyUp(object? sender, KeyEventArgs e)
    {
        _savedKeyModifiers = KeyModifiers.None;
    }
}
