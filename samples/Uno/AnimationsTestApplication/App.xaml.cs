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
        // The Avalonia window is 1100x760; WinUI windows are sized through their AppWindow.
        window.AppWindow.Resize(new SizeInt32 { Width = 1100, Height = 760 });
        window.Activate();
    }
}
