using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="ThemeVariantScope.RequestedThemeVariant"/> on the target control when executed.
/// </summary>
public class SetThemeVariantAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ThemeVariantScope?> TargetProperty =
        AvaloniaProperty.Register<SetThemeVariantAction, ThemeVariantScope?>(nameof(Target));

    /// <summary>
    /// Identifies the <see cref="ThemeVariant"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ThemeVariant?> ThemeVariantProperty =
        AvaloniaProperty.Register<SetThemeVariantAction, ThemeVariant?>(nameof(ThemeVariant));

    /// <summary>
    /// Gets or sets the target element. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public ThemeVariantScope? Target
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

        var target = Target ?? sender as ThemeVariantScope;
        if (target is null)
        {
            return false;
        }

        target.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, ThemeVariant);
        return true;
    }
}
