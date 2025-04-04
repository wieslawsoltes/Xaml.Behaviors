using System;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that executes when the associated object is attached to the logical tree.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AttachedToLogicalTreeTriggerBase<T>  : DisposingTrigger<T> where T : Visual
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
        _disposable = null;
    }
}
