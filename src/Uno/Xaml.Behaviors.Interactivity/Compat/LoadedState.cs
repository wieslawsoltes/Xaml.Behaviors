// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Tells whether an element is loaded (in a live tree, with its <c>Loaded</c> event raised).
/// </summary>
/// <remarks>
/// On native WinUI <see cref="FrameworkElement.IsLoaded"/> becomes <c>false</c>, for good, when the first
/// <c>Loaded</c> handler is added to an element that is already loaded, which is what attaching behaviors to a loaded
/// element does. The state is therefore captured before the handlers are added and then tracked with the
/// <c>Loaded</c> and <c>Unloaded</c> events. Uno Platform reports the state reliably.
/// </remarks>
internal static class LoadedState
{
#if WINUI
    // null: not tracked yet; otherwise the tracked state.
    private static readonly DependencyProperty IsLoadedProperty =
        DependencyProperty.RegisterAttached("IsLoaded", typeof(object), typeof(LoadedState), new PropertyMetadata(null));

    /// <summary>
    /// Gets a value indicating whether the element is loaded.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> when the element is loaded.</returns>
    public static bool IsLoaded(FrameworkElement element)
    {
        if (element.GetValue(IsLoadedProperty) is bool tracked)
        {
            return tracked;
        }

        // Read before the handlers are added (see the remarks).
        bool isLoaded = element.IsLoaded;
        element.SetValue(IsLoadedProperty, isLoaded);
        element.Loaded += OnLoaded;
        element.Unloaded += OnUnloaded;
        return isLoaded;
    }

    private static void OnLoaded(object sender, RoutedEventArgs e) => ((FrameworkElement)sender).SetValue(IsLoadedProperty, true);

    private static void OnUnloaded(object sender, RoutedEventArgs e) => ((FrameworkElement)sender).SetValue(IsLoadedProperty, false);
#else
    /// <summary>
    /// Gets a value indicating whether the element is loaded.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> when the element is loaded.</returns>
    public static bool IsLoaded(FrameworkElement element) => element.IsLoaded;
#endif
}
