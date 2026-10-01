// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using BehaviorsTestApplication.ViewModels;
using ReactiveUI.Uno;

namespace BehaviorsTestApplication.Views.Pages;

// WinUI XAML cannot use a generic root type (Avalonia: ReactiveUserControl<TViewModel> code-behind with a UserControl
// root). As recommended by ReactiveUI.Uno, the Uno views derive from these closed base classes, which are the roots of
// their .xaml twins.

/// <summary>
/// Base class of <see cref="HomePageView"/>.
/// </summary>
public partial class HomePageViewBase : ReactiveUserControl<HomePageViewModel>
{
}

/// <summary>
/// Base class of <see cref="DetailPageView"/>.
/// </summary>
public partial class DetailPageViewBase : ReactiveUserControl<DetailPageViewModel>
{
}

/// <summary>
/// Base class of <see cref="ReactiveNavigationView"/>.
/// </summary>
public partial class ReactiveNavigationViewBase : ReactiveUserControl<ReactiveNavigationViewModel>
{
}
