// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif
using ReactiveUI;
using ReactiveUI.Reactive;
using Splat;

#if UNO
namespace Xaml.Interactions.ReactiveUI;
#else
namespace Avalonia.Xaml.Interactions.ReactiveUI;
#endif

/// <summary>
/// An action that resolves and navigates to <typeparamref name="TViewModel"/> and clears the navigation stack.
/// </summary>
/// <typeparam name="TViewModel">The view model type to navigate to.</typeparam>
public partial class NavigateToAndResetAction<TViewModel> 
    : StyledElementAction where TViewModel : class, IRoutableViewModel
{

    /// <summary>
    /// Gets or sets the router used for navigation. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial RoutingState? Router { get; set; }

    /// <summary>
    /// Resolves an instance of <typeparamref name="TViewModel"/> from the service locator.
    /// </summary>
    /// <returns>The resolved view model instance or <c>null</c> if it cannot be created.</returns>
    protected virtual TViewModel? ResolveViewModel()
    {
        return Locator.Current.GetService<TViewModel>();
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true || Router is null)
        {
            return false;
        }

        var vm = parameter as TViewModel ?? ResolveViewModel();
        if (vm is null)
        {
            return false;
        }

        Router.NavigateAndReset.Execute(vm).Subscribe();
        return true;
    }
}
