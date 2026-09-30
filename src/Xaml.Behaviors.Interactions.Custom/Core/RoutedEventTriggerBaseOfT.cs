// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Xaml.Interactivity;
#else
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class RoutedEventTriggerBase<T> : RoutedEventTriggerBase where T : RoutedEventArgs
{
    /// <summary>
    /// 
    /// </summary>
#if UNO
    // WinUI routed event identifiers are not generic; T is the argument type the event raises.
    protected abstract RoutedEvent RoutedEvent { get; }
#else
    protected abstract RoutedEvent<T> RoutedEvent { get; }
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is Interactive interactive)
        {
#if UNO
            var disposable = interactive.AddDisposableHandler<T>(
#else
            var disposable = interactive.AddDisposableHandler(
#endif
                RoutedEvent, 
                Handler, 
                EventRoutingStrategy);
            return disposable;
        }

        return DisposableAction.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Handler(object? sender, T e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Execute(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected void Execute(T e)
    {
        e.Handled = MarkAsHandled;
        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }

}
