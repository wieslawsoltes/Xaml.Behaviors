// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Xaml.Interactivity;

namespace Xaml.Interactions.ReactiveUI;

/// <summary>
/// The parameterless <c>Subscribe</c> of System.Reactive used by the shared sources: ReactiveUI 25, which the Uno
/// Platform integration (ReactiveUI.Uno) requires, no longer depends on System.Reactive.
/// </summary>
internal static class ObservableCompatExtensions
{
    /// <summary>
    /// Subscribes to <paramref name="source"/> without observing its values (for example to run a command).
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The observable.</param>
    /// <returns>The subscription.</returns>
    public static IDisposable Subscribe<T>(this IObservable<T> source)
        => source.Subscribe(new AnonymousObserver<T>(static _ => { }));
}
