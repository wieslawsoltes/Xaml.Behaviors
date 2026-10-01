// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using BehaviorsTestApplication.ViewModels;
using Xaml.Interactions.Custom;
using Xaml.Interactions.ReactiveUI;

namespace BehaviorsTestApplication.Behaviors;

// WinUI XAML has no x:TypeArguments: the generic behaviors and actions used by the Avalonia sample with
// x:TypeArguments are closed here so the Uno views can use them.

/// <summary>
/// <see cref="ObservableTriggerBehavior{T}"/> for <see cref="int"/> values (Avalonia: <c>x:TypeArguments="x:Int32"</c>).
/// </summary>
public partial class Int32ObservableTriggerBehavior : ObservableTriggerBehavior<int>
{
}

/// <summary>
/// <see cref="NavigateToAction{TViewModel}"/> for <see cref="DetailPageViewModel"/>
/// (Avalonia: <c>x:TypeArguments="vm:DetailPageViewModel"</c>).
/// </summary>
public partial class NavigateToDetailPageAction : NavigateToAction<DetailPageViewModel>
{
}

/// <summary>
/// <see cref="NavigateToAndResetAction{TViewModel}"/> for <see cref="HomePageViewModel"/>
/// (Avalonia: <c>x:TypeArguments="vm:HomePageViewModel"</c>).
/// </summary>
public partial class NavigateToHomePageAndResetAction : NavigateToAndResetAction<HomePageViewModel>
{
}

/// <summary>
/// <see cref="NotNullValidationRule{T}"/> for <see cref="object"/> values (Avalonia: <c>x:TypeArguments="system:Object"</c>).
/// </summary>
public partial class ObjectNotNullValidationRule : NotNullValidationRule<object>
{
}
