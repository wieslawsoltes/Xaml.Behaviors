using System;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A base class for behaviors using ResourcesChanged event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ResourcesChangedBehavior<T> : DisposingBehavior<T> where T : StyledElement
{
    private IDisposable? _disposable;

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        return new DisposableAction(OnDelayedDispose);
    }

    /// <inheritdoc />
    protected override void OnResourcesChangedEvent()
    {
        _disposable = OnResourcesChangedEventOverride();
    }

    /// <summary>
    /// Called when the <see cref="StyledElementBehavior{T}.AssociatedObject"/> ResourcesChanged event is raised.
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected abstract IDisposable OnResourcesChangedEventOverride();
  
    private void OnDelayedDispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
