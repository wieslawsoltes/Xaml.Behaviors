// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the parameterless Avalonia <c>InputElement.Focus()</c> used by the shared sources.
/// </summary>
internal static class FocusCompatExtensions
{
    /// <summary>
    /// Focuses the element programmatically (<see cref="FocusState.Programmatic"/>).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> when the element received focus.</returns>
    public static bool Focus(this UIElement element) => element.Focus(FocusState.Programmatic);
}
