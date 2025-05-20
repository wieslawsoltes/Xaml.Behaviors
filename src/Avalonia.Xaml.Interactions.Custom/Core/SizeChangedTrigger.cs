using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that triggers its actions whenever the associated control size changes.
/// </summary>
public class SizeChangedTrigger : DisposingTrigger<Control>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;

        return DisposableAction.Create(() =>
        {
            AssociatedObject.SizeChanged -= AssociatedObjectOnSizeChanged;
        });

    }

    private void AssociatedObjectOnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Execute(parameter: e);
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
