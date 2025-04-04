using System.Reactive.Disposables;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A base class for behaviors using ActualThemeVariantChanged event.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ActualThemeVariantChangedBehavior<T> : DisposingBehavior<T> where T : StyledElement
{
    private CompositeDisposable? _disposables;

    /// <inheritdoc />
    protected override void OnAttached(CompositeDisposable disposables)
    {
        _disposables = disposables;
    }

    /// <inheritdoc />
    protected override void OnActualThemeVariantChangedEvent()
    {
        OnActualThemeVariantChangedEvent(_disposables!);
    }

    /// <summary>
    /// Called when the <see cref="StyledElementBehavior{T}.AssociatedObject"/> ActualThemeVariantChanged event is raised.
    /// </summary>
    /// <param name="disposable">The group of disposable resources that are disposed together</param>
    protected abstract void OnActualThemeVariantChangedEvent(CompositeDisposable disposable);
}
