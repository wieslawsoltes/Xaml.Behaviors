using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Allows a user to remove the item from a <see cref="ListBox"/> ItemTemplate.
/// </summary>
public sealed class RemoveItemInListBoxAction : StyledElementAction
{
    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
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
            var data = control.DataContext;
            if (listItemsSource.Contains(data))
            {
                listItemsSource.Remove(data);
                return true;
            }
        }
        else if (itemsControl is ListBox listBox)
        {
            var listBoxItem = control.GetSelfAndLogicalAncestors().OfType<ListBoxItem>().FirstOrDefault();
            if (listBoxItem is not null)
            {
                if (listBox.Items is IList listItems && listItems.Contains(listBoxItem.DataContext))
                {
                    listItems.Remove(listBoxItem.DataContext);
                    return true;
                }
            }
        }

        return false;
    }
}
