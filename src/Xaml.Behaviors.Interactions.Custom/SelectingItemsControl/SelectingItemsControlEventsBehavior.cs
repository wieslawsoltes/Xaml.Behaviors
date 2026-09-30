// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Xaml.Interactivity;
using SelectingItemsControl = Microsoft.UI.Xaml.Controls.Primitives.Selector;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Base class for behaviors that handle the selection changes of a <see cref="SelectingItemsControl"/>
/// (Uno Platform: a WinUI <c>Selector</c>).
/// </summary>
public abstract class SelectingItemsControlEventsBehavior : DisposingBehavior<SelectingItemsControl>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is not { } selectingItemsControl)
        {
            return DisposableAction.Empty;
        }

        selectingItemsControl.SelectionChanged += SelectingItemsControlOnSelectionChanged;

        return DisposableAction.Create(
                () => selectingItemsControl.SelectionChanged -= SelectingItemsControlOnSelectionChanged);
    }

    private void SelectingItemsControlOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        OnSelectionChanged(sender, e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }
}
