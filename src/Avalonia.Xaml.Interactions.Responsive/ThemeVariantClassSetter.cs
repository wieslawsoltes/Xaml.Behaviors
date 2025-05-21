using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Responsive;

/// <summary>
/// Conditional class setter based on the control's actual theme variant.
/// </summary>
public class ThemeVariantClassSetter : AvaloniaObject
{
    /// <summary>
    /// Identifies the <seealso cref="ThemeVariant"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ThemeVariant> ThemeVariantProperty =
        AvaloniaProperty.Register<ThemeVariantClassSetter, ThemeVariant>(nameof(ThemeVariant));

    /// <summary>
    /// Identifies the <seealso cref="ClassName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ClassNameProperty =
        AvaloniaProperty.Register<ThemeVariantClassSetter, string?>(nameof(ClassName));

    /// <summary>
    /// Identifies the <seealso cref="IsPseudoClass"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPseudoClassProperty =
        AvaloniaProperty.Register<ThemeVariantClassSetter, bool>(nameof(IsPseudoClass));

    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ThemeVariantClassSetter, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets the theme variant that should trigger adding the class. This is an avalonia property.
    /// </summary>
    public ThemeVariant ThemeVariant
    {
        get => GetValue(ThemeVariantProperty);
        set => SetValue(ThemeVariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the class name that should be added or removed. This is an avalonia property.
    /// </summary>
    [Content]
    public string? ClassName
    {
        get => GetValue(ClassNameProperty);
        set => SetValue(ClassNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the flag whether <see cref="ClassName"/> is a pseudo class. This is an avalonia property.
    /// </summary>
    public bool IsPseudoClass
    {
        get => GetValue(IsPseudoClassProperty);
        set => SetValue(IsPseudoClassProperty, value);
    }

    /// <summary>
    /// Gets or sets the target control that class name should be added or removed from when triggered. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }
}
