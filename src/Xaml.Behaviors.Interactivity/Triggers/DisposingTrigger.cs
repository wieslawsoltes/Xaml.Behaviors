// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A base class for triggers that require a disposable resource.
/// </summary>
/// <typeparam name="T">The object type to attach to</typeparam>
public abstract class DisposingTrigger<T> : StyledElementTrigger<T> where T : AvaloniaObject
{
    private IDisposable? _disposable;

    /// <summary>
    /// 
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        _disposable?.Dispose();
        _disposable = OnAttachedOverride();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected abstract IDisposable OnAttachedOverride();

    /// <summary>
    /// 
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        _disposable?.Dispose();
    }
}
