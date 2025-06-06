// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
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
