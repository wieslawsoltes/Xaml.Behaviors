using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Allows a user to add the item to <see cref="ItemsControl"/>.
/// </summary>
public sealed class AddItemToItemsControlAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Item"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ItemProperty =
        AvaloniaProperty.Register<AddItemToItemsControlAction, object?>(nameof(Item));
    
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

        if (sender is not Control control)
        {
            return false;
        }

        var itemsControl = control.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault();
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
