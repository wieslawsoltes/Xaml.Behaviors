// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Xaml.Interactivity;

/// <summary>
/// Registers a handler for an event of any delegate type without reflection.
/// </summary>
/// <remarks>
/// WinUI events use dedicated delegate types (for example <c>RoutedEventHandler</c> or
/// <c>TypedEventHandler&lt;TSender, TArgs&gt;</c>), so the handler delegate is created by <paramref name="create"/>.
/// </remarks>
/// <param name="name">The event name.</param>
/// <param name="create">Creates the event delegate that forwards to the trigger callback.</param>
/// <param name="add">Adds the delegate to the event.</param>
/// <param name="remove">Removes the delegate from the event.</param>
/// <typeparam name="TTarget">The event source type.</typeparam>
/// <typeparam name="THandler">The event delegate type.</typeparam>
public sealed class DelegateAddEventHandler<TTarget, THandler>(
    string name,
    Func<Action<object?, object>, THandler> create,
    Action<TTarget, THandler> add,
    Action<TTarget, THandler> remove)
    : IAddEventHandler
    where TTarget : class
    where THandler : Delegate
{
    /// <inheritdoc/>
    public bool Matches(object source, string eventName)
        => source is TTarget && eventName == name;

    /// <inheritdoc/>
    public IDisposable? AddHandler(object source, string eventName, Action<object?, object> handler)
    {
        if (source is not TTarget target || eventName != name)
        {
            return null;
        }

        var eventHandler = create(handler);
        add(target, eventHandler);
        return DisposableAction.Create(() => remove(target, eventHandler));
    }
}
