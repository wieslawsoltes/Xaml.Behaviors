using Avalonia.Controls;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="StyledElement.RequestedThemeVariant"/> on the associated control.
/// </summary>
public class ThemeVariantBehavior : AttachedToVisualTreeBehavior<StyledElement>
{
    /// <summary>
    /// Identifies the <see cref="ThemeVariant"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ThemeVariant?> ThemeVariantProperty =
        AvaloniaProperty.Register<ThemeVariantBehavior, ThemeVariant?>(nameof(ThemeVariant));

    /// <summary>
    /// Gets or sets the theme variant to assign. This is an avalonia property.
    /// </summary>
    public ThemeVariant? ThemeVariant
    {
        get => GetValue(ThemeVariantProperty);
        set => SetValue(ThemeVariantProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        var old = AssociatedObject.RequestedThemeVariant;
        AssociatedObject.SetCurrentValue(StyledElement.RequestedThemeVariantProperty, ThemeVariant);

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.SetCurrentValue(StyledElement.RequestedThemeVariantProperty, old);
            }
        });
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ThemeVariantProperty && AssociatedObject is not null)
        {
            AssociatedObject.SetCurrentValue(StyledElement.RequestedThemeVariantProperty, change.GetNewValue<ThemeVariant?>());
        }
    }
}
