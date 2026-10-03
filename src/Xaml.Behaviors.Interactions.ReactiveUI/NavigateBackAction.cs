// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif
using ReactiveUI;
using ReactiveUI.Reactive;

#if UNO
namespace Xaml.Interactions.ReactiveUI;
#else
namespace Avalonia.Xaml.Interactions.ReactiveUI;
#endif

/// <summary>
/// An action that navigates back in the <see cref="RoutingState"/> stack.
/// </summary>
public partial class NavigateBackAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the router used for navigation. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial RoutingState? Router { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The sender that triggered the action.</param>
    /// <param name="parameter">Optional parameter for the action.</param>
    /// <returns>True if navigation back was requested.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true || Router is null)
        {
            return false;
        }

        if (Router.NavigationStack.Count == 0)
        {
            return false;
        }

        Router.NavigateBack.Execute().Subscribe();
        return true;
    }
}
