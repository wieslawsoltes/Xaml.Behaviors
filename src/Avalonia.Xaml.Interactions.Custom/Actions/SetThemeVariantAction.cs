using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="StyledElement.RequestedThemeVariant"/> on the target control when executed.
/// </summary>
public class SetThemeVariantAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StyledElement?> TargetProperty =
        AvaloniaProperty.Register<SetThemeVariantAction, StyledElement?>(nameof(Target));

    /// <summary>
    /// Identifies the <see cref="ThemeVariant"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ThemeVariant?> ThemeVariantProperty =
        AvaloniaProperty.Register<SetThemeVariantAction, ThemeVariant?>(nameof(ThemeVariant));

    /// <summary>
    /// Gets or sets the target element. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public StyledElement? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the theme variant to assign. This is an avalonia property.
    /// </summary>
    public ThemeVariant? ThemeVariant
    {
        get => GetValue(ThemeVariantProperty);
        set => SetValue(ThemeVariantProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = Target ?? sender as StyledElement;
        if (target is null)
        {
            return false;
        }

        target.SetCurrentValue(StyledElement.RequestedThemeVariantProperty, ThemeVariant);
        return true;
    }
}
