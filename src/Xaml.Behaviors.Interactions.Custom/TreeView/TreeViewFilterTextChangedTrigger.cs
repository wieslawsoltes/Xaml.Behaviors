// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the search box text changes.
/// </summary>
public sealed class TreeViewFilterTextChangedTrigger : InteractiveTriggerBase
{
    /// <summary>
    /// Identifies the <seealso cref="SearchBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> SearchBoxProperty =
        AvaloniaProperty.Register<TreeViewFilterTextChangedTrigger, TextBox?>(nameof(SearchBox));

    /// <summary>
    /// Gets or sets the search box control.
    /// </summary>
    [ResolveByName]
    public TextBox? SearchBox
    {
        get => GetValue(SearchBoxProperty);
        set => SetValue(SearchBoxProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        SearchBox?.AddHandler(TextBox.TextChangedEvent, OnTextChanged, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        SearchBox?.RemoveHandler(TextBox.TextChangedEvent, OnTextChanged);
    }

    private void OnTextChanged(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }
}
