// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Toggles the <see cref="HamburgerMenu.IsPaneOpen"/> state of a <see cref="HamburgerMenu"/> when executed.
/// </summary>
public class HamburgerMenuTogglePaneAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TargetHamburgerMenu"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<HamburgerMenu?> TargetHamburgerMenuProperty =
        AvaloniaProperty.Register<HamburgerMenuTogglePaneAction, HamburgerMenu?>(nameof(TargetHamburgerMenu));

    /// <summary>
    /// Gets or sets the target <see cref="HamburgerMenu"/>. If not set, the sender is used.
    /// This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public HamburgerMenu? TargetHamburgerMenu
    {
        get => GetValue(TargetHamburgerMenuProperty);
        set => SetValue(TargetHamburgerMenuProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var hamburgerMenu = GetValue(TargetHamburgerMenuProperty) is not null ? TargetHamburgerMenu : sender as HamburgerMenu;
        if (hamburgerMenu is null)
        {
            return false;
        }

        hamburgerMenu.IsPaneOpen = !hamburgerMenu.IsPaneOpen;
        return true;
    }
}
