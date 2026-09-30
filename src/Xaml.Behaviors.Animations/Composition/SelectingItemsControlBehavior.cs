// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

// Based on code: https://github.com/adirh3/Avalonia.ListBoxAnimation.Samples

#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SelectingItemsControl = Microsoft.UI.Xaml.Controls.Primitives.Selector;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Enables the standard selection indicator animation on selecting items controls.
/// </summary>
[AttachedProperty("EnableSelectionAnimation", typeof(bool), HostType = typeof(SelectingItemsControl))]
public partial class SelectingItemsControlBehavior
{
    static partial void OnEnableSelectionAnimationChanged(SelectingItemsControl element, bool oldValue, bool newValue)
    {
#if UNO
        element.SelectionChanged -= SelectingItemsControlSelectionChanged;
        if (newValue)
        {
            element.SelectionChanged += SelectingItemsControlSelectionChanged;
        }
#else
        if (newValue)
        {
            element.PropertyChanged += SelectingItemsControlPropertyChanged;
        }
        else
        {
            element.PropertyChanged -= SelectingItemsControlPropertyChanged;
        }
#endif
    }

#if UNO
    private static void SelectingItemsControlSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        if (sender is not SelectingItemsControl selectingItemsControl ||
            args.AddedItems.Count == 0 || args.RemovedItems.Count == 0)
        {
            return;
        }

        if (selectingItemsControl.ContainerFromItem(args.AddedItems[0]) is not TemplatedControl newSelection
            || selectingItemsControl.ContainerFromItem(args.RemovedItems[0]) is not TemplatedControl oldSelection)
        {
            return;
        }

        SelectionIndicatorAnimation.TryStart(
            newSelection,
            oldSelection,
            SelectionIndicatorAnimation.DefaultDuration);
    }
#else
    private static void SelectingItemsControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (sender is not SelectingItemsControl selectingItemsControl ||
            args.Property != SelectingItemsControl.SelectedIndexProperty ||
            args.OldValue is not int oldIndex || args.NewValue is not int newIndex)
        {
            return;
        }

        if (selectingItemsControl.ContainerFromIndex(newIndex) is not TemplatedControl newSelection
            || selectingItemsControl.ContainerFromIndex(oldIndex) is not TemplatedControl oldSelection)
        {
            return;
        }

        SelectionIndicatorAnimation.TryStart(
            newSelection,
            oldSelection,
            SelectionIndicatorAnimation.DefaultDuration);
    }
#endif
}
