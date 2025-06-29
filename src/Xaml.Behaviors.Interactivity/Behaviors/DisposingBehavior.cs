// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A base class for behaviors with disposable resources.
/// </summary>
/// <typeparam name="T">The object type to attach to</typeparam>
public abstract class DisposingBehavior<T> : StyledElementBehavior<T> where T : AvaloniaObject
{
    private IDisposable? _disposable;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        _disposable?.Dispose();
        _disposable = OnAttachedOverride();
    }

    /// <summary>
    /// Called after the behavior is attached to the <see cref="IBehavior.AssociatedObject"/>.
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected abstract IDisposable OnAttachedOverride();

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();

        _disposable?.Dispose();
        _disposable = null;
    }
}
