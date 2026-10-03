// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Tracks the windows of a WinUI 3 application, so behaviors can find the window that hosts an element.
/// </summary>
/// <remarks>
/// WinUI has no API to enumerate the windows of an application (Uno Platform has one). Behaviors and actions that work
/// with the window of their element (for example <c>CloseWindowAction</c>, <c>WindowStateTrigger</c> or the storage
/// and clipboard actions) find it among the tracked windows. Call <see cref="Track"/> for every window the application
/// creates, before showing it; a window is no longer tracked once it is closed.
/// </remarks>
public static class WindowTracker
{
    private static readonly List<Window> s_windows = [];

    /// <summary>
    /// Gets the tracked windows, in the order they were tracked.
    /// </summary>
    public static IReadOnlyList<Window> Windows => s_windows;

    /// <summary>
    /// Tracks a window until it is closed.
    /// </summary>
    /// <param name="window">The window.</param>
    public static void Track(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (s_windows.Contains(window))
        {
            return;
        }

        s_windows.Add(window);
        window.Closed += OnWindowClosed;
    }

    /// <summary>
    /// Stops tracking a window.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns><c>true</c> when the window was tracked.</returns>
    public static bool Untrack(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (!s_windows.Remove(window))
        {
            return false;
        }

        window.Closed -= OnWindowClosed;
        return true;
    }

    private static void OnWindowClosed(object sender, WindowEventArgs args)
    {
        if (sender is Window window)
        {
            Untrack(window);
        }
    }
}
