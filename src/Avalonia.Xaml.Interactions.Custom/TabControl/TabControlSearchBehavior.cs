using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters <see cref="TabControl"/> items based on the text of a search box.
/// </summary>
public sealed class TabControlSearchBehavior : StyledElementBehavior<TabControl>
{
    /// <summary>
    /// Identifies the <seealso cref="SearchBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> SearchBoxProperty =
        AvaloniaProperty.Register<TabControlSearchBehavior, TextBox?>(nameof(SearchBox));

    /// <summary>
    /// Identifies the <seealso cref="NoMatchesText"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBlock?> NoMatchesTextProperty =
        AvaloniaProperty.Register<TabControlSearchBehavior, TextBlock?>(nameof(NoMatchesText));

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
    /// Gets or sets the text block displayed when no matches are found.
    /// </summary>
    [ResolveByName]
    public TextBlock? NoMatchesText
    {
        get => GetValue(NoMatchesTextProperty);
        set => SetValue(NoMatchesTextProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (SearchBox is not null)
        {
            SearchBox.AddHandler(InputElement.TextInputEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
            SearchBox.AddHandler(TextBox.TextChangedEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
        }
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

    private void SearchBox_TextChanged(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var query = SearchBox?.Text?.ToLowerInvariant() ?? string.Empty;
        var visibleCount = 0;
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

        if (NoMatchesText is not null)
        {
            NoMatchesText.IsVisible = visibleCount == 0;
        }
    }
}
