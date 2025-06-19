// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Action that detaches a <see cref="TabItem"/> from its parent <see cref="TabControl"/> and opens it in a new <see cref="Window"/>.
/// </summary>
public class OpenTabInWindowAction : AvaloniaObject, IAction
{
    /// <inheritdoc />
    public object? Execute(object? sender, object? parameter)
    {
        if (sender is not TabItem tabItem)
        {
            return false;
        }

        if (tabItem.Parent is TabControl tabControl)
        {
            if (tabControl.Items is IList list)
            {
                list.Remove(tabItem);
            }
        }

        var window = new Window
        {
            Width = 400,
            Height = 300
        };
        var host = new TabControl();
        host.Items.Add(tabItem);
        window.Content = host;
        window.Show();

        return true;
    }
}
