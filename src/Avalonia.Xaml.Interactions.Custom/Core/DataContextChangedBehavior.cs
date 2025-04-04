using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A base class for behaviors using DataContextChanged event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class DataContextChangedBehavior<T> : DisposingBehavior<T> where T : StyledElement
{
    private CompositeDisposable? _disposables;

    /// <inheritdoc />
    protected override void OnAttached(CompositeDisposable disposables)
    {
        _disposables = disposables;
    }

    /// <inheritdoc />
    protected override void OnDataContextChangedEvent()
    {
        OnDataContextChangedEvent(_disposables!);
    }

    /// <summary>
    /// Called when the <see cref="StyledElementBehavior{T}.AssociatedObject"/> DataContextChanged event is raised.
    /// </summary>
    /// <param name="disposable">The group of disposable resources that are disposed together</param>
    protected abstract void OnDataContextChangedEvent(CompositeDisposable disposable);
}
