// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnimationsTestApplication.Views;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace AnimationsTestApplication;

/// <summary>
/// The Uno Platform application and its composition root (twin of the Avalonia <c>App</c>).
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var window = new MainWindow();
        if (window.Content is FrameworkElement content)
        {
            content.Loaded += (_, _) => Resize(window, content);
        }

        window.Activate();
    }

    private static void Resize(Window window, FrameworkElement content)
    {
        // The Avalonia window is 1100x760 device independent pixels; WinUI windows are sized through their AppWindow
        // in physical pixels, so the size is scaled by the rasterization scale of the window's content.
        double scale = content.XamlRoot?.RasterizationScale ?? 1d;
        window.AppWindow.Resize(new SizeInt32 { Width = (int)(1100 * scale), Height = (int)(760 * scale) });
    }
}
