using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.Collections;

/// <summary>
/// Adds a range of items to a target <see cref="IList"/> when invoked.
/// </summary>
public sealed class AddRangeAction : AvaloniaObject, IAction
{
    /// <summary>
    /// Identifies the <see cref="Items"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsProperty =
        AvaloniaProperty.Register<AddRangeAction, IEnumerable?>(nameof(Items));

    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IList?> TargetProperty =
        AvaloniaProperty.Register<AddRangeAction, IList?>(nameof(Target));

    /// <summary>
    /// Gets or sets the items to add.
    /// </summary>
    [Content]
    public IEnumerable? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection to add items to.
    /// </summary>
    [ResolveByName]
    public IList? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <inheritdoc />
    public object Execute(object? sender, object? parameter)
    {
        if (Target is not IList list || Items is null)
        {
            return false;
        }

        foreach (var item in Items)
        {
            list.Add(item);
        }

        return true;
    }
}
