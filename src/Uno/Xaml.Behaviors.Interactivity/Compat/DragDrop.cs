// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>DragDrop</c> helpers used by the shared sources.
/// </summary>
internal static class DragDrop
{
    /// <summary>Gets the drag enter event.</summary>
    public static RoutedEvent DragEnterEvent => UIElement.DragEnterEvent;

    /// <summary>Gets the drag leave event.</summary>
    public static RoutedEvent DragLeaveEvent => UIElement.DragLeaveEvent;

    /// <summary>Gets the drag over event.</summary>
    public static RoutedEvent DragOverEvent => UIElement.DragOverEvent;

    /// <summary>Gets the drop event.</summary>
    public static RoutedEvent DropEvent => UIElement.DropEvent;

    /// <summary>Gets a value indicating whether the element accepts drops.</summary>
    public static bool GetAllowDrop(UIElement element) => element.AllowDrop;

    /// <summary>Sets a value indicating whether the element accepts drops.</summary>
    public static void SetAllowDrop(UIElement element, bool value) => element.AllowDrop = value;
}
