using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class HideAttachedFlyoutBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <returns></returns>
    protected override IDisposable OnAttachedOverride()
    {
       return this.GetObservable(IsFlyoutOpenProperty)
            .Subscribe(new AnonymousObserver<bool>(
                isOpen =>
                {
                    if (!isOpen && AssociatedObject is not null)
                    {
                        FlyoutBase.GetAttachedFlyout(AssociatedObject)?.Hide();
                    }
                }));
    }
}
