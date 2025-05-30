using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters <see cref="ComboBox"/> items based on the text of a search box.
/// </summary>
public sealed class ComboBoxSearchBehavior : StyledElementBehavior<ComboBox>
{
    /// <summary>
    /// Identifies the <seealso cref="SearchBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> SearchBoxProperty =
        AvaloniaProperty.Register<ComboBoxSearchBehavior, TextBox?>(nameof(SearchBox));

    /// <summary>
    /// Identifies the <seealso cref="NoMatchesControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> NoMatchesControlProperty =
        AvaloniaProperty.Register<ComboBoxSearchBehavior, Control?>(nameof(NoMatchesControl));

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
        var items = AssociatedObject.Items.Cast<object>().ToList();

        foreach (var item in items)
        {
            var container = AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) as ComboBoxItem;
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

        if (NoMatchesControl is not null)
        {
            NoMatchesControl.IsVisible = visibleCount == 0;
        }

        AssociatedObject.IsDropDownOpen = true;
    }
}
