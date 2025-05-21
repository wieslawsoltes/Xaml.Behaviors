using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Styling;
using Avalonia.Xaml.Interactions.Custom;

namespace Avalonia.Xaml.Interactions.Responsive;

/// <summary>
/// Observes <see cref="StyledElement.ActualThemeVariant"/> changes on a control and
/// toggles classes when specified theme variants are active.
/// </summary>
public class ThemeVariantBehavior : ActualThemeVariantChangedBehavior<StyledElement>
{
    private AvaloniaList<ThemeVariantClassSetter>? _setters;

    /// <summary>
    /// Identifies the <seealso cref="SourceElement"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StyledElement?> SourceElementProperty =
        AvaloniaProperty.Register<ThemeVariantBehavior, StyledElement?>(nameof(SourceElement));

    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ThemeVariantBehavior, Control?>(nameof(TargetControl));

    /// <summary>
    /// Identifies the <seealso cref="Setters"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ThemeVariantBehavior, AvaloniaList<ThemeVariantClassSetter>> SettersProperty =
        AvaloniaProperty.RegisterDirect<ThemeVariantBehavior, AvaloniaList<ThemeVariantClassSetter>>(nameof(Setters), t => t.Setters);

    /// <summary>
    /// Gets or sets the element whose theme variant is observed. If not set, <see cref="StyledElementBehavior{T}.AssociatedObject"/> is used. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public StyledElement? SourceElement
    {
        get => GetValue(SourceElementProperty);
        set => SetValue(SourceElementProperty, value);
    }

    /// <summary>
    /// Gets or sets the target control to add or remove classes from. If not set, the associated object or setter TargetControl is used. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <summary>
    /// Gets theme variant class setters collection. This is an avalonia property.
    /// </summary>
    [Content]
    public AvaloniaList<ThemeVariantClassSetter> Setters => _setters ??= [];

    /// <inheritdoc />
    protected override IDisposable OnActualThemeVariantChangedEventOverride()
    {
        var source = SourceElement ?? AssociatedObject;
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Execute(source.ActualThemeVariant);

        return source.GetObservable(StyledElement.ActualThemeVariantProperty)
            .Subscribe(new AnonymousObserver<ThemeVariant>(variant => Execute(variant)));
    }

    private void Execute(ThemeVariant variant)
    {
        if (Setters is null)
        {
            return;
        }

        foreach (var setter in Setters)
        {
            var target = setter.GetValue(ThemeVariantClassSetter.TargetControlProperty) ?? TargetControl ?? AssociatedObject as Control;
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var className = setter.ClassName;
            var isPseudoClass = setter.IsPseudoClass;

            if (variant.Equals(setter.ThemeVariant))
            {
                Add(target, className, isPseudoClass);
            }
            else
            {
                Remove(target, className, isPseudoClass);
            }
        }
    }

    private static void Add(Control targetControl, string? className, bool isPseudoClass)
    {
        if (className is null || string.IsNullOrEmpty(className) || targetControl.Classes.Contains(className))
        {
            return;
        }

        if (isPseudoClass)
        {
            ((IPseudoClasses)targetControl.Classes).Add(className);
        }
        else
        {
            targetControl.Classes.Add(className);
        }
    }

    private static void Remove(Control targetControl, string? className, bool isPseudoClass)
    {
        if (className is null || string.IsNullOrEmpty(className) || !targetControl.Classes.Contains(className))
        {
            return;
        }

        if (isPseudoClass)
        {
            ((IPseudoClasses)targetControl.Classes).Remove(className);
        }
        else
        {
            targetControl.Classes.Remove(className);
        }
    }
}
