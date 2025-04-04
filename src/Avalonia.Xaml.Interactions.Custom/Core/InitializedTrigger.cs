using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class InitializedTrigger : StyledElementTrigger<StyledElement>
{
    /// <inheritdoc />
    protected override void OnInitializedEvent()
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
