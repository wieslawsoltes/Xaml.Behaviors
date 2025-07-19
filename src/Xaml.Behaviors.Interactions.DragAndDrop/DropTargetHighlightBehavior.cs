using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Highlights the drop target while a drag is over the control.
/// </summary>
public class DropTargetHighlightBehavior : DragAndDropEventsBehavior
{
    /// <summary>
    /// Identifies the <seealso cref="ClassName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ClassNameProperty =
        AvaloniaProperty.Register<DropTargetHighlightBehavior, string?>(nameof(ClassName));

    /// <summary>
    /// Identifies the <seealso cref="IsPseudoClass"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPseudoClassProperty =
        AvaloniaProperty.Register<DropTargetHighlightBehavior, bool>(nameof(IsPseudoClass));

    /// <summary>
    /// Identifies the <seealso cref="IsHighlighted"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsHighlightedProperty =
        AvaloniaProperty.Register<DropTargetHighlightBehavior, bool>(nameof(IsHighlighted));

    /// <summary>
    /// Gets or sets the CSS class or pseudo class name that is toggled when the drop target is highlighted.
    /// This is an avalonia property.
    /// </summary>
    [Content]
    public string? ClassName
    {
        get => GetValue(ClassNameProperty);
        set => SetValue(ClassNameProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="ClassName"/> is a pseudo class. This is an avalonia property.
    /// </summary>
    public bool IsPseudoClass
    {
        get => GetValue(IsPseudoClassProperty);
        set => SetValue(IsPseudoClassProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the target control is highlighted.
    /// This is an avalonia property.
    /// </summary>
    public bool IsHighlighted
    {
        get => GetValue(IsHighlightedProperty);
        set => SetValue(IsHighlightedProperty, value);
    }

    /// <inheritdoc />
    protected override void OnDragEnter(object? sender, DragEventArgs e)
    {
        SetHighlight(true);
    }

    /// <inheritdoc />
    protected override void OnDragLeave(object? sender, DragEventArgs e)
    {
        SetHighlight(false);
    }

    /// <inheritdoc />
    protected override void OnDrop(object? sender, DragEventArgs e)
    {
        SetHighlight(false);
    }

    private void SetHighlight(bool highlight)
    {
        var target = TargetControl ?? AssociatedObject;
        if (target is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(ClassName))
        {
            if (highlight)
            {
                if (IsPseudoClass)
                {
                    ((IPseudoClasses)target.Classes).Add(ClassName);
                }
                else
                {
                    target.Classes.Add(ClassName);
                }
            }
            else
            {
                if (IsPseudoClass)
                {
                    ((IPseudoClasses)target.Classes).Remove(ClassName);
                }
                else
                {
                    target.Classes.Remove(ClassName);
                }
            }
        }

        IsHighlighted = highlight;
    }
}

