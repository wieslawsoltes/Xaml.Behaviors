// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Describes a state for <see cref="HamburgerMenuStateBehavior"/> using size conditions.
/// </summary>
public class HamburgerMenuStateSetter : AvaloniaObject
{
    /// <summary>
    /// Identifies the <seealso cref="MinWidth"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinWidthProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, double>(nameof(MinWidth));

    /// <summary>
    /// Identifies the <seealso cref="MinWidthOperator"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> MinWidthOperatorProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, ComparisonConditionType>(nameof(MinWidthOperator), ComparisonConditionType.GreaterThanOrEqual);

    /// <summary>
    /// Identifies the <seealso cref="MaxWidth"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxWidthProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, double>(nameof(MaxWidth), double.PositiveInfinity);

    /// <summary>
    /// Identifies the <seealso cref="MaxWidthOperator"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> MaxWidthOperatorProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, ComparisonConditionType>(nameof(MaxWidthOperator), ComparisonConditionType.LessThan);

    /// <summary>
    /// Identifies the <seealso cref="MinHeight"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinHeightProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, double>(nameof(MinHeight));

    /// <summary>
    /// Identifies the <seealso cref="MinHeightOperator"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> MinHeightOperatorProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, ComparisonConditionType>(nameof(MinHeightOperator), ComparisonConditionType.GreaterThanOrEqual);

    /// <summary>
    /// Identifies the <seealso cref="MaxHeight"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxHeightProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, double>(nameof(MaxHeight), double.PositiveInfinity);

    /// <summary>
    /// Identifies the <seealso cref="MaxHeightOperator"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> MaxHeightOperatorProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, ComparisonConditionType>(nameof(MaxHeightOperator), ComparisonConditionType.LessThan);

    /// <summary>
    /// Identifies the <seealso cref="DisplayMode"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<HamburgerMenuDisplayMode> DisplayModeProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, HamburgerMenuDisplayMode>(nameof(DisplayMode));

    /// <summary>
    /// Identifies the <seealso cref="PanePlacement"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<HamburgerMenuPanePlacement> PanePlacementProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, HamburgerMenuPanePlacement>(nameof(PanePlacement));

    /// <summary>
    /// Identifies the <seealso cref="IsPaneOpen"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, bool>(nameof(IsPaneOpen));

    /// <summary>
    /// Identifies the <seealso cref="TargetHamburgerMenu"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<HamburgerMenu?> TargetHamburgerMenuProperty =
        AvaloniaProperty.Register<HamburgerMenuStateSetter, HamburgerMenu?>(nameof(TargetHamburgerMenu));

    /// <summary>
    /// Gets or sets minimum bounds width. This is an avalonia property.
    /// </summary>
    public double MinWidth
    {
        get => GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum width comparison operator. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType MinWidthOperator
    {
        get => GetValue(MinWidthOperatorProperty);
        set => SetValue(MinWidthOperatorProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum bounds width. This is an avalonia property.
    /// </summary>
    public double MaxWidth
    {
        get => GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum width comparison operator. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType MaxWidthOperator
    {
        get => GetValue(MaxWidthOperatorProperty);
        set => SetValue(MaxWidthOperatorProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum bounds height. This is an avalonia property.
    /// </summary>
    public double MinHeight
    {
        get => GetValue(MinHeightProperty);
        set => SetValue(MinHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum height comparison operator. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType MinHeightOperator
    {
        get => GetValue(MinHeightOperatorProperty);
        set => SetValue(MinHeightOperatorProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum bounds height. This is an avalonia property.
    /// </summary>
    public double MaxHeight
    {
        get => GetValue(MaxHeightProperty);
        set => SetValue(MaxHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum height comparison operator. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType MaxHeightOperator
    {
        get => GetValue(MaxHeightOperatorProperty);
        set => SetValue(MaxHeightOperatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the display mode to apply. This is an avalonia property.
    /// </summary>
    public HamburgerMenuDisplayMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the pane placement to apply. This is an avalonia property.
    /// </summary>
    public HamburgerMenuPanePlacement PanePlacement
    {
        get => GetValue(PanePlacementProperty);
        set => SetValue(PanePlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the pane is open. This is an avalonia property.
    /// </summary>
    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the target <see cref="HamburgerMenu"/> to apply the state to. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public HamburgerMenu? TargetHamburgerMenu
    {
        get => GetValue(TargetHamburgerMenuProperty);
        set => SetValue(TargetHamburgerMenuProperty, value);
    }
}
