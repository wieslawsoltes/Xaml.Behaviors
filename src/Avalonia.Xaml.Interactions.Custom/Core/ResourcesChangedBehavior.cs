using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A base class for behaviors using ResourcesChanged event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ResourcesChangedBehavior<T> : DisposingBehavior<T> where T : StyledElement
{
    private CompositeDisposable? _disposables;

    /// <inheritdoc />
    protected override void OnAttached(CompositeDisposable disposables)
    {
        _disposables = disposables;
    }

    /// <inheritdoc />
    protected override void OnResourcesChangedEvent()
    {
        OnResourcesChangedEvent(_disposables!);
    }

    /// <summary>
    /// Called when the <see cref="StyledElementBehavior{T}.AssociatedObject"/> ResourcesChanged event is raised.
    /// </summary>
    /// <param name="disposable">The group of disposable resources that are disposed together</param>
    protected abstract void OnResourcesChangedEvent(CompositeDisposable disposable);
}
