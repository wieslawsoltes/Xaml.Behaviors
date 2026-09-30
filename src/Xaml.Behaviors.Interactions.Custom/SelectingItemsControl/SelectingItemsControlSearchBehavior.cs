// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
using SelectingItemsControl = Microsoft.UI.Xaml.Controls.TabView;
using TabItem = Microsoft.UI.Xaml.Controls.TabViewItem;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Filters <see cref="SelectingItemsControl"/> items based on the text of a search box.
/// </summary>
/// <remarks>
/// The behavior filters tab items by header. On Uno Platform it is attached to a WinUI <c>TabView</c> and filters
/// its <c>TabViewItem</c>s.
/// </remarks>
public sealed partial class SelectingItemsControlSearchBehavior : StyledElementBehavior<SelectingItemsControl>
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

#if UNO
    /// <summary>
    /// Gets or sets the control displayed when no matches are found.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? NoMatchesControl { get; set; }
#else
    /// <summary>
    /// Identifies the <seealso cref="NoMatchesControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBlock?> NoMatchesControlProperty =
        AvaloniaProperty.Register<SelectingItemsControlSearchBehavior, TextBlock?>(nameof(NoMatchesControl));
#endif

    /// <summary>
    /// Gets or sets the search box control.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial TextBox? SearchBox { get; set; }

#if !UNO
    /// <summary>
    /// Gets or sets the control displayed when no matches are found.
    /// </summary>
    [ResolveByName]
    public Control? NoMatchesControl
    {
        get => (Control?)GetValue(NoMatchesControlProperty);
        set => SetValue(NoMatchesControlProperty, value);
    }
#endif

    /// <summary>
    /// Gets or sets a value indicating whether items should be sorted.
    /// </summary>
    [StyledProperty]
    public partial bool EnableSorting { get; set; }

    /// <summary>
    /// Gets or sets the sort order for the items.
    /// </summary>
    [StyledProperty(DefaultValue = SortDirection.Ascending)]
    public partial SortDirection SortOrder { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (SearchBox is not null)
        {
#if UNO
            // WinUI raises TextChanged (a CLR event) after every edit, including text input.
            SearchBox.TextChanged += SearchBox_TextChanged;
#else
            SearchBox.AddHandler(InputElement.TextInputEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
            SearchBox.AddHandler(TextBox.TextChangedEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
#endif
        }

        SortItems();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (SearchBox is not null)
        {
#if UNO
            SearchBox.TextChanged -= SearchBox_TextChanged;
#else
            SearchBox.RemoveRoutedEventHandler(InputElement.TextInputEvent, SearchBox_TextChanged);
            SearchBox.RemoveRoutedEventHandler(TextBox.TextChangedEvent, SearchBox_TextChanged);
#endif
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
#if UNO
        SortTabItems(AssociatedObject.TabItems, tabItemComparer);
#else
        ArrayList.Adapter(AssociatedObject.Items).Sort(tabItemComparer);
#endif
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

#if UNO
        var tabItems = AssociatedObject.TabItems.OfType<TabItem>().ToList();
#else
        var tabItems = AssociatedObject.Items.OfType<TabItem>().ToList();
#endif

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
