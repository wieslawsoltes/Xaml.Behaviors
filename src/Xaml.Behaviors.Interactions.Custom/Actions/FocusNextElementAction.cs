// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// An action that moves focus to the next element in the tab order.
/// </summary>
public class FocusNextElementAction : StyledElementAction
{
    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        var source = sender as Control;
        if (source is null)
        {
            return null;
        }

        var topLevel = TopLevel.GetTopLevel(source);
        if (topLevel is not null)
        {
            var focusManager = topLevel.FocusManager;
            if (focusManager is not null)
            {
                var current = focusManager.GetFocusedElement();
                if (current is not null)
                {
                    var next = KeyboardNavigationHandler.GetNext(current, NavigationDirection.Next);
                    if (next is not null)
                    {
                        Dispatcher.UIThread.Post(() => next.Focus());
                    }
                }
            }
        }

        return null;
    }
}
