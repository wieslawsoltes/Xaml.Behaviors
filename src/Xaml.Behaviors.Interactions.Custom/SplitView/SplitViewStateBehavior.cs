// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Updates <see cref="SplitView"/> properties based on size conditions.
/// </summary>
public class SplitViewStateBehavior : StyledElementBehavior<SplitView>
{
    private IDisposable? _disposable;
    private AvaloniaList<SplitViewStateSetter>? _setters;

    /// <summary>
    /// Identifies the <seealso cref="SourceControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> SourceControlProperty =
        AvaloniaProperty.Register<SplitViewStateBehavior, Control?>(nameof(SourceControl));

    /// <summary>
    /// Identifies the <seealso cref="Setters"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<SplitViewStateBehavior, AvaloniaList<SplitViewStateSetter>> SettersProperty =
        AvaloniaProperty.RegisterDirect<SplitViewStateBehavior, AvaloniaList<SplitViewStateSetter>>(nameof(Setters), b => b.Setters);

    /// <summary>
    /// Gets or sets the control whose bounds are observed. If not set, the associated object is used.
    /// This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? SourceControl
    {
        get => GetValue(SourceControlProperty);
        set => SetValue(SourceControlProperty, value);
    }

    /// <summary>
    /// Gets split view state setters collection. This is an avalonia property.
    /// </summary>
    [Content]
    public AvaloniaList<SplitViewStateSetter> Setters => _setters ??= [];

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
        var source = GetValue(SourceControlProperty) is not null ? SourceControl : AssociatedObject;
        if (source is not null)
        {
            _disposable = ObserveBounds(source);
        }
    }

    private void StopObserving() => _disposable?.Dispose();

    private IDisposable ObserveBounds(Control source)
    {
        Execute(Setters, source.Bounds);
        return source.GetObservable(Visual.BoundsProperty)
            .Subscribe(new AnonymousObserver<Rect>(b => Execute(Setters, b)));
    }

    private void Execute(AvaloniaList<SplitViewStateSetter>? setters, Rect bounds)
    {
        if (AssociatedObject is null || setters is null)
        {
            return;
        }

        foreach (var setter in setters)
        {
            var widthSet = setter.IsSet(SplitViewStateSetter.MinWidthProperty) || setter.IsSet(SplitViewStateSetter.MaxWidthProperty);
            var widthMatch = GetResult(setter.MinWidthOperator, bounds.Width, setter.MinWidth) &&
                             GetResult(setter.MaxWidthOperator, bounds.Width, setter.MaxWidth);

            var heightSet = setter.IsSet(SplitViewStateSetter.MinHeightProperty) || setter.IsSet(SplitViewStateSetter.MaxHeightProperty);
            var heightMatch = GetResult(setter.MinHeightOperator, bounds.Height, setter.MinHeight) &&
                              GetResult(setter.MaxHeightOperator, bounds.Height, setter.MaxHeight);

            var isTriggered = widthSet switch
            {
                true when !heightSet => widthMatch,
                false when heightSet => heightMatch,
                true when heightSet => widthMatch && heightMatch,
                _ => false
            };

            if (!isTriggered)
            {
                continue;
            }

            var target = setter.GetValue(SplitViewStateSetter.TargetSplitViewProperty) is not null
                ? setter.TargetSplitView
                : AssociatedObject;

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (setter.IsSet(SplitViewStateSetter.DisplayModeProperty))
            {
                target.DisplayMode = setter.DisplayMode;
            }
            if (setter.IsSet(SplitViewStateSetter.PanePlacementProperty))
            {
                target.PanePlacement = setter.PanePlacement;
            }
            if (setter.IsSet(SplitViewStateSetter.IsPaneOpenProperty))
            {
                target.IsPaneOpen = setter.IsPaneOpen;
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
}
