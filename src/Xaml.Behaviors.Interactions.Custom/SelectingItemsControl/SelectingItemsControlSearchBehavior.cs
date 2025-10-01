// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters <see cref="SelectingItemsControl"/> items based on the text of a search box.
/// </summary>
public sealed class SelectingItemsControlSearchBehavior : StyledElementBehavior<SelectingItemsControl>
{
    /// <summary>
    /// Sort order for tab items.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Sort items ascending.
        /// </summary>
        Ascending,

        /// <summary>
        /// Sort items descending.
        /// </summary>
        Descending
    }

    /// <summary>
    /// Identifies the <seealso cref="SearchBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> SearchBoxProperty =
        AvaloniaProperty.Register<SelectingItemsControlSearchBehavior, TextBox?>(nameof(SearchBox));

    /// <summary>
    /// Identifies the <seealso cref="NoMatchesControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBlock?> NoMatchesControlProperty =
        AvaloniaProperty.Register<SelectingItemsControlSearchBehavior, TextBlock?>(nameof(NoMatchesControl));

    /// <summary>
    /// Identifies the <seealso cref="EnableSorting"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableSortingProperty =
        AvaloniaProperty.Register<SelectingItemsControlSearchBehavior, bool>(nameof(EnableSorting));

    /// <summary>
    /// Identifies the <seealso cref="SortOrder"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<SortDirection> SortOrderProperty =
        AvaloniaProperty.Register<SelectingItemsControlSearchBehavior, SortDirection>(nameof(SortOrder), SortDirection.Ascending);

    /// <summary>
    /// Gets or sets the search box control.
    /// </summary>
    [ResolveByName]
    public TextBox? SearchBox
    {
        get => GetValue(SearchBoxProperty);
        set => SetValue(SearchBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the control displayed when no matches are found.
    /// </summary>
    [ResolveByName]
    public Control? NoMatchesControl
    {
        get => GetValue(NoMatchesControlProperty);
        set => SetValue(NoMatchesControlProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether items should be sorted.
    /// </summary>
    public bool EnableSorting
    {
        get => GetValue(EnableSortingProperty);
        set => SetValue(EnableSortingProperty, value);
    }

    /// <summary>
    /// Gets or sets the sort order for the items.
    /// </summary>
    public SortDirection SortOrder
    {
        get => GetValue(SortOrderProperty);
        set => SetValue(SortOrderProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (SearchBox is not null)
        {
            SearchBox.AddHandler(InputElement.TextInputEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
            SearchBox.AddHandler(TextBox.TextChangedEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
        }

        SortItems();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (SearchBox is not null)
        {
            SearchBox.RemoveHandler(InputElement.TextInputEvent, SearchBox_TextChanged);
            SearchBox.RemoveHandler(TextBox.TextChangedEvent, SearchBox_TextChanged);
        }
    }

    private void SortItems()
    {
        if (AssociatedObject is null || !EnableSorting)
        {
            return;
        }

        var tabItemComparer = SortOrder == SortDirection.Ascending
            ? Comparer<Object>.Create((x, y) => (x as TabItem)?.Header?.ToString()?.CompareTo((y as TabItem)?.Header?.ToString()) ?? -1)
            : Comparer<Object>.Create((x, y) => (y as TabItem)?.Header?.ToString()?.CompareTo((x as TabItem)?.Header?.ToString()) ?? -1);
        ArrayList.Adapter(AssociatedObject.Items).Sort(tabItemComparer);
    }

    private void SearchBox_TextChanged(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var query = SearchBox?.Text?.ToLowerInvariant() ?? string.Empty;
        var visibleCount = 0;

        SortItems();

        var tabItems = AssociatedObject.Items.OfType<TabItem>().ToList();

        foreach (var item in tabItems)
        {
            var header = item.Header?.ToString()?.ToLowerInvariant() ?? string.Empty;
            var visible = header.Contains(query);
            item.IsVisible = visible;
            if (visible)
            {
                visibleCount++;
            }
        }

        AssociatedObject.SelectedItem = tabItems.FirstOrDefault(x => x.IsVisible);

        if (NoMatchesControl is not null)
        {
            NoMatchesControl.IsVisible = visibleCount == 0;
        }
    }
}
