using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Avalonia.Xaml.Interactions.ReactiveUI;

/// <summary>
/// An action that resets the navigation stack.
/// </summary>
public class ClearNavigationStackAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Router"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutingState?> RouterProperty =
        AvaloniaProperty.Register<NavigateAction, RoutingState?>(nameof(Router));

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
