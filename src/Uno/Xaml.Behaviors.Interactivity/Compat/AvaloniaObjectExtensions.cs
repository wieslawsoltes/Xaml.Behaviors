// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the <c>Avalonia.AvaloniaObjectExtensions</c> helpers used by the shared sources.
/// </summary>
internal static class AvaloniaObjectExtensions
{
    /// <summary>
    /// Gets an observable that produces the current value of the property and then every change.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="o">The object.</param>
    /// <param name="property">The property.</param>
    /// <returns>The observable.</returns>
    public static IObservable<T> GetObservable<T>(this DependencyObject o, DependencyProperty property)
        => new DependencyPropertyObservable<T>(o, property);

    /// <summary>
    /// Gets an observable that produces the current value of the property and then every change.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="property">The property.</param>
    /// <returns>The observable.</returns>
    public static IObservable<object?> GetObservable(this DependencyObject o, DependencyProperty property)
        => new DependencyPropertyObservable<object?>(o, property);

    private sealed class DependencyPropertyObservable<T>(DependencyObject owner, DependencyProperty property) : IObservable<T>
    {
        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnNext(Read());

            var token = owner.RegisterPropertyChangedCallback(property, (_, _) => observer.OnNext(Read()));
            return DisposableAction.Create(() => owner.UnregisterPropertyChangedCallback(property, token));
        }

        private T Read() => owner.GetValue(property) is T value ? value : default!;
    }
}
