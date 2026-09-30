// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Behavior that hides a button's flyout when <see cref="IsFlyoutOpen"/> becomes false.
/// </summary>
public partial class ButtonHideFlyoutBehavior : DisposingBehavior<Button>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial bool IsFlyoutOpen { get; set; }

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
