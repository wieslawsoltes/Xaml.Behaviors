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

#if WINUI
            // Native WinUI raises the change of some properties (FrameworkElement.Transitions) before GetValue returns
            // the new value: a change that still reads the reported value is read again once the change is applied.
            Subscription subscription = new(owner, property);
            object? reported = owner.GetValue(property);
            subscription.Token = owner.RegisterPropertyChangedCallback(property, (_, _) =>
            {
                object? current = owner.GetValue(property);
                if (!ReferenceEquals(current, reported) || current is null)
                {
                    reported = current;
                    observer.OnNext(Read());
                    return;
                }

                owner.DispatcherQueue?.TryEnqueue(() =>
                {
                    object? applied = owner.GetValue(property);
                    if (!subscription.IsDisposed && !ReferenceEquals(applied, reported))
                    {
                        reported = applied;
                        observer.OnNext(Read());
                    }
                });
            });
            return subscription;
#else
            long token = owner.RegisterPropertyChangedCallback(property, (_, _) => observer.OnNext(Read()));
            return new Subscription(owner, property) { Token = token };
#endif
        }

        private T Read() => owner.GetValue(property) is T value ? value : default!;
    }

    private sealed class Subscription(DependencyObject owner, DependencyProperty property) : IDisposable
    {
        public long Token { get; set; }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            owner.UnregisterPropertyChangedCallback(property, Token);
        }
    }
}
