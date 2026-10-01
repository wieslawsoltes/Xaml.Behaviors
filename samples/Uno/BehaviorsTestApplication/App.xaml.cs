// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using BehaviorsTestApplication.ViewModels;
using BehaviorsTestApplication.Views;
using Microsoft.UI.Xaml;
using ReactiveUI.Builder;
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

        RxAppBuilder.CreateReactiveUIBuilder()
            .WithUno(window)
            .BuildApp();

        // Avalonia: MainWindow { DataContext = new MainWindowViewModel() } hosting MainView. A WinUI window has no data
        // context, so the view model is the data context of MainView.
        window.Content = new MainView { DataContext = new MainWindowViewModel() };
        window.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1000, Height = 700 });
        window.Activate();
    }
}
