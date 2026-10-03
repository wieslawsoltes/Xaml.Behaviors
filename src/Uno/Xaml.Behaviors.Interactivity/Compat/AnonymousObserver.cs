// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of <c>Avalonia.Reactive.AnonymousObserver&lt;T&gt;</c> used by the shared sources.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
internal sealed class AnonymousObserver<T> : IObserver<T>
{
    private readonly Action<T> _onNext;
    private readonly Action<Exception>? _onError;
    private readonly System.Action? _onCompleted;

    public AnonymousObserver(Action<T> onNext, Action<Exception>? onError = null, System.Action? onCompleted = null)
    {
        _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
        _onError = onError;
        _onCompleted = onCompleted;
    }

    public void OnNext(T value) => _onNext(value);

    public void OnError(Exception error) => _onError?.Invoke(error);

    public void OnCompleted() => _onCompleted?.Invoke();
}
