using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Custom;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Animates elements to smoothly transition to their new layout positions.
/// </summary>
public class FluidMoveBehavior : AttachedToVisualTreeBehavior<Panel>
{
    /// <summary>
    /// Defines whether the animation applies to the panel itself or its children.
    /// </summary>
    public enum AppliesTo
    {
        /// <summary>
        /// Animate the associated panel.
        /// </summary>
        Self,

        /// <summary>
        /// Animate the immediate children of the panel.
        /// </summary>
        Children
    }

    /// <summary>
    /// Identifies the <see cref="Duration"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<FluidMoveBehavior, TimeSpan>(nameof(Duration),
            TimeSpan.FromMilliseconds(250));

    /// <summary>
    /// Identifies the <see cref="Easing"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Easing?> EasingProperty =
        AvaloniaProperty.Register<FluidMoveBehavior, Easing?>(nameof(Easing));

    /// <summary>
    /// Identifies the <see cref="Scope"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AppliesTo> ScopeProperty =
        AvaloniaProperty.Register<FluidMoveBehavior, AppliesTo>(nameof(Scope), AppliesTo.Children);

    /// <summary>
    /// Gets or sets the duration of the animation.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the easing of the animation.
    /// </summary>
    public Easing? Easing
    {
        get => GetValue(EasingProperty);
        set => SetValue(EasingProperty, value);
    }

    /// <summary>
    /// Gets or sets the scope of the animation.
    /// </summary>
    public AppliesTo Scope
    {
        get => GetValue(ScopeProperty);
        set => SetValue(ScopeProperty, value);
    }

    private readonly Dictionary<object, Rect> _bounds = new();

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is { } panel)
        {
            panel.LayoutUpdated += PanelOnLayoutUpdated;

            foreach (var child in panel.Children)
            {
                if (child is Visual visual)
                {
                    _bounds[GetKey(visual)] = visual.Bounds;
                }
            }

            return DisposableAction.Create(() =>
            {
                panel.LayoutUpdated -= PanelOnLayoutUpdated;
                _bounds.Clear();
            });
        }

        return DisposableAction.Empty;
    }

    private void PanelOnLayoutUpdated(object? sender, EventArgs e)
    {
        if (AssociatedObject is not Panel panel)
        {
            return;
        }

        IEnumerable<Visual> elements = Scope == AppliesTo.Self
            ? new[] { panel }
            : panel.Children;

        foreach (var element in elements)
        {
            var key = GetKey(element);
            var newBounds = element.Bounds;

            if (_bounds.TryGetValue(key, out var oldBounds))
            {
                if (oldBounds.Position != newBounds.Position)
                {
                    var dx = oldBounds.X - newBounds.X;
                    var dy = oldBounds.Y - newBounds.Y;
                    StartAnimation(element, dx, dy);
                }

                _bounds[key] = newBounds;
            }
            else
            {
                _bounds.Add(key, newBounds);
            }
        }
    }

    private static object GetKey(Visual element) => (element as Control)?.Tag ?? element;

    private void StartAnimation(Visual element, double dx, double dy)
    {
        if (Math.Abs(dx) < double.Epsilon && Math.Abs(dy) < double.Epsilon)
        {
            return;
        }

        var transform = new TranslateTransform(dx, dy);
        element.RenderTransform = transform;

        var animation = new Animation.Animation
        {
            Duration = Duration,
            Children =
            {
                new KeyFrame
                {
                    KeyTime = TimeSpan.Zero,
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, dx),
                        new Setter(TranslateTransform.YProperty, dy)
                    }
                },
                new KeyFrame
                {
                    KeyTime = Duration,
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d),
                        new Setter(TranslateTransform.YProperty, 0d)
                    },
                    Easing = Easing
                }
            }
        };

        _ = animation.RunAsync(transform);
    }
}
