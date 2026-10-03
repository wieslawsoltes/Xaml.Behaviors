// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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

#if UNO
        // WinUI applies the x:Bind values of a view when it loads, and a view that is never shown (the content of a
        // tab that is not selected) never loads: the current collection is not reported for an element that is not
        // loaded, so that the actions do not run with unset values. Changes are always reported.
        var isCurrent = true;
        return TransitionOperations.Observe(
            AssociatedObject,
            _ =>
            {
                var skip = isCurrent && AssociatedObject is { } element && !LoadedState.IsLoaded(element);
                isCurrent = false;
                if (!skip)
                {
                    Dispatcher.UIThread.Post(() => Execute(null));
                }
            });
#else
        return TransitionOperations.Observe(
            AssociatedObject,
            _ =>
            {
                Dispatcher.UIThread.Post(() => Execute(null));
            });
#endif
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
