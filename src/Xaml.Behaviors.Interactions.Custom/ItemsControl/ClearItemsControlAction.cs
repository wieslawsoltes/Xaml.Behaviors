using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Allows a user to clear the items from a <see cref="ItemsControl"/>.
/// </summary>
public sealed class ClearItemsControlAction : StyledElementAction
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
            listItemsSource.Clear();
            return true;
        }

        return false;
    }
}
