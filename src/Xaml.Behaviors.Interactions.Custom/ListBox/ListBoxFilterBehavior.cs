using System;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters <see cref="ListBox"/> items based on the text of a search box.
/// </summary>
public sealed class ListBoxFilterBehavior : StyledElementBehavior<ListBox>
{
    /// <summary>
    /// Identifies the <seealso cref="SearchBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> SearchBoxProperty =
        AvaloniaProperty.Register<ListBoxFilterBehavior, TextBox?>(nameof(SearchBox));

    /// <summary>
    /// Identifies the <seealso cref="NoMatchesControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> NoMatchesControlProperty =
        AvaloniaProperty.Register<ListBoxFilterBehavior, Control?>(nameof(NoMatchesControl));

    /// <summary>
    /// Identifies the <seealso cref="FilterMemberPath"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FilterMemberPathProperty =
        AvaloniaProperty.Register<ListBoxFilterBehavior, string?>(nameof(FilterMemberPath));

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
    /// Gets or sets the member path used for filtering items.
    /// </summary>
    public string? FilterMemberPath
    {
        get => GetValue(FilterMemberPathProperty);
        set => SetValue(FilterMemberPathProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (SearchBox is not null)
        {
            SearchBox.AddHandler(InputElement.TextInputEvent, OnTextChanged, RoutingStrategies.Bubble);
            SearchBox.AddHandler(TextBox.TextChangedEvent, OnTextChanged, RoutingStrategies.Bubble);
        }

        ApplyFilter();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (SearchBox is not null)
        {
            SearchBox.RemoveHandler(InputElement.TextInputEvent, OnTextChanged);
            SearchBox.RemoveHandler(TextBox.TextChangedEvent, OnTextChanged);
        }
    }

    private void OnTextChanged(object? sender, RoutedEventArgs e)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var query = SearchBox?.Text?.ToLowerInvariant() ?? string.Empty;
        var visibleCount = 0;
        foreach (var item in AssociatedObject.Items)
        {
            var container = AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
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

        AssociatedObject.SelectedItem = AssociatedObject.Items.Cast<object>()
            .FirstOrDefault(x =>
                (AssociatedObject.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem)?.IsVisible == true);

        if (NoMatchesControl is not null)
        {
            NoMatchesControl.IsVisible = visibleCount == 0;
        }
    }
}
