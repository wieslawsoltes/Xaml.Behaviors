// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>TopLevel.GetTopLevel</c> lookup used by the shared sources.
/// </summary>
/// <remarks>
/// WinUI has no top level element type: the top level of an element is the <see cref="Window"/> whose content shares
/// the <see cref="XamlRoot"/> of the element. The open windows are enumerated through
/// <c>Uno.UI.ApplicationHelper.Windows</c>.
/// </remarks>
internal static class TopLevel
{
    /// <summary>
    /// Gets the window hosting the element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The window, or <c>null</c> when the element is not loaded in a window.</returns>
    public static Window? GetTopLevel(UIElement? element)
    {
        if (element?.XamlRoot is not { } root)
        {
            return null;
        }

        foreach (var window in Uno.UI.ApplicationHelper.Windows)
        {
            if (ReferenceEquals(window.Content?.XamlRoot, root))
            {
                return window;
            }
        }

        return null;
    }
}

/// <summary>
/// Makes the inherited Avalonia <c>Window.GetTopLevel</c> static method available on the WinUI <see cref="Window"/>.
/// </summary>
internal static class WindowTopLevelCompatExtensions
{
    extension(Window)
    {
        /// <summary>
        /// Gets the window hosting the element (Avalonia <c>TopLevel.GetTopLevel</c>).
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The window, or <c>null</c> when the element is not loaded in a window.</returns>
        public static Window? GetTopLevel(UIElement? element) => TopLevel.GetTopLevel(element);
    }
}
