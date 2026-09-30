// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using BehaviorsTestApplication.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace BehaviorsTestApplication.Views;

/// <summary>
/// The sample page.
/// </summary>
public sealed partial class MainPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets the view model for compiled bindings (x:Bind).
    /// </summary>
    public MainViewModel ViewModel => (MainViewModel)DataContext;
}
