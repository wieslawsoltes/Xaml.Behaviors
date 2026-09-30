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
/// A behavior that displays cursor position on <see cref="InputElement.PointerMoved"/> event for the <see cref="StyledElementBehavior{T}.AssociatedObject"/> using <see cref="TextBlock.Text"/> property.
/// </summary>
public partial class ShowPointerPositionBehavior : StyledElementBehavior<Control>
{

    /// <summary>
    /// Gets or sets the target TextBlock object in which this behavior displays cursor position on PointerMoved event.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial TextBlock? TargetTextBlock { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved += AssociatedObject_PointerMoved;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved -= AssociatedObject_PointerMoved;
        }
    }

    private void AssociatedObject_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (TargetTextBlock is not null)
        {
            TargetTextBlock.Text = e.GetPosition(AssociatedObject).ToString();
        }
    }
}
