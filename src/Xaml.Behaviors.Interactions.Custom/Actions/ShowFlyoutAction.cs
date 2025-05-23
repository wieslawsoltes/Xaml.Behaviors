using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows a <see cref="FlyoutBase"/> when executed.
/// </summary>
public class ShowFlyoutAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Flyout"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<FlyoutBase?> FlyoutProperty =
        AvaloniaProperty.Register<ShowFlyoutAction, FlyoutBase?>(nameof(Flyout));

    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ShowFlyoutAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets the flyout instance to show. If not set, the attached flyout of the target control is used.
    /// </summary>
    public FlyoutBase? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    /// <summary>
    /// Gets or sets the target control that hosts the flyout. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        if (control is null)
        {
            return false;
        }

        var flyout = Flyout ?? FlyoutBase.GetAttachedFlyout(control);
        if (flyout is null)
        {
            return false;
        }

        flyout.ShowAt(control);

        return true;
    }
}
