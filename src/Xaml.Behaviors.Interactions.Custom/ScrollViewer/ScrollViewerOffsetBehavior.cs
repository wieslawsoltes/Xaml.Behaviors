using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="ScrollViewer.Offset"/> of the associated <see cref="ScrollViewer"/>.
/// </summary>
public class ScrollViewerOffsetBehavior : AttachedToVisualTreeBehavior<ScrollViewer>
{
    /// <summary>
    /// Identifies the <see cref="HorizontalOffset"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double?> HorizontalOffsetProperty =
        AvaloniaProperty.Register<ScrollViewerOffsetBehavior, double?>(nameof(HorizontalOffset));

    /// <summary>
    /// Identifies the <see cref="VerticalOffset"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double?> VerticalOffsetProperty =
        AvaloniaProperty.Register<ScrollViewerOffsetBehavior, double?>(nameof(VerticalOffset));

    /// <summary>
    /// Gets or sets the horizontal offset value. This is an avalonia property.
    /// </summary>
    public double? HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical offset value. This is an avalonia property.
    /// </summary>
    public double? VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        SetOffset();
        return DisposableAction.Empty;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == HorizontalOffsetProperty || change.Property == VerticalOffsetProperty)
        {
            SetOffset();
        }
    }

    private void SetOffset()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var offset = AssociatedObject.Offset;

        if (HorizontalOffset.HasValue)
        {
            offset = offset.WithX(HorizontalOffset.Value);
        }

        if (VerticalOffset.HasValue)
        {
            offset = offset.WithY(VerticalOffset.Value);
        }

        AssociatedObject.Offset = offset;
    }
}
