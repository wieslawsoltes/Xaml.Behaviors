using System.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Moves an item within an <see cref="ItemsControl"/> from <see cref="FromIndex"/> to <see cref="ToIndex"/>.
/// </summary>
public sealed class MoveItemInItemsControlAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="ItemsControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ItemsControl?> ItemsControlProperty =
        AvaloniaProperty.Register<MoveItemInItemsControlAction, ItemsControl?>(nameof(ItemsControl));

    /// <summary>
    /// Identifies the <see cref="FromIndex"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> FromIndexProperty =
        AvaloniaProperty.Register<MoveItemInItemsControlAction, int>(nameof(FromIndex));

    /// <summary>
    /// Identifies the <see cref="ToIndex"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> ToIndexProperty =
        AvaloniaProperty.Register<MoveItemInItemsControlAction, int>(nameof(ToIndex));

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
    /// Gets or sets source index.
    /// </summary>
    public int FromIndex
    {
        get => GetValue(FromIndexProperty);
        set => SetValue(FromIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets target index.
    /// </summary>
    public int ToIndex
    {
        get => GetValue(ToIndexProperty);
        set => SetValue(ToIndexProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var itemsControl = ItemsControl;
        if (itemsControl is null)
        {
            if (sender is Control control)
            {
                itemsControl = control.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault();
            }
        }

        if (itemsControl?.ItemsSource is IList list && !list.IsReadOnly)
        {
            var targetIndex = ToIndex < 0 ? list.Count - 1 : ToIndex;

            if (FromIndex >= 0 && FromIndex < list.Count && targetIndex >= 0 && targetIndex < list.Count && FromIndex != targetIndex)
            {
                var item = list[FromIndex];
                list.RemoveAt(FromIndex);
                list.Insert(targetIndex, item);
                return true;
            }
        }

        return false;
    }
}
