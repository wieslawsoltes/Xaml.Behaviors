// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Runtime.ExceptionServices;

#if UNO
namespace Xaml.Interactions.ReactiveUI;
#else
namespace Avalonia.Xaml.Interactions.ReactiveUI;
#endif

/// <summary>
/// The parameterless <c>Subscribe</c> of System.Reactive used to run ReactiveUI commands: ReactiveUI 25 no longer
/// depends on System.Reactive.
/// </summary>
internal static class ObservableExtensions
{
    /// <summary>
    /// Subscribes to <paramref name="source"/> without observing its values (for example to run a command); errors are
    /// rethrown.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The observable.</param>
    /// <returns>The subscription.</returns>
    public static IDisposable Subscribe<T>(this IObservable<T> source)
        => source.Subscribe(IgnoringObserver<T>.Instance);

    private sealed class IgnoringObserver<T> : IObserver<T>
    {
        public static IgnoringObserver<T> Instance { get; } = new();

        public void OnNext(T value)
        {
        }

        // Like System.Reactive, an error without a handler is rethrown.
        public void OnError(Exception error) => ExceptionDispatchInfo.Throw(error);

        public void OnCompleted()
        {
        }
    }
}
