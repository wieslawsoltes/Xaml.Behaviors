using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that executes when the associated object is attached to the visual tree.
/// </summary>
public class AttachedToVisualTreeTrigger : StyledElementTrigger<Visual>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        Execute(parameter: null);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
