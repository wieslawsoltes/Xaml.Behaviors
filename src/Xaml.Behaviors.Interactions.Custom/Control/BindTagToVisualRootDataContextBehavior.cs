// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Binds AssociatedObject object Tag property to root visual DataContext.
/// </summary>
public class BindTagToVisualRootDataContextBehavior : DisposingBehavior<Control>
{
    private IDisposable? _binding;

    /// <summary>
    /// Called when the behavior is attached: binds the tag when the associated object already has a visual root.
    /// </summary>
    /// <returns>A disposable that clears the binding.</returns>
    protected override IDisposable OnAttachedOverride()
    {
        BindToVisualRoot();
        return DisposableAction.Create(ClearBinding);
    }

    /// <summary>
    /// Binds the tag once the associated object is attached to the visual tree: behaviors declared in XAML are
    /// attached before their element has a visual root.
    /// </summary>
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        BindToVisualRoot();
    }

    private void BindToVisualRoot()
    {
        if (_binding is not null || AssociatedObject is null)
        {
            return;
        }

#if UNO
        // WinUI: the root element of the XAML island hosting the control.
        var visualRoot = AssociatedObject.XamlRoot?.Content as StyledElement;
#else
        var visualRoot = TopLevel.GetTopLevel(AssociatedObject);
#endif
        if (visualRoot is not null)
        {
            _binding = BindDataContextToTag(visualRoot, AssociatedObject);
        }
    }

    private void ClearBinding()
    {
        _binding?.Dispose();
        _binding = null;
    }

    private static IDisposable BindDataContextToTag(StyledElement source, Control? target)
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
