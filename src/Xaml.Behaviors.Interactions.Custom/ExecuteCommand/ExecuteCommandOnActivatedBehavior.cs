// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            var mainWindow = SourceControl as Window ?? lifetime.MainWindow;

            if (mainWindow is not null)
            {
                mainWindow.Activated += WindowOnActivated;
                return DisposableAction.Create(() => mainWindow.Activated -= WindowOnActivated);
            }
        }
        
        return DisposableAction.Empty;
    }

    private void WindowOnActivated(object? sender, EventArgs e)
    {
        ExecuteCommand();
    }
}
