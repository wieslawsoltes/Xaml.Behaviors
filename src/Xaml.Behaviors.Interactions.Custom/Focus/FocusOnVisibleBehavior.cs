// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Xaml.Interactivity;
#else
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Focuses the associated control when it becomes visible.
/// </summary>
public class FocusOnVisibleBehavior : FocusBehaviorBase
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        if (AssociatedObject.IsVisible)
        {
            Focus();
        }

#if UNO
        var observable = AssociatedObject
            .GetObservable<Visibility>(UIElement.VisibilityProperty)
            .Subscribe(new AnonymousObserver<Visibility>(visibility =>
            {
                if (visibility == Visibility.Visible)
                {
                    Focus();
                }
            }));
#else
        var observable = AssociatedObject
            .GetObservable(Visual.IsVisibleProperty)
            .Subscribe(new AnonymousObserver<bool>(visible =>
            {
                if (visible)
                {
                    Focus();
                }
            }));
#endif

        return observable;
    }
}
