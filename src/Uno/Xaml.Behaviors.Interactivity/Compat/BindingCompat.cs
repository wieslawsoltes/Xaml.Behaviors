// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>AvaloniaObject.Bind</c> overloads used by the shared sources.
/// </summary>
internal static class BindingCompatExtensions
{
    /// <summary>
    /// Applies a binding to a property and returns a disposable that clears it.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <param name="property">The target property.</param>
    /// <param name="binding">The binding.</param>
    /// <returns>A disposable that removes the binding (clears the local value).</returns>
    public static IDisposable Bind(this DependencyObject target, DependencyProperty property, BindingBase binding)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(binding);

        BindingOperations.SetBinding(target, property, binding);
        return DisposableAction.Create(() => target.ClearValue(property));
    }

    /// <summary>
    /// Pushes the values produced by an observable to a property and returns a disposable that stops it.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="target">The target object.</param>
    /// <param name="property">The target property.</param>
    /// <param name="source">The source of values.</param>
    /// <returns>A disposable that ends the subscription and clears the local value.</returns>
    public static IDisposable Bind<T>(this DependencyObject target, DependencyProperty property, IObservable<T> source)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(source);

        var subscription = source.Subscribe(new AnonymousObserver<T>(value => target.SetValue(property, value)));
        return DisposableAction.Create(() =>
        {
            subscription.Dispose();
            target.ClearValue(property);
        });
    }
}
