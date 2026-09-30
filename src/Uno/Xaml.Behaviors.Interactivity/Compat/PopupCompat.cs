// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>Popup.Open</c>/<c>Popup.Close</c> methods used by the shared sources.
/// </summary>
internal static class PopupCompatExtensions
{
    /// <summary>
    /// Opens the popup.
    /// </summary>
    /// <param name="popup">The popup.</param>
    public static void Open(this Popup popup) => popup.IsOpen = true;

    /// <summary>
    /// Closes the popup.
    /// </summary>
    /// <param name="popup">The popup.</param>
    public static void Close(this Popup popup) => popup.IsOpen = false;
}
