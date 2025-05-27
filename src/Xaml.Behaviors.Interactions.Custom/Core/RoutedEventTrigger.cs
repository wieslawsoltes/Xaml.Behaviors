// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public abstract class RoutedEventTrigger : RoutedEventTriggerBase
{
    private IDisposable? _disposable;
    
    /// <summary>
    /// 
    /// </summary>
    protected abstract RoutedEvent RoutedEvent { get; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        return new DisposableAction(OnDelayedDispose);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is Interactive interactive)
        {
            var disposable = AddDisposableHandler(
                interactive,
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
    protected virtual void Handler(object? sender, RoutedEventArgs e)
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
    protected void Execute(RoutedEventArgs e)
    {
        e.Handled = MarkAsHandled;
        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }

    private void OnDelayedDispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    private static IDisposable AddDisposableHandler(
        Interactive o, 
        RoutedEvent routedEvent,
        EventHandler<RoutedEventArgs> handler,
        RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble,
        bool handledEventsToo = false)
    {
        o.AddHandler(routedEvent, handler, routes, handledEventsToo);

        return DisposableAction.Create(() => o.RemoveHandler(routedEvent, handler));
    }
}
