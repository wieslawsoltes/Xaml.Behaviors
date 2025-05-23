using System;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Avalonia.Xaml.Interactions.ReactiveUI;

/// <summary>
/// An action that navigates back in the <see cref="RoutingState"/> stack.
/// </summary>
public class NavigateBackAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Router"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutingState?> RouterProperty =
        AvaloniaProperty.Register<NavigateBackAction, RoutingState?>(nameof(Router));

    /// <summary>
    /// Gets or sets the router used for navigation. This is an avalonia property.
    /// </summary>
    public RoutingState? Router
    {
        get => GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

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
