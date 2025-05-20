using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="Control.Tag"/> of generated item containers to their data item.
/// </summary>
public class FluidMoveSetTagBehavior : ItemsControlContainerEventsBehavior
{
    /// <inheritdoc />
    protected override void OnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        if (e.Container is Control control)
        {
            control.Tag = e.Item;
        }
    }

    /// <inheritdoc />
    protected override void OnContainerClearing(object? sender, ContainerClearingEventArgs e)
    {
        if (e.Container is Control control)
        {
            control.ClearValue(Control.TagProperty);
        }
    }
}
