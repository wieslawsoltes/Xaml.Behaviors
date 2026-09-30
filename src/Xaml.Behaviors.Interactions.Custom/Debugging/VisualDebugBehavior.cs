using System;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that visualizes events on the attached control for debugging purposes.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class VisualDebugBehavior : EventTriggerBase
{

    /// <summary>
    /// Gets or sets the color used to highlight the control when the event fires.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "Colors.Red")]
    public partial Color HighlightColor { get; set; }

    /// <summary>
    /// Gets or sets the duration of the highlight.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromSeconds(0.5)")]
    public partial TimeSpan Duration { get; set; }

    /// <inheritdoc />
    protected override void OnEvent(object? eventArgs)
    {
        // Execute any associated actions
        base.OnEvent(eventArgs);

        if (AssociatedObject is Control control)
        {
            ShowHighlight(control);
        }
    }

    private void ShowHighlight(Control control)
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(control);
        if (adornerLayer == null)
        {
            return;
        }

        var brush = new SolidColorBrush(HighlightColor);
        var adorner = new VisualDebugAdorner(control, brush);
        
        AdornerLayer.SetAdornedElement(adorner, control);
        adornerLayer.Children.Add(adorner);

        DispatcherTimer.RunOnce(() =>
        {
            adornerLayer.Children.Remove(adorner);
        }, Duration);
    }
}
