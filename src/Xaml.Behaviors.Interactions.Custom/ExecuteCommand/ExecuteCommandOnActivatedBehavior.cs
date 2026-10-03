// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
public class ExecuteCommandOnActivatedBehavior : ExecuteCommandBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        // WinUI windows are not elements: the main (first) window of the application is observed.
        if (Window.Current is { } mainWindow)
        {
            mainWindow.Activated += WindowOnActivated;
            return DisposableAction.Create(() => mainWindow.Activated -= WindowOnActivated);
        }
#else
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            var mainWindow = SourceControl as Window ?? lifetime.MainWindow;

            if (mainWindow is not null)
            {
                mainWindow.Activated += WindowOnActivated;
                return DisposableAction.Create(() => mainWindow.Activated -= WindowOnActivated);
            }
        }
#endif

        return DisposableAction.Empty;
    }

#if UNO
    private void WindowOnActivated(object sender, WindowActivatedEventArgs e)
    {
        // WinUI also raises Activated when the window is deactivated.
        if ((int)e.WindowActivationState != (int)WindowActivationState.Deactivated)
        {
            ExecuteCommand();
        }
    }
#else
    private void WindowOnActivated(object? sender, EventArgs e)
    {
        ExecuteCommand();
    }
#endif
}
