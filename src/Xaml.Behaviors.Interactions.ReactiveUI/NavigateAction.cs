// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif
using ReactiveUI;

#if UNO
namespace Xaml.Interactions.ReactiveUI;
#else
namespace Avalonia.Xaml.Interactions.ReactiveUI;
#endif

/// <summary>
/// An action that navigates to a specified <see cref="IRoutableViewModel"/>.
/// </summary>
public partial class NavigateAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the router used for navigation. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial RoutingState? Router { get; set; }

    /// <summary>
    /// Gets or sets the view model to navigate to. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IRoutableViewModel? ViewModel { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The sender that triggered the action.</param>
    /// <param name="parameter">Optional parameter for the action.</param>
    /// <returns>True if navigation was requested.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true || Router is null)
        {
            return false;
        }

        var vm = ViewModel ?? parameter as IRoutableViewModel;
        if (vm is null)
        {
            return false;
        }

        Router.Navigate.Execute(vm).Subscribe();
        return true;
    }
}
