// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters items of a <see cref="ComboBox"/> using text from the
/// associated <see cref="SearchBox"/> or the provided parameter.
/// </summary>
public sealed class FilterComboBoxItemsAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="ComboBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComboBox?> ComboBoxProperty =
        AvaloniaProperty.Register<FilterComboBoxItemsAction, ComboBox?>(nameof(ComboBox));

    /// <summary>
    /// Identifies the <seealso cref="SearchBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> SearchBoxProperty =
        AvaloniaProperty.Register<FilterComboBoxItemsAction, TextBox?>(nameof(SearchBox));

    /// <summary>
    /// Gets or sets the combo box to filter. If not set, the associated object is used.
    /// </summary>
    [ResolveByName]
    public ComboBox? ComboBox
    {
        get => GetValue(ComboBoxProperty);
        set => SetValue(ComboBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the search box providing filter text.
    /// </summary>
    [ResolveByName]
    public TextBox? SearchBox
    {
        get => GetValue(SearchBoxProperty);
        set => SetValue(SearchBoxProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var combo = GetValue(ComboBoxProperty);
        if (combo is null)
        {
            return false;
        }

        var query = parameter as string ?? SearchBox?.Text ?? string.Empty;
        query = query.ToLowerInvariant();

        var items = combo.Items.Cast<object>().ToList();
        var visibleCount = 0;

        foreach (var item in items)
        {
            var container = combo.ContainerFromItem(item) as ComboBoxItem;
            var text = item?.ToString()?.ToLowerInvariant() ?? string.Empty;
            var visible = text.Contains(query);
            if (container is not null)
            {
                container.IsVisible = visible;
            }
            if (visible)
            {
                visibleCount++;
            }
        }

        return visibleCount > 0;
    }
}
