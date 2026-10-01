// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Xaml.Interactivity;

namespace BehaviorsTestApplication.Behaviors;

/// <summary>
/// Publishes the layout bounds of the associated element as a bindable <see cref="Bounds"/> value, the counterpart of
/// the Avalonia <c>Visual.Bounds</c> property used by the samples (<c>{Binding #Target.Bounds}</c>,
/// <c>{Binding $parent[TopLevel].Bounds.Width}</c>).
/// </summary>
/// <remarks>
/// WinUI elements expose their layout size through <c>ActualWidth</c>/<c>ActualHeight</c>, which raise no change
/// notifications for bindings. The Uno Platform views bind <c>{x:Bind Observer.Bounds, Mode=OneWay}</c> instead.
/// </remarks>
public sealed partial class ElementBoundsBehavior : Behavior<FrameworkElement>
{
    /// <summary>
    /// Identifies the <see cref="Bounds"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoundsProperty =
        DependencyProperty.Register(nameof(Bounds), typeof(Rect), typeof(ElementBoundsBehavior), new PropertyMetadata(default(Rect)));

    /// <summary>
    /// Gets the bounds of the associated element (its offset in the parent and its actual size).
    /// </summary>
    public Rect Bounds
    {
        get => (Rect)GetValue(BoundsProperty);
        private set => SetValue(BoundsProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is { } element)
        {
            element.SizeChanged += OnSizeChanged;
            element.LayoutUpdated += OnLayoutUpdated;
            Update();
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } element)
        {
            element.SizeChanged -= OnSizeChanged;
            element.LayoutUpdated -= OnLayoutUpdated;
        }

        base.OnDetaching();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => Update();

    private void OnLayoutUpdated(object? sender, object e) => Update();

    private void Update()
    {
        if (AssociatedObject is not { } element)
        {
            return;
        }

        var offset = element.ActualOffset;
        var bounds = new Rect(offset.X, offset.Y, Math.Max(0, element.ActualWidth), Math.Max(0, element.ActualHeight));
        if (bounds != Bounds)
        {
            Bounds = bounds;
        }
    }
}
