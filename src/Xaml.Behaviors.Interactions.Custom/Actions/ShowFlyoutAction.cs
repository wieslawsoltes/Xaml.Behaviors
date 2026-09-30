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
/// Shows a <see cref="FlyoutBase"/> when executed.
/// </summary>
public partial class ShowFlyoutAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the flyout instance to show. If not set, the attached flyout of the target control is used.
    /// </summary>
    [StyledProperty]
    public partial FlyoutBase? Flyout { get; set; }

    /// <summary>
    /// Gets or sets the target control that hosts the flyout. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
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

        var flyout = Flyout ?? FlyoutBase.GetAttachedFlyout(control);
        if (flyout is null)
        {
            return false;
        }

        flyout.ShowAt(control);

        return true;
    }
}
