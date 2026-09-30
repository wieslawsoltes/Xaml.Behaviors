// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
/// An action that resets the navigation stack.
/// </summary>
public partial class ClearNavigationStackAction : StyledElementAction
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
    /// <returns>True if navigation was requested.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true || Router is null)
        {
            return false;
        }

        Router.NavigationStack.Clear();
        return true;
    }
}
