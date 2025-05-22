using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Allows a user to remove the item from a <see cref="ListBox"/> ItemTemplate.
/// </summary>
public sealed class RemoveItemInListBoxAction : StyledElementAction
{
    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (sender is not IControl control)
        {
            return false;
        }

        var itemsControl = control.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault();
        if (itemsControl is null)
        {
            return false;
        }

        if (itemsControl.ItemsSource is IList list && !list.IsReadOnly)
        {
            var data = control.DataContext;
            if (list.Contains(data))
            {
                list.Remove(data);
                return true;
            }
        }
        else if (itemsControl is ListBox listBox)
        {
            var listBoxItem = control.GetSelfAndLogicalAncestors().OfType<ListBoxItem>().FirstOrDefault();
            if (listBoxItem is not null)
            {
                if (listBox.Items is IList list && list.Contains(listBoxItem.DataContext))
                {
                    list.Remove(listBoxItem.DataContext);
                    return true;
                }
            }
        }

        return false;
    }
}
