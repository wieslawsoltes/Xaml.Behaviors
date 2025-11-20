using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that visualizes events on the attached control for debugging purposes.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class VisualDebugBehavior : EventTriggerBase
{
    /// <summary>
    /// Identifies the <seealso cref="HighlightColor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Color> HighlightColorProperty =
        AvaloniaProperty.Register<VisualDebugBehavior, Color>(nameof(HighlightColor), Colors.Red);

    /// <summary>
    /// Identifies the <seealso cref="Duration"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<VisualDebugBehavior, TimeSpan>(nameof(Duration), TimeSpan.FromSeconds(0.5));

    /// <summary>
    /// Gets or sets the color used to highlight the control when the event fires.
    /// </summary>
    public Color HighlightColor
    {
        get => GetValue(HighlightColorProperty);
        set => SetValue(HighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the highlight.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

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
