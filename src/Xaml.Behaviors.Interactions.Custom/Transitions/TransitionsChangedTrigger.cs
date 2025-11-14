// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions whenever the <see cref="Avalonia.Animation.Transitions"/> collection changes.
/// </summary>
public class TransitionsChangedTrigger : DisposingTrigger<Control>
{
    /// <inheritdoc />
    protected override System.IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        return AssociatedObject.GetObservable(StyledElement.TransitionsProperty)
            .Subscribe(new AnonymousObserver<Transitions?>(_ =>
            {
                Dispatcher.UIThread.Post(() => Execute(null));
            }));
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
