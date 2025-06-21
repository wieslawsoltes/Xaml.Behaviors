// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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
            .Subscribe(new AnonymousObserver<string?>(_ => Execute()));

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
