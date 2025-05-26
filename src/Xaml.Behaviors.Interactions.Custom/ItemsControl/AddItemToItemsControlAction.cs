using System.Collections;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Allows a user to add the item to <see cref="ItemsControl"/>.
/// </summary>
public sealed class AddItemToItemsControlAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="ItemsControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ItemsControl?> ItemsControlProperty =
        AvaloniaProperty.Register<AddItemToItemsControlAction, ItemsControl?>(nameof(ItemsControl));
  
    /// <summary>
    /// Identifies the <see cref="Item"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ItemProperty =
        AvaloniaProperty.Register<AddItemToItemsControlAction, object?>(nameof(Item));
    
    /// <summary>
    /// Gets or sets items control.
    /// </summary>
    [ResolveByName]
    public ItemsControl? ItemsControl
    {
        get => GetValue(ItemsControlProperty);
        set => SetValue(ItemsControlProperty, value);
    }
    
    /// <summary>
    /// Gets or sets item to add.
    /// </summary>
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

        var item = Item;
        if (item is null)
        {
            return false;
        }

        var itemsControl = ItemsControl;
        if (itemsControl is null)
        {
            return false;
        }

        if (itemsControl.ItemsSource is IList listItemsSource && !listItemsSource.IsReadOnly)
        {
            listItemsSource.Add(item);
            return true;
        }

        return false;
    }
}
