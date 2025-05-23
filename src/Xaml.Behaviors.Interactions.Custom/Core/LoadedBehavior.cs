using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A base class for behaviors using loaded event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class LoadedBehavior<T> : DisposingBehavior<T> where T : Control
{
    private IDisposable? _disposable;

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        return new DisposableAction(OnDelayedDispose);
    }

    /// <inheritdoc />
    protected override void OnLoaded()
    {
        _disposable = OnLoadedOverride();
    }

    /// <summary>
    /// Called after the <see cref="StyledElementBehavior{T}.AssociatedObject"/> is loaded.
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected abstract IDisposable OnLoadedOverride();

    private void OnDelayedDispose()
    {
        _disposable?.Dispose();
    }
}
