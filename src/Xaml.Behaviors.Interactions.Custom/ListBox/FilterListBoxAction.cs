using System;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters items in a <see cref="ListBox"/> based on the provided text.
/// </summary>
public sealed class FilterListBoxAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="ListBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ListBox?> ListBoxProperty =
        AvaloniaProperty.Register<FilterListBoxAction, ListBox?>(nameof(ListBox));

    /// <summary>
    /// Identifies the <see cref="FilterText"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FilterTextProperty =
        AvaloniaProperty.Register<FilterListBoxAction, string?>(nameof(FilterText));

    /// <summary>
    /// Identifies the <see cref="FilterMemberPath"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FilterMemberPathProperty =
        AvaloniaProperty.Register<FilterListBoxAction, string?>(nameof(FilterMemberPath));

    /// <summary>
    /// Gets or sets the list box to filter. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public ListBox? ListBox
    {
        get => GetValue(ListBoxProperty);
        set => SetValue(ListBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the filter text. This is an avalonia property.
    /// </summary>
    public string? FilterText
    {
        get => GetValue(FilterTextProperty);
        set => SetValue(FilterTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the member path used for filtering items. This is an avalonia property.
    /// </summary>
    public string? FilterMemberPath
    {
        get => GetValue(FilterMemberPathProperty);
        set => SetValue(FilterMemberPathProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var listBox = GetValue(ListBoxProperty) is not null ? ListBox : sender as ListBox;
        if (listBox is null)
        {
            return false;
        }

        var query = FilterText?.ToLowerInvariant() ?? string.Empty;
        var visibleCount = 0;
        foreach (var item in listBox.Items)
        {
            var container = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (container is null)
            {
                continue;
            }

            var text = item?.ToString() ?? string.Empty;
            if (FilterMemberPath is not null && item is not null)
            {
                var prop = item.GetType().GetRuntimeProperty(FilterMemberPath);
                if (prop is not null)
                {
                    text = Convert.ToString(prop.GetValue(item)) ?? string.Empty;
                }
            }

            var visible = text.ToLowerInvariant().Contains(query);
            container.IsVisible = visible;
            if (visible)
            {
                visibleCount++;
            }
        }

        return visibleCount > 0;
    }
}
