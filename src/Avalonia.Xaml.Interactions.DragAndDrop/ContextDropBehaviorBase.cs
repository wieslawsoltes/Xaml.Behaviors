using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior base class that enables dropping context data onto the associated control.
/// </summary>
public abstract class ContextDropBehaviorBase : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the data format used to store context information.
    /// </summary>
    public static string DataFormat = nameof(Context);

    /// <summary>
    /// Identifies the <see cref="Context"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ContextProperty =
        AvaloniaProperty.Register<ContextDropBehaviorBase, object?>(nameof(Context));

    /// <summary>
    /// Gets or sets context data provided to the drop handler.
    /// </summary>
    public object? Context
    {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            DragDrop.SetAllowDrop(AssociatedObject, true);
        }
        AssociatedObject?.AddHandler(DragDrop.DragEnterEvent, DragEnter);
        AssociatedObject?.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        AssociatedObject?.AddHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject?.AddHandler(DragDrop.DropEvent, Drop);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            DragDrop.SetAllowDrop(AssociatedObject, false);
        }
        AssociatedObject?.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
        AssociatedObject?.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
        AssociatedObject?.RemoveHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject?.RemoveHandler(DragDrop.DropEvent, Drop);
    }

    /// <summary>
    /// Called when a drag enters the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    protected abstract void OnEnter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);
    
    /// <summary>
    /// Called when the drag leaves the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected abstract void OnLeave(object? sender, RoutedEventArgs e);

    /// <summary>
    /// Called repeatedly as the drag moves over the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    protected abstract void OnOver(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);

    /// <summary>
    /// Called when the drag leaves the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    protected abstract void OnDrop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);

    private void DragEnter(object? sender, DragEventArgs e)
    {
        var sourceContext = e.Data.Contains(ContextDropBehavior.DataFormat) 
            ? e.Data.Get(ContextDropBehavior.DataFormat) 
            : null;
        var targetContext = Context ?? AssociatedObject?.DataContext;
        OnEnter(sender, e, sourceContext, targetContext);
    }

    private void DragLeave(object? sender, RoutedEventArgs e)
    {
        OnLeave(sender, e);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        var sourceContext = e.Data.Contains(ContextDropBehavior.DataFormat) 
            ? e.Data.Get(ContextDropBehavior.DataFormat) 
            : null;
        var targetContext = Context ?? AssociatedObject?.DataContext;
        OnOver(sender, e, sourceContext, targetContext);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var sourceContext = e.Data.Contains(ContextDropBehavior.DataFormat) 
            ? e.Data.Get(ContextDropBehavior.DataFormat) 
            : null;
        var targetContext = Context ?? AssociatedObject?.DataContext;
        OnDrop(sender, e, sourceContext, targetContext);
    }
}
