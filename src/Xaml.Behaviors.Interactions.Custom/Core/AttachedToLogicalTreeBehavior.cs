using System;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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
