// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for <see cref="TextBox.TextChangedEvent"/> on a search box.
/// </summary>
public class ListBoxFilterTrigger : InteractiveTriggerBase
{
    /// <summary>
    /// Identifies the <see cref="ListBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ListBox?> ListBoxProperty =
        AvaloniaProperty.Register<ListBoxFilterTrigger, ListBox?>(nameof(ListBox));

    /// <summary>
    /// Gets or sets the list box associated with the trigger. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public ListBox? ListBox
    {
        get => GetValue(ListBoxProperty);
        set => SetValue(ListBoxProperty, value);
    }

    static ListBoxFilterTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<ListBoxFilterTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(TextBox.TextChangedEvent, OnTextChanged, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(TextBox.TextChangedEvent, OnTextChanged);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Execute(e);
    }
}
