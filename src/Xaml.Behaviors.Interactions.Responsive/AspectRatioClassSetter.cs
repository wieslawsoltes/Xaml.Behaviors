// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Responsive;

/// <summary>
/// Conditional class setter based on aspect ratio used in <see cref="AspectRatioBehavior"/>.
/// </summary>
public class AspectRatioClassSetter : AvaloniaObject
{
    /// <summary>
    /// Identifies the <seealso cref="MinRatio"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinRatioProperty =
        AvaloniaProperty.Register<AspectRatioClassSetter, double>(nameof(MinRatio));

    /// <summary>
    /// Identifies the <seealso cref="MinRatioOperator"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> MinRatioOperatorProperty =
        AvaloniaProperty.Register<AspectRatioClassSetter, ComparisonConditionType>(nameof(MinRatioOperator), ComparisonConditionType.GreaterThanOrEqual);

    /// <summary>
    /// Identifies the <seealso cref="MaxRatio"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxRatioProperty =
        AvaloniaProperty.Register<AspectRatioClassSetter, double>(nameof(MaxRatio), double.PositiveInfinity);

    /// <summary>
    /// Identifies the <seealso cref="MaxRatioOperator"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> MaxRatioOperatorProperty =
        AvaloniaProperty.Register<AspectRatioClassSetter, ComparisonConditionType>(nameof(MaxRatioOperator), ComparisonConditionType.LessThan);

    /// <summary>
    /// Identifies the <seealso cref="ClassName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ClassNameProperty =
        AvaloniaProperty.Register<AspectRatioClassSetter, string?>(nameof(ClassName));

    /// <summary>
    /// Identifies the <seealso cref="IsPseudoClass"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPseudoClassProperty =
        AvaloniaProperty.Register<AspectRatioClassSetter, bool>(nameof(IsPseudoClass));

    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<AspectRatioClassSetter, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets minimum aspect ratio used for comparison. This is an avalonia property.
    /// </summary>
    public double MinRatio
    {
        get => GetValue(MinRatioProperty);
        set => SetValue(MinRatioProperty, value);
    }

    /// <summary>
    /// Gets or sets minimum aspect ratio comparison operator. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType MinRatioOperator
    {
        get => GetValue(MinRatioOperatorProperty);
        set => SetValue(MinRatioOperatorProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum aspect ratio used for comparison. This is an avalonia property.
    /// </summary>
    public double MaxRatio
    {
        get => GetValue(MaxRatioProperty);
        set => SetValue(MaxRatioProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum aspect ratio comparison operator. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType MaxRatioOperator
    {
        get => GetValue(MaxRatioOperatorProperty);
        set => SetValue(MaxRatioOperatorProperty, value);
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
