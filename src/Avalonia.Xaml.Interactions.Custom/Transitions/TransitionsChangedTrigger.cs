using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions whenever the <see cref="StyledElement.Transitions"/> property changes.
/// </summary>
public class TransitionsChangedTrigger : StyledElementTrigger
{
    /// <inheritdoc />
    protected override System.IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        return AssociatedObject.GetObservable(StyledElement.TransitionsProperty)
            .Subscribe(_ => Dispatcher.UIThread.Post(() => Execute(null)));
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null || !IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
