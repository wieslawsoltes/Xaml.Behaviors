using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Determines if the behavior applies to the associated element or its children.
/// </summary>
public enum FluidMoveScope
{
    /// <summary>
    /// Apply behavior to the element itself.
    /// </summary>
    Self,
    /// <summary>
    /// Apply behavior to the children of the element.
    /// </summary>
    Children
}

/// <summary>
/// Behavior that animates position changes of a control or its children.
/// </summary>
public class FluidMoveBehavior : Behavior<Visual>
{
    private readonly Dictionary<Control, PixelPoint> _positions = new();

    /// <summary>
    /// Identifies the <see cref="AppliesTo"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<FluidMoveScope> AppliesToProperty =
        AvaloniaProperty.Register<FluidMoveBehavior, FluidMoveScope>(nameof(AppliesTo));

    /// <summary>
    /// Identifies the <see cref="Duration"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<FluidMoveBehavior, TimeSpan>(nameof(Duration), TimeSpan.FromMilliseconds(300));

    /// <summary>
    /// Gets or sets how the behavior is applied.
    /// </summary>
    public FluidMoveScope AppliesTo
    {
        get => GetValue(AppliesToProperty);
        set => SetValue(AppliesToProperty, value);
    }

    /// <summary>
    /// Gets or sets animation duration.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is Layoutable v)
        {
            v.LayoutUpdated += OnLayoutUpdated;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is Layoutable v)
        {
            v.LayoutUpdated -= OnLayoutUpdated;
        }
        base.OnDetaching();
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (AssociatedObject is not { } root)
        {
            return;
        }

        if (AppliesTo == FluidMoveScope.Self && root is Control c)
        {
            UpdateControl(c, root);
        }
        else if (AppliesTo == FluidMoveScope.Children && root is Panel panel)
        {
            foreach (var child in panel.Children.OfType<Control>())
            {
                UpdateControl(child, root);
            }
        }
    }

    private void UpdateControl(Control control, Visual root)
    {
        var p = control.TranslatePoint(new Point(0, 0), root);
        if (p is null)
        {
            return;
        }

        var current = new PixelPoint((int)p.Value.X, (int)p.Value.Y);

        if (!_positions.TryGetValue(control, out var previous))
        {
            _positions[control] = current;
            return;
        }

        if (previous == current)
        {
            return;
        }

        var dx = previous.X - current.X;
        var dy = previous.Y - current.Y;

        if (control.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            control.RenderTransform = transform;
        }

        transform.X = dx;
        transform.Y = dy;

        var animation = new Animation.Animation
        {
            Duration = Duration,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, dx),
                        new Setter(TranslateTransform.YProperty, dy)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d),
                        new Setter(TranslateTransform.YProperty, 0d)
                    }
                }
            }
        };

        _ = animation.RunAsync(transform);

        _positions[control] = current;
    }
}
