// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform counterpart of <c>Avalonia.AvaloniaObjectExtensions.GetObservable</c> used by the shared sources.
/// </summary>
/// <remarks>
/// This assembly does not reference Xaml.Behaviors.Uno.Interactivity (whose compat layer provides the same helper),
/// so it carries its own minimal copy.
/// </remarks>
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

    private sealed class DependencyPropertyObservable<T>(DependencyObject owner, DependencyProperty property) : IObservable<T>
    {
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentNullException.ThrowIfNull(observer);
            observer.OnNext(Read());

            long token = owner.RegisterPropertyChangedCallback(property, (_, _) => observer.OnNext(Read()));
            return new Subscription(owner, property, token);
        }

        private T Read() => owner.GetValue(property) is T value ? value : default!;
    }

    private sealed class Subscription(DependencyObject owner, DependencyProperty property, long token) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            owner.UnregisterPropertyChangedCallback(property, token);
        }
    }
}
