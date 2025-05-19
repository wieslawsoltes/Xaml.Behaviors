using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Action that switches the visual state of a control.
/// </summary>
public class GoToStateAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="StateName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> StateNameProperty =
        AvaloniaProperty.Register<GoToStateAction, string?>(nameof(StateName));

    /// <summary>
    /// Identifies the <seealso cref="UseTransitions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> UseTransitionsProperty =
        AvaloniaProperty.Register<GoToStateAction, bool>(nameof(UseTransitions), true);

    /// <summary>
    /// Gets or sets the state name to switch to.
    /// </summary>
    public string? StateName
    {
        get => GetValue(StateNameProperty);
        set => SetValue(StateNameProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether transitions should be used.
    /// </summary>
    public bool UseTransitions
    {
        get => GetValue(UseTransitionsProperty);
        set => SetValue(UseTransitionsProperty, value);
    }

    /// <inheritdoc />
    protected override void Invoke(object? parameter)
    {
        if (AssociatedObject is Control control && StateName is not null)
        {
            VisualStateManager.GoToState(control, StateName, UseTransitions);
        }
    }
}
