// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Describes a state for <see cref="SplitViewStateBehavior"/> using size conditions.
/// </summary>
public partial class SplitViewStateSetter : AvaloniaObject
{

    /// <summary>
    /// Gets or sets minimum bounds width. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double MinWidth { get; set; }

    /// <summary>
    /// Gets or sets minimum width comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.GreaterThanOrEqual)]
    public partial ComparisonConditionType MinWidthOperator { get; set; }

    /// <summary>
    /// Gets or sets maximum bounds width. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = double.PositiveInfinity)]
    public partial double MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets maximum width comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.LessThan)]
    public partial ComparisonConditionType MaxWidthOperator { get; set; }

    /// <summary>
    /// Gets or sets minimum bounds height. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double MinHeight { get; set; }

    /// <summary>
    /// Gets or sets minimum height comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.GreaterThanOrEqual)]
    public partial ComparisonConditionType MinHeightOperator { get; set; }

    /// <summary>
    /// Gets or sets maximum bounds height. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = double.PositiveInfinity)]
    public partial double MaxHeight { get; set; }

    /// <summary>
    /// Gets or sets maximum height comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.LessThan)]
    public partial ComparisonConditionType MaxHeightOperator { get; set; }

    /// <summary>
    /// Gets or sets the display mode to apply. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial SplitViewDisplayMode DisplayMode { get; set; }

    /// <summary>
    /// Gets or sets the pane placement to apply. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial SplitViewPanePlacement PanePlacement { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the pane is open. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool IsPaneOpen { get; set; }

    /// <summary>
    /// Gets or sets the target <see cref="SplitView"/> to apply the state to. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial SplitView? TargetSplitView { get; set; }
}
