// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Hides the flyout attached to the control when <see cref="IsFlyoutOpen"/> is false.
/// </summary>
public class HideAttachedFlyoutBehavior : DisposingBehavior<Control>
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
