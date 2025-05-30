using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Invokes actions whenever the <see cref="PathIcon.Data"/> property changes.
/// </summary>
public class PathIconDataChangedTrigger : DisposingTrigger<PathIcon>
{
    /// <inheritdoc />
    protected override System.IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        return AssociatedObject.GetObservable(PathIcon.DataProperty)
            .Subscribe(new AnonymousObserver<Geometry?>(_ => Execute(null)));
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
