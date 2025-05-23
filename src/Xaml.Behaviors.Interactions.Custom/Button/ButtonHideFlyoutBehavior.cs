using System;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that hides a button's flyout when <see cref="IsFlyoutOpen"/> becomes false.
/// </summary>
public class ButtonHideFlyoutBehavior : DisposingBehavior<Button>
{
    /// <summary>
    /// Gets or sets a value indicating whether the flyout is open.
    /// </summary>
    public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
        AvaloniaProperty.Register<ButtonHideFlyoutBehavior, bool>(nameof(IsFlyoutOpen));

    /// <summary>
    /// 
    /// </summary>
    public bool IsFlyoutOpen
    {
        get => GetValue(IsFlyoutOpenProperty);
        set => SetValue(IsFlyoutOpenProperty, value);
    }

    /// <summary>
    /// Subscribes to <see cref="IsFlyoutOpen"/> changes.
    /// </summary>
    /// <returns>A disposable that removes the subscription.</returns>
    protected override IDisposable OnAttachedOverride()
    {
        return this.GetObservable(IsFlyoutOpenProperty)
            .Subscribe(new AnonymousObserver<bool>(isOpen =>
            {
                if (!isOpen)
                {
                    AssociatedObject?.Flyout?.Hide();
                }
            }));
    }
}
