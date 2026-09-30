// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform counterpart of <c>Avalonia.Reactive.AnonymousObserver&lt;T&gt;</c> used by the shared sources.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
internal sealed class AnonymousObserver<T>(Action<T> onNext) : IObserver<T>
{
    private readonly Action<T> _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));

    public void OnNext(T value) => _onNext(value);

    public void OnError(Exception error)
    {
    }

    public void OnCompleted()
    {
    }
}
