using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Events;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Trigger that listens for the <see cref="DragDrop.DragOverEvent"/>.
/// </summary>
public class DragOverEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(DragDrop.DragOverEvent, OnDragOver, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        Execute(e);
    }
}
