// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Binds AssociatedObject object Tag property to root visual DataContext.
/// </summary>
public class BindTagToVisualRootDataContextBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable that clears the binding.</returns>
    protected override IDisposable OnAttachedOverride()
    {
        var visualRoot = (Control?)AssociatedObject?.GetVisualRoot();
        if (visualRoot is not null)
        {
            return BindDataContextToTag(visualRoot, AssociatedObject);
        }

        return DisposableAction.Empty;
    }

    private static IDisposable BindDataContextToTag(Control source, Control? target)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        return target.Bind(
            Control.TagProperty, 
            source.GetObservable(StyledElement.DataContextProperty));
    }
}
