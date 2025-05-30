using System.Collections;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.Collections;

/// <summary>
/// Clears all items from a target <see cref="IList"/> when invoked.
/// </summary>
public sealed class ClearCollectionAction : AvaloniaObject, IAction
{
    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IList?> TargetProperty =
        AvaloniaProperty.Register<ClearCollectionAction, IList?>(nameof(Target));

    /// <summary>
    /// Gets or sets the collection to clear.
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
        if (Target is not IList list)
        {
            return false;
        }

        list.Clear();
        return true;
    }
}
