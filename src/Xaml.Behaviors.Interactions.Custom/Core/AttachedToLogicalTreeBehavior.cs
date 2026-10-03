// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A base class for behaviors using attached to logical tree event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AttachedToLogicalTreeBehavior<T> : DisposingBehavior<T> where T : StyledElement
{
    private IDisposable? _disposable;

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        return new DisposableAction(OnDelayedDispose);
    }

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree()
    {
        _disposable = OnAttachedToLogicalTreeOverride();
    }

    /// <summary>
    /// Called after the <see cref="StyledElementBehavior{T}.AssociatedObject"/> is attached to the logical tree.
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected abstract IDisposable OnAttachedToLogicalTreeOverride();

    private void OnDelayedDispose()
    {
        _disposable?.Dispose();
    }
}
