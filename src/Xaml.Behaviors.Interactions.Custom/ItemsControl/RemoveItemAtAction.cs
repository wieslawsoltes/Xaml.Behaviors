using System.Collections;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Removes an item at a specified index from an <see cref="ItemsControl"/>.
/// </summary>
public sealed class RemoveItemAtAction : AvaloniaObject, IAction
{
    /// <summary>
    /// Identifies the <see cref="ItemsControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ItemsControl?> ItemsControlProperty =
        AvaloniaProperty.Register<RemoveItemAtAction, ItemsControl?>(nameof(ItemsControl));

    /// <summary>
    /// Identifies the <see cref="Index"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> IndexProperty =
        AvaloniaProperty.Register<RemoveItemAtAction, int>(nameof(Index));

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
    /// Gets or sets index to remove.
    /// </summary>
    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    /// <inheritdoc />
    public object Execute(object? sender, object? parameter)
    {
        var itemsControl = ItemsControl ?? sender as ItemsControl;
        if (itemsControl?.ItemsSource is IList list && !list.IsReadOnly)
        {
            if (Index >= 0 && Index < list.Count)
            {
                list.RemoveAt(Index);
                return true;
            }
        }

        return false;
    }
}
