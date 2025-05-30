using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.Automation;

/// <summary>
/// Executes actions when <see cref="AutomationProperties.NameProperty"/> of the associated control changes.
/// </summary>
public class AutomationNameChangedTrigger : DisposingTrigger<Control>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        var subscription = AssociatedObject.GetObservable(AutomationProperties.NameProperty)
            .Subscribe(_ => Execute());

        return subscription;
    }

    private void Execute()
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter: null);
    }
}
