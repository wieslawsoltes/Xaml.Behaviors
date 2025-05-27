// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A base class for behaviors using ActualThemeVariantChanged event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ActualThemeVariantChangedBehavior<T> : DisposingBehavior<T> where T : StyledElement
{
    private IDisposable? _disposable;
    
    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        return new DisposableAction(OnDelayedDispose);
    }

    /// <inheritdoc />
    protected override void OnActualThemeVariantChangedEvent()
    {
        _disposable = OnActualThemeVariantChangedEventOverride();
    }

    /// <summary>
    /// Called when the <see cref="StyledElementBehavior{T}.AssociatedObject"/> ActualThemeVariantChanged event is raised.
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected abstract IDisposable OnActualThemeVariantChangedEventOverride();

    private void OnDelayedDispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
