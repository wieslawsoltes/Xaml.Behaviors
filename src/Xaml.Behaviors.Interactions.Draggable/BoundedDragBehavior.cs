using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Base class for drag behaviors that support constraining movement to a bounding container.
/// </summary>
public abstract class BoundedDragBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="BoundingContainer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> BoundingContainerProperty =
        AvaloniaProperty.Register<BoundedDragBehavior, Control?>(nameof(BoundingContainer));

    /// <summary>
    /// Gets or sets the control that defines the draggable area bounds.
    /// </summary>
    [ResolveByName]
    public Control? BoundingContainer
    {
        get => GetValue(BoundingContainerProperty);
        set => SetValue(BoundingContainerProperty, value);
    }
}
