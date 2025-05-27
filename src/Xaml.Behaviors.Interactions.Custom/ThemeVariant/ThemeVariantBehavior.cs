// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="ThemeVariantScope.RequestedThemeVariant"/> on the associated control.
/// </summary>
public class ThemeVariantBehavior : AttachedToVisualTreeBehavior<ThemeVariantScope>
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
        AssociatedObject.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, ThemeVariant);

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, old);
            }
        });
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ThemeVariantProperty && AssociatedObject is not null)
        {
            AssociatedObject.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, change.GetNewValue<ThemeVariant?>());
        }
    }
}
