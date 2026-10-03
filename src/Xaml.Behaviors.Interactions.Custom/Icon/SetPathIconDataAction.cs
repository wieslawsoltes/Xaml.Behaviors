// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Changes the <see cref="PathIcon.Data"/> of a target icon when executed.
/// </summary>
public partial class SetPathIconDataAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the geometry to apply. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Geometry? Data { get; set; }

    /// <summary>
    /// Gets or sets the target icon. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial PathIcon? PathIcon { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = (PathIcon?)GetValue(PathIconProperty) ?? sender as PathIcon;
        if (target is null)
        {
            return false;
        }

        target.Data = Data;
        return true;
    }
}
