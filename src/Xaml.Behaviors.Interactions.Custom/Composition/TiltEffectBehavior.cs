// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that applies a 3D tilt rotation to the element based on the pointer position.
/// </summary>
public partial class TiltEffectBehavior : Behavior<Control>
{

    /// <summary>
    /// Gets or sets the maximum tilt angle in degrees.
    /// </summary>
    [StyledProperty(DefaultValue = 5.0)]
    public partial double TiltStrength { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerExited += OnPointerExited;
            AssociatedObject.SizeChanged += OnSizeChanged;
            TiltAnimation.UpdateCenterPoint(AssociatedObject);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved -= OnPointerMoved;
            AssociatedObject.PointerExited -= OnPointerExited;
            AssociatedObject.SizeChanged -= OnSizeChanged;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject is not null)
        {
#if UNO
            TiltAnimation.Apply(AssociatedObject, e.GetCurrentPoint(AssociatedObject).Position, TiltStrength);
#else
            TiltAnimation.Apply(AssociatedObject, e.GetPosition(AssociatedObject), TiltStrength);
#endif
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        TiltAnimation.Reset(AssociatedObject);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        TiltAnimation.UpdateCenterPoint(AssociatedObject);
    }
}
