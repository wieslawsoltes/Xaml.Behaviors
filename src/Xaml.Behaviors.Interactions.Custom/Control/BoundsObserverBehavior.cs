// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Observes the bounds of an associated <see cref="Control"/> and updates its Width and Height properties.
/// </summary>
public class BoundsObserverBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// Defines the <see cref="Bounds"/> property.
    /// </summary>
    public static readonly StyledProperty<Rect> BoundsProperty =
        AvaloniaProperty.Register<BoundsObserverBehavior, Rect>(nameof(Bounds), defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// Defines the <see cref="Width"/> property.
    /// </summary>
    public static readonly StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<BoundsObserverBehavior, double>(nameof(Width),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="Height"/> property.
    /// </summary>
    public static readonly StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<BoundsObserverBehavior, double>(nameof(Height),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the bounds of the associated control. This is a styled Avalonia property.
    /// </summary>
    public Rect Bounds
    {
        get => GetValue(BoundsProperty);
        set => SetValue(BoundsProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the associated control. This is a two-way bound Avalonia property.
    /// </summary>
    public double Width
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the associated control. This is a two-way bound Avalonia property.
    /// </summary>
    public double Height
    {
        get => GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    /// <summary>
    /// Attaches the behavior to the associated control and starts observing its bounds to update the Width and Height properties accordingly.
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is not null)
        {
            return this.GetObservable(BoundsProperty)
                .Subscribe(new AnonymousObserver<Rect>(bounds =>
                {
                    Width = bounds.Width;
                    Height = bounds.Height;
                }));
        }
        
        return DisposableAction.Empty;
    }
}
