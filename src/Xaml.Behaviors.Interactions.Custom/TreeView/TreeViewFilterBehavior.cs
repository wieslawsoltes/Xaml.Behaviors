// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters <see cref="TreeView"/> items based on the text of a search box.
/// </summary>
public sealed class TreeViewFilterBehavior : StyledElementBehavior<TreeView>
{
    /// <summary>
    /// Identifies the <seealso cref="SearchBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> SearchBoxProperty =
        AvaloniaProperty.Register<TreeViewFilterBehavior, TextBox?>(nameof(SearchBox));

    /// <summary>
    /// Identifies the <seealso cref="NoMatchesControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> NoMatchesControlProperty =
        AvaloniaProperty.Register<TreeViewFilterBehavior, Control?>(nameof(NoMatchesControl));

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

        var query = SearchBox?.Text ?? string.Empty;
        var count = TreeViewFilterHelper.Filter(AssociatedObject, query);

        if (NoMatchesControl is not null)
        {
            NoMatchesControl.IsVisible = count == 0;
        }
    }
}
