// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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

        var observable = AssociatedObject
            .GetObservable(Visual.IsVisibleProperty)
            .Subscribe(new AnonymousObserver<bool>(visible =>
            {
                if (visible)
                {
                    Focus();
                }
            }));

        return observable;
    }
}
