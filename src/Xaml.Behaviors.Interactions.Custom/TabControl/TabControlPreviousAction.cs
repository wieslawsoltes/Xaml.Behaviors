// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Moves the target <see cref="TabControl"/> to the previous tab.
/// </summary>
public class TabControlPreviousAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TabControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TabControl?> TabControlProperty =
        AvaloniaProperty.Register<TabControlPreviousAction, TabControl?>(nameof(TabControl));

    /// <summary>
    /// Gets or sets the tab control instance this action will operate on. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public TabControl? TabControl
    {
        get => GetValue(TabControlProperty);
        set => SetValue(TabControlProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var tabControl = TabControl ?? sender as TabControl;
        if (tabControl is null)
        {
            return false;
        }

        if (tabControl.SelectedIndex > 0)
        {
            tabControl.SelectedIndex -= 1;
        }

        return null;
    }
}
