using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;
using Splat;

namespace Avalonia.Xaml.Interactions.ReactiveUI;

/// <summary>
/// An action that resolves and navigates to <typeparamref name="TViewModel"/> and clears the navigation stack.
/// </summary>
/// <typeparam name="TViewModel">The view model type to navigate to.</typeparam>
public class NavigateToAndResetAction<TViewModel> 
    : StyledElementAction where TViewModel : class, IRoutableViewModel
{
    /// <summary>
    /// Identifies the <seealso cref="Router"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutingState?> RouterProperty =
        AvaloniaProperty.Register<NavigateToAndResetAction<TViewModel>, RoutingState?>(nameof(Router));

    /// <summary>
    /// Gets or sets the router used for navigation. This is an avalonia property.
    /// </summary>
    public RoutingState? Router
    {
        get => GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

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
