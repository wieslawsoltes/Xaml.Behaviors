// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using ThemeVariant = Microsoft.UI.Xaml.ElementTheme;
using ThemeVariantScope = Microsoft.UI.Xaml.FrameworkElement;
#else
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Sets the <see cref="ThemeVariantScope.RequestedThemeVariant"/> on the associated control.
/// </summary>
/// <remarks>
/// On Uno Platform the behavior sets <c>FrameworkElement.RequestedTheme</c> (a <c>null</c> theme variant maps to
/// <c>ElementTheme.Default</c>).
/// </remarks>
public partial class ThemeVariantBehavior : AttachedToVisualTreeBehavior<ThemeVariantScope>
{

    /// <summary>
    /// Gets or sets the theme variant to assign. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial ThemeVariant? ThemeVariant { get; set; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

#if UNO
        var old = AssociatedObject.RequestedTheme;
        AssociatedObject.RequestedTheme = ThemeVariant ?? Microsoft.UI.Xaml.ElementTheme.Default;

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.RequestedTheme = old;
            }
        });
#else
        var old = AssociatedObject.RequestedThemeVariant;
        AssociatedObject.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, ThemeVariant);

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, old);
            }
        });
#endif
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ThemeVariantProperty && AssociatedObject is not null)
        {
#if UNO
            AssociatedObject.RequestedTheme = change.GetNewValue<ThemeVariant?>() ?? Microsoft.UI.Xaml.ElementTheme.Default;
#else
            AssociatedObject.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, change.GetNewValue<ThemeVariant?>());
#endif
        }
    }
}
