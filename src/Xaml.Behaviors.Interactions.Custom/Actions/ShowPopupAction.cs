// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Shows a <see cref="Popup"/> when executed.
/// </summary>
public partial class ShowPopupAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the popup instance to show.
    /// </summary>
    [StyledProperty]
    public partial Popup? Popup { get; set; }

    /// <summary>
    /// Gets or sets the target control that hosts the popup. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        var popup = Popup;
        if (control is null || popup is null)
        {
            return false;
        }

        if (popup.PlacementTarget is null)
        {
            popup.PlacementTarget = control;
        }

        popup.Open();
        return true;
    }
}
