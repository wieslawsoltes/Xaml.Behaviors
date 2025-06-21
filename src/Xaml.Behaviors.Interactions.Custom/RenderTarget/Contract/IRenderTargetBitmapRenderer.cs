// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Media;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides a method used by <see cref="RenderTargetBitmapBehavior"/> to draw onto a render target.
/// </summary>
public interface IRenderTargetBitmapRenderer
{
    /// <summary>
    /// Draws into the provided <see cref="DrawingContext"/>.
    /// </summary>
    /// <param name="context">The drawing context to use.</param>
    /// <param name="elapsed">The elapsed time since the behavior was attached.</param>
    void Render(DrawingContext context, TimeSpan elapsed);
}
