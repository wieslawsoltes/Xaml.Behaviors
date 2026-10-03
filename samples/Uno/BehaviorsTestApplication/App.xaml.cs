// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using BehaviorsTestApplication.ViewModels;
using BehaviorsTestApplication.Views;
using Microsoft.UI.Xaml;
#if !WINUI
using ReactiveUI.Builder;
#endif
using ReactiveUI.Reactive.Builder;

namespace BehaviorsTestApplication;

/// <summary>
/// The application and its composition root (Uno Platform counterpart of the Avalonia <c>App</c>).
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
        var window = new Window { Title = "XamlBehaviors Test Application" };

#if WINUI
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWinUI()
            .BuildApp();

        // WinUI cannot enumerate the windows of an application: the window behaviors find this one through the tracker.
        global::Xaml.Interactivity.WindowTracker.Track(window);
#else
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithUno(window)
            .BuildApp();
#endif

        // Avalonia: MainWindow { DataContext = new MainWindowViewModel() } hosting MainView. A WinUI window has no data
        // context, so the view model is the data context of MainView.
        var mainView = new MainView { DataContext = new MainWindowViewModel() };
        window.Content = mainView;
#if WINUI
        // AppWindow sizes are physical pixels on native WinUI: the size is scaled once the display scale is known.
        mainView.Loaded += (_, _) =>
        {
            var scale = mainView.XamlRoot?.RasterizationScale ?? 1d;
            window.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)(1000 * scale), Height = (int)(700 * scale) });
        };
#else
        window.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1000, Height = 700 });
#endif
        window.Activate();
    }
}
