// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A base class for behaviors using attached to visual tree event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AttachedToVisualTreeBehavior<T> : DisposingBehavior<T> where T : Visual
{
	private IDisposable? _disposable;

    /// <inheritdoc />
	protected override IDisposable OnAttachedOverride()
	{
		return new DisposableAction(OnDelayedDispose);
	}

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        OnDelayedDispose();
        _disposable = OnAttachedToVisualTreeOverride();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        OnDelayedDispose();
    }

    /// <summary>
    /// Called after the <see cref="StyledElementBehavior{T}.AssociatedObject"/> is attached to the visual tree.
    /// </summary>
    /// <returns>A resource disposed when the associated object leaves the visual tree or the behavior is detached.</returns>
	protected abstract IDisposable OnAttachedToVisualTreeOverride();

    private void OnDelayedDispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
