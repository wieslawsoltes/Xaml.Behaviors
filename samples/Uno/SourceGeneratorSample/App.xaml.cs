// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using ReactiveUI.Builder;
using ReactiveUI.Reactive.Builder;
using SourceGeneratorSample.ViewModels;
using SourceGeneratorSample.Views;

namespace SourceGeneratorSample;

/// <summary>
/// The application and its composition root (the Uno Platform twin of the Avalonia sample's App).
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
        var viewModel = new MainViewModel();
        var window = new MainWindow { ViewModel = viewModel };

        RxAppBuilder.CreateReactiveUIBuilder()
            .WithUno(window)
            .BuildApp();

        window.Activate();
    }
}
