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
/// Action that invokes <see cref="IRenderTargetBitmapRenderHost.Render"/> on the specified target.
/// </summary>
public partial class RenderRenderTargetBitmapAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the render host. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial IRenderTargetBitmapRenderHost? Target { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true)
        {
            return false;
        }

        Target?.Render();
        return true;
    }
}
