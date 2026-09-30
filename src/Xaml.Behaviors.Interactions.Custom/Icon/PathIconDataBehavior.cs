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
/// Sets the <see cref="PathIcon.Data"/> when the associated icon is attached to the visual tree.
/// </summary>
public partial class PathIconDataBehavior : AttachedToVisualTreeBehavior<PathIcon>
{

    private Geometry? _oldData;

    /// <summary>
    /// Gets or sets the geometry used for the icon. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Geometry? Data { get; set; }

    /// <inheritdoc />
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        _oldData = AssociatedObject.Data;
        if (Data is not null)
        {
            AssociatedObject.Data = Data;
        }

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.Data = _oldData;
            }
        });
    }
}
