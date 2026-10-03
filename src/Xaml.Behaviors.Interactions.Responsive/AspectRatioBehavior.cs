// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Xaml.Interactivity;
#else
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Responsive;
#else
namespace Avalonia.Xaml.Interactions.Responsive;
#endif

/// <summary>
/// Observes bounds changes of a control (or a specified source) and conditionally adds or removes classes
/// based on <see cref="AspectRatioClassSetter"/> rules.
/// </summary>
/// <remarks>
/// WinUI has no style classes: on Uno Platform adding or removing a class (or pseudo class) moves the target control
/// to the visual state of the same name (leading <c>:</c> removed, PascalCase accepted) and back to its <c>Not{Name}</c>,
/// <c>Normal</c> or <c>Default</c> state (see <c>VisualStateManager</c>).
/// </remarks>
public partial class AspectRatioBehavior : StyledElementBehavior<Control>
{
    private IDisposable? _disposable;

    /// <summary>
    /// Gets or sets the control whose bounds are observed. If not set, <see cref="StyledElementBehavior{T}.AssociatedObject"/> is used. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? SourceControl { get; set; }

    /// <summary>
    /// Gets or sets the target control to add or remove classes from. If not set, the associated object or setter TargetControl is used. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <summary>
    /// Gets aspect ratio class setters collection. This is an avalonia property.
    /// </summary>
    /// <remarks>
    /// On Uno Platform the setters are a <c>DependencyObjectCollection</c> so they inherit the data context of the behavior.
    /// </remarks>
    [DirectProperty(Lazy = true, Content = true)]
#if UNO
    public partial DependencyObjectCollection<AspectRatioClassSetter> Setters { get; }
#else
    public partial AvaloniaList<AspectRatioClassSetter> Setters { get; }
#endif

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        StopObserving();
        StartObserving();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        StopObserving();
    }

    private void StartObserving()
    {
        var sourceControl = GetValue(SourceControlProperty) is not null
            ? SourceControl
            : AssociatedObject;

        if (sourceControl is not null)
        {
            _disposable = ObserveBounds(sourceControl);
        }
    }

    private void StopObserving()
    {
        _disposable?.Dispose();
    }

    private IDisposable ObserveBounds(Control sourceControl)
    {
        if (sourceControl is null)
        {
            throw new ArgumentNullException(nameof(sourceControl));
        }

        Execute(sourceControl, Setters, sourceControl.GetValue(Visual.BoundsProperty));

        return sourceControl.GetObservable(Visual.BoundsProperty)
            .Subscribe(new AnonymousObserver<Rect>(bounds => Execute(sourceControl, Setters, bounds)));
    }

    private void Execute(Control? sourceControl, IEnumerable<AspectRatioClassSetter>? setters, Rect bounds)
    {
        if (sourceControl is null || setters is null)
        {
            return;
        }

        var ratio = bounds.Height > 0 ? bounds.Width / bounds.Height : double.PositiveInfinity;

        foreach (var setter in setters)
        {
            var ratioConditionTriggered = GetResult(setter.MinRatioOperator, ratio, setter.MinRatio) &&
                                           GetResult(setter.MaxRatioOperator, ratio, setter.MaxRatio);

            var targetControl = setter.GetValue(AspectRatioClassSetter.TargetControlProperty) is not null
                ? setter.TargetControl
                : GetValue(TargetControlProperty) is not null
                    ? TargetControl
                    : AssociatedObject;

            if (targetControl is not null)
            {
                var className = setter.ClassName;
                var isPseudoClass = setter.IsPseudoClass;

                if (ratioConditionTriggered)
                {
                    Add(targetControl, className, isPseudoClass);
                }
                else
                {
                    Remove(targetControl, className, isPseudoClass);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(targetControl));
            }
        }
    }

    private static bool GetResult(ComparisonConditionType comparisonConditionType, double property, double value) => comparisonConditionType switch
    {
        ComparisonConditionType.Equal => property == value,
        ComparisonConditionType.NotEqual => property != value,
        ComparisonConditionType.LessThan => property < value,
        ComparisonConditionType.LessThanOrEqual => property <= value,
        ComparisonConditionType.GreaterThan => property > value,
        ComparisonConditionType.GreaterThanOrEqual => property >= value,
        _ => throw new ArgumentOutOfRangeException()
    };

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
