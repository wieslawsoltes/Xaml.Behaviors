using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

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
