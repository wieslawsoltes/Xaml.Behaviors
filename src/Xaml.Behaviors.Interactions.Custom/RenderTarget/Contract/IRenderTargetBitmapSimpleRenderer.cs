// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Media;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides a method used by <see cref="StaticRenderTargetBitmapBehavior"/> to draw onto a render target.
/// </summary>
public interface IRenderTargetBitmapSimpleRenderer
{
    /// <summary>
    /// Draws into the provided <see cref="DrawingContext"/>.
    /// </summary>
    /// <param name="context">The drawing context to use.</param>
    void Render(DrawingContext context);
}
