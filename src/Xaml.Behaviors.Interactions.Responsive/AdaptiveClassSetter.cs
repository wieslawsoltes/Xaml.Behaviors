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
/// Conditional class setter used in <see cref="AdaptiveBehavior"/> behavior.
/// </summary>
public partial class AdaptiveClassSetter : AvaloniaObject
{
        
    /// <summary>
    /// Gets or sets minimum bounds width value used for property comparison. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double MinWidth { get; set; }

    /// <summary>
    /// Gets or sets minimum bounds width value comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.GreaterThanOrEqual)]
    public partial ComparisonConditionType MinWidthOperator { get; set; }

    /// <summary>
    /// Gets or sets maximum width value used for property comparison. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = double.PositiveInfinity)]
    public partial double MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets maximum bounds width value comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.LessThan)]
    public partial ComparisonConditionType MaxWidthOperator { get; set; }

    /// <summary>
    /// Gets or sets minimum bounds height value used for property comparison. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double MinHeight { get; set; }

    /// <summary>
    /// Gets or sets minimum bounds height value comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.GreaterThanOrEqual)]
    public partial ComparisonConditionType MinHeightOperator { get; set; }

    /// <summary>
    /// Gets or sets maximum height value used for property comparison. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = double.PositiveInfinity)]
    public partial double MaxHeight { get; set; }

    /// <summary>
    /// Gets or sets maximum bounds height value comparison operator. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = ComparisonConditionType.LessThan)]
    public partial ComparisonConditionType MaxHeightOperator { get; set; }

    /// <summary>
    /// Gets or sets the class name that should be added or removed. This is an avalonia property.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets the flag whether ClassName is a PseudoClass. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool IsPseudoClass { get; set; }

    /// <summary>
    /// Gets or sets the target control that class name that should be added or removed when triggered, if not set <see cref="StyledElementBehavior{T}.AssociatedObject"/> is used or <see cref="AdaptiveBehavior.TargetControl"/> from <see cref="AdaptiveBehavior"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }
}
