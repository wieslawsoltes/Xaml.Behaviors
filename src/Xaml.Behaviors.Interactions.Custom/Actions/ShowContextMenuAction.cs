// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows the <see cref="ContextMenu"/> of the associated or target control when executed.
/// </summary>
public class ShowContextMenuAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ShowContextMenuAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets the control whose context menu will be shown. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

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
