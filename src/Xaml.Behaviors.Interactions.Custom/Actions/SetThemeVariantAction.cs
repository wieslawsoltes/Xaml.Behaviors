// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
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
/// Sets the <see cref="ThemeVariantScope.RequestedThemeVariant"/> on the target control when executed.
/// </summary>
public partial class SetThemeVariantAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target element. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ThemeVariantScope? Target { get; set; }

    /// <summary>
    /// Gets or sets the theme variant to assign. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial ThemeVariant? ThemeVariant { get; set; }

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
