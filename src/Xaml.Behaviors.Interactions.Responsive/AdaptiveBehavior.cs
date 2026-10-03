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
/// Observes <see cref="StyledElementBehavior{T}.AssociatedObject"/> control or <see cref="SourceControl"/> control <see cref="Visual.Bounds"/> property changes and if triggered sets or removes style classes when conditions from <see cref="AdaptiveClassSetter"/> are met.
/// </summary>
/// <remarks>
/// WinUI has no style classes: on Uno Platform adding or removing a class (or pseudo class) moves the target control
/// to the visual state of the same name (leading <c>:</c> removed, PascalCase accepted) and back to its <c>Not{Name}</c>,
/// <c>Normal</c> or <c>Default</c> state (see <c>VisualStateManager</c>).
/// </remarks>
public partial class AdaptiveBehavior : StyledElementBehavior<Control>
{
    private IDisposable? _disposable;

    /// <summary>
    /// Gets or sets the the source control that <see cref="Visual.BoundsProperty"/> property are observed from, if not set <see cref="StyledElementBehavior{T}.AssociatedObject"/> is used. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? SourceControl { get; set; }

    /// <summary>
    /// Gets or sets the target control that class name that should be added or removed when triggered, if not set <see cref="StyledElementBehavior{T}.AssociatedObject"/> is used or <see cref="AdaptiveClassSetter.TargetControl"/> from <see cref="AdaptiveClassSetter"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <summary>
    /// Gets adaptive class setters collection. This is an avalonia property.
    /// </summary>
    /// <remarks>
    /// On Uno Platform the setters are a <c>DependencyObjectCollection</c> so they inherit the data context of the behavior.
    /// </remarks>
    [DirectProperty(Lazy = true, Content = true)]
#if WINUI
    public partial AdaptiveClassSetterCollection Setters { get; }
#elif UNO
    public partial DependencyObjectCollection<AdaptiveClassSetter> Setters { get; }
#else
    public partial AvaloniaList<AdaptiveClassSetter> Setters { get; }
#endif

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        StopObserving();
        StartObserving();
    }

    /// <inheritdoc/>
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
            .Subscribe(new AnonymousObserver<Rect>(bounds =>
            {
                Execute(sourceControl, Setters, bounds);
            }));
    }

    private void Execute(Control? sourceControl, IEnumerable<AdaptiveClassSetter>? setters, Rect bounds)
    {
        if (sourceControl is null || setters is null)
        {
            return;
        }

        foreach (var setter in setters)
        {
            var isMinOrMaxWidthSet = setter.IsSet(AdaptiveClassSetter.MinWidthProperty)
                                     || setter.IsSet(AdaptiveClassSetter.MaxWidthProperty);
            var widthConditionTriggered = GetResult(setter.MinWidthOperator, bounds.Width, setter.MinWidth)
                                          && GetResult(setter.MaxWidthOperator, bounds.Width, setter.MaxWidth);

            var isMinOrMaxHeightSet = setter.IsSet(AdaptiveClassSetter.MinHeightProperty)
                                      || setter.IsSet(AdaptiveClassSetter.MaxHeightProperty);
            var heightConditionTriggered = GetResult(setter.MinHeightOperator, bounds.Height, setter.MinHeight)
                                           && GetResult(setter.MaxHeightOperator, bounds.Height, setter.MaxHeight);

            var isAddClassTriggered = isMinOrMaxWidthSet switch
            {
                true when !isMinOrMaxHeightSet => widthConditionTriggered,
                false when isMinOrMaxHeightSet => heightConditionTriggered,
                true when isMinOrMaxHeightSet => widthConditionTriggered && heightConditionTriggered,
                _ => false
            };

            var targetControl = setter.GetValue(AdaptiveClassSetter.TargetControlProperty) is not null
                ? setter.TargetControl 
                : GetValue(TargetControlProperty) is not null
                    ? TargetControl 
                    : AssociatedObject;

            if (targetControl is not null)
            {
                var className = setter.ClassName;
                var isPseudoClass = setter.IsPseudoClass;

                if (isAddClassTriggered)
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

    private bool GetResult(ComparisonConditionType comparisonConditionType, double property, double value)
    {
        return comparisonConditionType switch
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            ComparisonConditionType.Equal => property == value,
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            ComparisonConditionType.NotEqual => property != value,
            ComparisonConditionType.LessThan => property < value,
            ComparisonConditionType.LessThanOrEqual => property <= value,
            ComparisonConditionType.GreaterThan => property > value,
            ComparisonConditionType.GreaterThanOrEqual => property >= value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static void Add(Control targetControl, string? className, bool isPseudoClass)
    {
        if (className is null || string.IsNullOrEmpty(className) || targetControl.Classes.Contains(className))
        {
            return;
        }

        if (isPseudoClass)
        {
            ((IPseudoClasses) targetControl.Classes).Add(className);
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
            ((IPseudoClasses) targetControl.Classes).Remove(className);
        }
        else
        {
            targetControl.Classes.Remove(className);
        }
    }
}
