// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Avalonia.Media.Imaging;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Defines a render host that can update an underlying <see cref="RenderTargetBitmap"/>.
/// </summary>
public interface IRenderTargetBitmapRenderHost
{
    /// <summary>
    /// Requests that the render host renders its content to the bitmap.
    /// </summary>
    void Render();
}
