// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Avalonia <c>Window.WindowState</c> mapped to the presenter of the WinUI <see cref="AppWindow"/>.
/// </summary>
internal static class WindowStateExtensions
{
    extension(Window window)
    {
        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        public WindowState WindowState
        {
            get => GetState(window.AppWindow);
            set => SetState(window.AppWindow, value);
        }
    }

    /// <summary>
    /// Gets the state of the window presented by <paramref name="appWindow"/>.
    /// </summary>
    /// <param name="appWindow">The application window.</param>
    /// <returns>The window state.</returns>
    public static WindowState GetState(AppWindow? appWindow)
    {
        return appWindow?.Presenter switch
        {
            { Kind: AppWindowPresenterKind.FullScreen } => WindowState.FullScreen,
            OverlappedPresenter { State: OverlappedPresenterState.Minimized } => WindowState.Minimized,
            OverlappedPresenter { State: OverlappedPresenterState.Maximized } => WindowState.Maximized,
            _ => WindowState.Normal,
        };
    }

    private static void SetState(AppWindow? appWindow, WindowState state)
    {
        if (appWindow is null)
        {
            return;
        }

        if (state == WindowState.FullScreen)
        {
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            return;
        }

        if (appWindow.Presenter is not OverlappedPresenter)
        {
            appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
        }

        if (appWindow.Presenter is not OverlappedPresenter presenter)
        {
            return;
        }

        switch (state)
        {
            case WindowState.Minimized:
                presenter.Minimize();
                break;
            case WindowState.Maximized:
                presenter.Maximize();
                break;
            default:
                presenter.Restore();
                break;
        }
    }
}
