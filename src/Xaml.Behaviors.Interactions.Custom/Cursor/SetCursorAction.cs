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
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Sets the cursor on a target control.
/// </summary>
public partial class SetCursorAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial InputElement? TargetControl { get; set; }

    /// <summary>
    /// Gets or sets the cursor to apply.
    /// </summary>
    [StyledProperty]
    public partial Cursor? Cursor { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as InputElement;
        var cursor = Cursor;
        if (control is null || cursor is null)
        {
            return false;
        }

        control.SetCurrentValue(InputElement.CursorProperty, cursor);
        return true;
    }
}
