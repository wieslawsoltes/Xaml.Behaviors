// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Custom;

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
