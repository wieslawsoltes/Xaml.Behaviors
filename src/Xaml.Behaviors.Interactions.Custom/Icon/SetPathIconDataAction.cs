using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Changes the <see cref="PathIcon.Data"/> of a target icon when executed.
/// </summary>
public class SetPathIconDataAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Data"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<SetPathIconDataAction, Geometry?>(nameof(Data));

    /// <summary>
    /// Identifies the <seealso cref="PathIcon"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<PathIcon?> PathIconProperty =
        AvaloniaProperty.Register<SetPathIconDataAction, PathIcon?>(nameof(PathIcon));

    /// <summary>
    /// Gets or sets the geometry to apply. This is an avalonia property.
    /// </summary>
    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// Gets or sets the target icon. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public PathIcon? PathIcon
    {
        get => GetValue(PathIconProperty);
        set => SetValue(PathIconProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = GetValue(PathIconProperty) ?? sender as PathIcon;
        if (target is null)
        {
            return false;
        }

        target.Data = Data;
        return true;
    }
}
