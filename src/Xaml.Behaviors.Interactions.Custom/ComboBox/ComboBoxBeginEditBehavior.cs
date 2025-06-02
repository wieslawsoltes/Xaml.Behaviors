// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Opens the associated <see cref="ComboBox"/> for editing when triggered by a key or double tap.
/// </summary>
public class ComboBoxBeginEditBehavior : StyledElementBehavior<ComboBox>
{
    /// <summary>
    /// Identifies the <see cref="EditKey"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Key> EditKeyProperty =
        AvaloniaProperty.Register<ComboBoxBeginEditBehavior, Key>(nameof(EditKey), Key.F2);

    /// <summary>
    /// Gets or sets the key used to start editing.
    /// </summary>
    public Key EditKey
    {
        get => GetValue(EditKeyProperty);
        set => SetValue(EditKeyProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.DoubleTappedEvent, OnDoubleTapped, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            AssociatedObject.RemoveHandler(InputElement.DoubleTappedEvent, OnDoubleTapped);
        }
    }

    private void OnDoubleTapped(object? sender, RoutedEventArgs e) => BeginEdit();

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == EditKey)
        {
            BeginEdit();
            e.Handled = true;
        }
    }

    private void BeginEdit()
    {
        if (AssociatedObject is null)
            return;

        AssociatedObject.IsDropDownOpen = true;

        if (AssociatedObject.IsEditable)
        {
            var textBox = AssociatedObject.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
            textBox?.Focus();
            textBox?.SelectAll();
        }
    }
}
