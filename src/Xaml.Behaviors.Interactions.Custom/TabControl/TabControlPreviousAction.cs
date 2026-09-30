// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Moves the target <see cref="TabControl"/> to the previous tab.
/// </summary>
public partial class TabControlPreviousAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the tab control instance this action will operate on. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial TabControl? TabControl { get; set; }

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
