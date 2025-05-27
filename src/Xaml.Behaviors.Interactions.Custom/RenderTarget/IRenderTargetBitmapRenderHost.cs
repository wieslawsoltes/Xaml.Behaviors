// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Media.Imaging;

namespace Avalonia.Xaml.Interactions.Custom;

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
