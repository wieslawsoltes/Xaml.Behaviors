// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Invokes an <see cref="IWriteableBitmapRenderer"/> to render into a bitmap.
/// </summary>
public partial class WriteableBitmapRenderAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the renderer used when executing the action. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IWriteableBitmapRenderer? Renderer { get; set; }

    /// <summary>
    /// Gets or sets the target bitmap. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial WriteableBitmap? Bitmap { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled || Renderer is null || Bitmap is null)
        {
            return false;
        }

        Renderer.Render(Bitmap);
        return true;
    }
}
