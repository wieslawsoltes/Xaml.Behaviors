// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Provides a custom cursor instance.
/// </summary>
public interface ICursorProvider
{
    /// <summary>
    /// Creates a cursor instance.
    /// </summary>
    /// <returns>The created <see cref="Cursor"/>.</returns>
    Cursor CreateCursor();
}
