using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class DetachedFromVisualTreeTrigger : StyledElementTrigger<Visual>
{
    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
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
