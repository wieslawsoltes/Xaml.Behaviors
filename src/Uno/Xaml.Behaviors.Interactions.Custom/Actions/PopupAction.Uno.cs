// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform placement of the popup.
/// </content>
public partial class PopupAction
{
    /// <summary>
    /// Places the popup like Avalonia's <c>PlacementMode.Pointer</c>.
    /// </summary>
    /// <remarks>
    /// WinUI popups have no pointer placement: when the action is invoked by a pointer event the popup opens at the
    /// pointer position (window coordinates), otherwise it is placed next to the invoking element.
    /// </remarks>
    /// <param name="popup">The popup.</param>
    /// <param name="parent">The element invoking the action.</param>
    /// <param name="parameter">The action parameter (the event arguments of the trigger).</param>
    private static void PlacePopup(Popup popup, FrameworkElement? parent, object? parameter)
    {
        if (parent?.XamlRoot is { } xamlRoot)
        {
            popup.XamlRoot = xamlRoot;
        }

        if (parameter is PointerRoutedEventArgs pointer)
        {
            var position = pointer.GetCurrentPoint(null).Position;
            popup.PlacementTarget = null;
            popup.HorizontalOffset = position.X;
            popup.VerticalOffset = position.Y;
            return;
        }

        popup.PlacementTarget = parent;
        popup.HorizontalOffset = 0;
        popup.VerticalOffset = 0;
    }
}
