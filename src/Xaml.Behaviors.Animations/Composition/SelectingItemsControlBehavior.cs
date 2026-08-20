// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

// Based on code: https://github.com/adirh3/Avalonia.ListBoxAnimation.Samples

using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Enables the standard selection indicator animation on selecting items controls.
/// </summary>
public class SelectingItemsControlBehavior
{
    /// <summary>
    /// Identifies the <see cref="GetEnableSelectionAnimation"/> attached property.
    /// </summary>
    public static readonly AttachedProperty<bool> EnableSelectionAnimationProperty =
        AvaloniaProperty.RegisterAttached<SelectingItemsControl, bool>("EnableSelectionAnimation",
            typeof(SelectingItemsControlBehavior));

    /// <summary>
    /// Initializes the attached-property change handler.
    /// </summary>
    static SelectingItemsControlBehavior()
    {
        EnableSelectionAnimationProperty.Changed.AddClassHandler<Control>(OnEnableSelectionAnimation);
    }

    private static void OnEnableSelectionAnimation(Control control, AvaloniaPropertyChangedEventArgs args)
    {
        if (control is SelectingItemsControl selectingItemsControl)
        {
            if (args.NewValue is true)
            {
                selectingItemsControl.PropertyChanged += SelectingItemsControlPropertyChanged;
            }
            else
            {
                selectingItemsControl.PropertyChanged -= SelectingItemsControlPropertyChanged;
            }
        }
    }

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

    /// <summary>
    /// Gets whether selection indicator animation is enabled for an element.
    /// </summary>
    /// <param name="element">The selecting items control.</param>
    /// <returns><c>true</c> when selection animation is enabled; otherwise, <c>false</c>.</returns>
    public static bool GetEnableSelectionAnimation(SelectingItemsControl element)
    {
        return element.GetValue(EnableSelectionAnimationProperty);
    }

    /// <summary>
    /// Sets whether selection indicator animation is enabled for an element.
    /// </summary>
    /// <param name="element">The selecting items control.</param>
    /// <param name="value">The value to set.</param>
    public static void SetEnableSelectionAnimation(SelectingItemsControl element, bool value)
    {
        element.SetValue(EnableSelectionAnimationProperty, value);
    }
}
