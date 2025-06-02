using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="PathIcon.Data"/> when the associated icon is attached to the visual tree.
/// </summary>
public class PathIconDataBehavior : AttachedToVisualTreeBehavior<PathIcon>
{
    /// <summary>
    /// Identifies the <seealso cref="Data"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<PathIconDataBehavior, Geometry?>(nameof(Data));

    private Geometry? _oldData;

    /// <summary>
    /// Gets or sets the geometry used for the icon. This is an avalonia property.
    /// </summary>
    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <inheritdoc />
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        _oldData = AssociatedObject.Data;
        if (Data is not null)
        {
            AssociatedObject.Data = Data;
        }

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.Data = _oldData;
            }
        });
    }
}
