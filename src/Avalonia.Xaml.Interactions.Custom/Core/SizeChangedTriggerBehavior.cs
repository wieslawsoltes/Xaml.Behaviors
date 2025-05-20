using System;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that triggers its actions whenever the associated control size changes.
/// </summary>
public class SizeChangedTriggerBehavior : StyledElementTrigger<Control>
{
    private IDisposable? _disposable;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            _disposable = AssociatedObject
                .GetObservable(Visual.BoundsProperty)
                .Subscribe(new AnonymousObserver<Rect>(_ => Execute(parameter: null)));
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _disposable?.Dispose();
        _disposable = null;
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
