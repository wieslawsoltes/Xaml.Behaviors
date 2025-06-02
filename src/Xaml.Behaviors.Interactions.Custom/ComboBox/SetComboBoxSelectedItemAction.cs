// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="ComboBox.SelectedItem"/> of the associated or target combo box.
/// </summary>
public class SetComboBoxSelectedItemAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="ComboBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComboBox?> ComboBoxProperty =
        AvaloniaProperty.Register<SetComboBoxSelectedItemAction, ComboBox?>(nameof(ComboBox));

    /// <summary>
    /// Identifies the <see cref="Item"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ItemProperty =
        AvaloniaProperty.Register<SetComboBoxSelectedItemAction, object?>(nameof(Item));

    /// <summary>
    /// Gets or sets the target combo box. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public ComboBox? ComboBox
    {
        get => GetValue(ComboBoxProperty);
        set => SetValue(ComboBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the item to select.
    /// </summary>
    [Content]
    public object? Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var combo = ComboBox ?? sender as ComboBox;
        if (combo is null || Item is null)
        {
            return false;
        }

        combo.SelectedItem = Item;
        return true;
    }
}
