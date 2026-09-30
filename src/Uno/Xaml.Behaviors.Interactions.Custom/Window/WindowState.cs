// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Xaml.Interactions.Custom;

/// <summary>
/// Defines the states of a window (Uno Platform counterpart of the Avalonia <c>WindowState</c>).
/// </summary>
/// <remarks>
/// The state maps to the WinUI <c>AppWindow</c> presenter: an overlapped presenter state (restored, minimized or
/// maximized) or the full screen presenter.
/// </remarks>
public enum WindowState
{
    /// <summary>
    /// The window is neither minimized, maximized nor full screen.
    /// </summary>
    Normal,

    /// <summary>
    /// The window is minimized.
    /// </summary>
    Minimized,

    /// <summary>
    /// The window is maximized.
    /// </summary>
    Maximized,

    /// <summary>
    /// The window is full screen.
    /// </summary>
    FullScreen,
}
