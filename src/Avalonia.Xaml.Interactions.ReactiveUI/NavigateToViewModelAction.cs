using System;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Avalonia.Xaml.Interactions.ReractiveUI;

/// <summary>
/// An action that navigates to a specified <see cref="IRoutableViewModel"/>.
/// </summary>
public class NavigateToViewModelAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Router"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutingState?> RouterProperty =
        AvaloniaProperty.Register<NavigateToViewModelAction, RoutingState?>(nameof(Router));

    /// <summary>
    /// Identifies the <seealso cref="ViewModel"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IRoutableViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<NavigateToViewModelAction, IRoutableViewModel?>(nameof(ViewModel));

    /// <summary>
    /// Gets or sets the router used for navigation. This is an avalonia property.
    /// </summary>
    public RoutingState? Router
    {
        get => GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>
    /// Gets or sets the view model to navigate to. This is an avalonia property.
    /// </summary>
    public IRoutableViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

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
