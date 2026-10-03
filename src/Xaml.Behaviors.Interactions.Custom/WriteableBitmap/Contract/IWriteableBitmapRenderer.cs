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
/// Defines a method used to render into a <see cref="WriteableBitmap"/>.
/// </summary>
public interface IWriteableBitmapRenderer
{
    /// <summary>
    /// Renders into the provided <see cref="WriteableBitmap"/>.
    /// </summary>
    /// <param name="bitmap">The target bitmap.</param>
    void Render(WriteableBitmap bitmap);
}
