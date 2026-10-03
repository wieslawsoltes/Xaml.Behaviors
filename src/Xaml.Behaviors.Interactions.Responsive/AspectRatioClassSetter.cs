// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Responsive;
#else
namespace Avalonia.Xaml.Interactions.Responsive;
#endif

/// <summary>
/// Conditional class setter based on aspect ratio used in <see cref="AspectRatioBehavior"/>.
/// </summary>
public partial class AspectRatioClassSetter : AvaloniaObject
{

    /// <summary>
    /// Gets or sets minimum aspect ratio used for comparison. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double MinRatio { get; set; }

    /// <summary>
    /// Gets or sets minimum aspect ratio comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.GreaterThanOrEqual)]
    public partial ComparisonConditionType MinRatioOperator { get; set; }

    /// <summary>
    /// Gets or sets maximum aspect ratio used for comparison. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = double.PositiveInfinity)]
    public partial double MaxRatio { get; set; }

    /// <summary>
    /// Gets or sets maximum aspect ratio comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.LessThan)]
    public partial ComparisonConditionType MaxRatioOperator { get; set; }

    /// <summary>
    /// Gets or sets the class name that should be added or removed. This is an avalonia property.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets the flag whether <see cref="ClassName"/> is a pseudo class. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool IsPseudoClass { get; set; }

    /// <summary>
    /// Gets or sets the target control that class name should be added or removed from when triggered. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }
}
