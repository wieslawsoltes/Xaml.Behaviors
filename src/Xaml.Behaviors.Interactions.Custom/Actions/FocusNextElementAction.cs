// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
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
        if (sender is not Control source)
        {
            return null;
        }

        var topLevel = TopLevel.GetTopLevel(source);
        var current = topLevel?.FocusManager?.GetFocusedElement() ?? source;
        if (topLevel is not null
            && FocusNavigationHelper.FindAdjacent(topLevel, current, NavigationDirection.Next, wrap: false) is { } next)
        {
            next.Focus(NavigationMethod.Tab, KeyModifiers.None);
        }

        return null;
    }
}
