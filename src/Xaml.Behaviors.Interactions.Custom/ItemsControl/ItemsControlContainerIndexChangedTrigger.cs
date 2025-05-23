using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that listens for a <see cref="ItemsControl.ContainerIndexChanged"/> event on its source and executes its actions when that event is fired.
/// </summary>
public class ItemsControlContainerIndexChangedTrigger : StyledElementTrigger<ItemsControl>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.ContainerIndexChanged += ItemsControlOnContainerIndexChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.ContainerIndexChanged -= ItemsControlOnContainerIndexChanged;
        }
    }

    private void ItemsControlOnContainerIndexChanged(object? sender, ContainerIndexChangedEventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
        }
    }
}
