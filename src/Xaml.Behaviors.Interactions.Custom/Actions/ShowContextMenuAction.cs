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
/// Shows the <see cref="ContextMenu"/> of the associated or target control when executed.
/// </summary>
public partial class ShowContextMenuAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the control whose context menu will be shown. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The object that triggered the action.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <returns>True if a context menu was shown; otherwise false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        var contextMenu = control?.ContextMenu;
        if (control is null || contextMenu is null)
        {
            return false;
        }

        contextMenu.Open(control);
        return true;
    }
}
