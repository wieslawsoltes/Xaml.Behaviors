using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that enables dropping context data onto the associated control.
/// </summary>
public class ContextDropBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the data format used to store context information.
    /// </summary>
    public static string DataFormat = nameof(Context);

    /// <summary>
    /// Identifies the <see cref="Context"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ContextProperty =
        AvaloniaProperty.Register<ContextDropBehavior, object?>(nameof(Context));

    /// <summary>
    /// Identifies the <see cref="Handler"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IDropHandler?> HandlerProperty =
        AvaloniaProperty.Register<ContextDropBehavior, IDropHandler?>(nameof(Handler));

    /// <summary>
    /// Gets or sets context data provided to the drop handler.
    /// </summary>
    public object? Context
    {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    /// <summary>
    /// Gets or sets the drop handler that receives drop notifications.
    /// </summary>
    public IDropHandler? Handler
    {
        get => GetValue(HandlerProperty);
        set => SetValue(HandlerProperty, value);
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

    private void DragEnter(object? sender, DragEventArgs e)
    {
        var sourceContext = e.Data.Contains(ContextDropBehavior.DataFormat) 
            ? e.Data.Get(ContextDropBehavior.DataFormat) 
            : null;
        var targetContext = Context ?? AssociatedObject?.DataContext;
        Handler?.Enter(sender, e, sourceContext, targetContext);
    }

    private void DragLeave(object? sender, RoutedEventArgs e)
    {
        Handler?.Leave(sender, e);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        var sourceContext = e.Data.Contains(ContextDropBehavior.DataFormat) 
            ? e.Data.Get(ContextDropBehavior.DataFormat) 
            : null;
        var targetContext = Context ?? AssociatedObject?.DataContext;
        Handler?.Over(sender, e, sourceContext, targetContext);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var sourceContext = e.Data.Contains(ContextDropBehavior.DataFormat) 
            ? e.Data.Get(ContextDropBehavior.DataFormat) 
            : null;
        var targetContext = Context ?? AssociatedObject?.DataContext;
        Handler?.Drop(sender, e, sourceContext, targetContext);
    }
}
