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
/// Sets the <see cref="ToolTip.TipProperty"/> of the associated or target control when executed.
/// </summary>
public partial class SetToolTipTipAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the control whose tooltip will be updated. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <summary>
    /// Gets or sets the new tooltip content. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Tip { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        if (control is null)
        {
            return false;
        }

        ToolTip.SetTip(control, Tip);
        return true;
    }
}
