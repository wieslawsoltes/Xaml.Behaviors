// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using BehaviorsTestApplication.ViewModels;
using BehaviorsTestApplication.Views;
using Microsoft.UI.Xaml;
using ReactiveUI.Builder;

namespace BehaviorsTestApplication;

/// <summary>
/// The application and its composition root.
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
        var window = new Window { Title = "XAML Behaviors (Uno Platform)" };

        RxAppBuilder.CreateReactiveUIBuilder()
            .WithUno(window)
            .BuildApp();

        window.Content = new MainPage { DataContext = new MainViewModel() };
        window.Activate();
    }
}
