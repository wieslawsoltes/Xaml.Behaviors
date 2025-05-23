using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Clears the <see cref="StyledElement.Transitions"/> collection.
/// </summary>
public class ClearTransitionsAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="StyledElement"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StyledElement?> StyledElementProperty =
        AvaloniaProperty.Register<ClearTransitionsAction, StyledElement?>(nameof(StyledElement));

    /// <summary>
    /// Gets or sets the target styled element. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public StyledElement? StyledElement
    {
        get => GetValue(StyledElementProperty);
        set => SetValue(StyledElementProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = GetValue(StyledElementProperty) ?? sender as StyledElement;
        if (target?.Transitions is null)
        {
            return false;
        }

        target.Transitions.Clear();
        return true;
    }
}
