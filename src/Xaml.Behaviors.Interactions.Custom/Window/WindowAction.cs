// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Defines the window action types.
/// </summary>
public enum WindowActionType
{
    /// <summary>
    /// Closes the window.
    /// </summary>
    Close,

    /// <summary>
    /// Minimizes the window.
    /// </summary>
    Minimize,

    /// <summary>
    /// Maximizes the window.
    /// </summary>
    Maximize,

    /// <summary>
    /// Restores the window.
    /// </summary>
    Restore,

    /// <summary>
    /// Toggles full screen mode.
    /// </summary>
    ToggleFullScreen
}

/// <summary>
/// An action that performs common window operations.
/// </summary>
public class WindowAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="ActionType"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WindowActionType> ActionTypeProperty =
        AvaloniaProperty.Register<WindowAction, WindowActionType>(nameof(ActionType));

    /// <summary>
    /// Gets or sets the type of action to perform. This is an avalonia property.
    /// </summary>
    public WindowActionType ActionType
    {
        get => GetValue(ActionTypeProperty);
        set => SetValue(ActionTypeProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        var source = sender as Control;
        if (source is null)
        {
            return null;
        }

        var window = Window.GetTopLevel(source) as Window;
        if (window is null)
        {
            return null;
        }

        switch (ActionType)
        {
            case WindowActionType.Close:
                window.Close();
                break;
            case WindowActionType.Minimize:
                Dispatcher.UIThread.Post(() => window.WindowState = WindowState.Minimized);
                break;
            case WindowActionType.Maximize:
                Dispatcher.UIThread.Post(() => window.WindowState = WindowState.Maximized);
                break;
            case WindowActionType.Restore:
                Dispatcher.UIThread.Post(() => window.WindowState = WindowState.Normal);
                break;
            case WindowActionType.ToggleFullScreen:
                Dispatcher.UIThread.Post(() =>
                {
                    if (window.WindowState == WindowState.FullScreen)
                    {
                        window.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        window.WindowState = WindowState.FullScreen;
                    }
                });
                break;
        }

        return null;
    }
}
