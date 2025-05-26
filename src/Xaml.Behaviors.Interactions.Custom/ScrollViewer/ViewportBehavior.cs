using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Listens for the associated element entering or exiting the parent <see cref="ScrollViewer"/> viewport.
/// </summary>
public class ViewportBehavior : AttachedToVisualTreeBehavior<Visual>
{
    /// <summary>
    /// Identifies the <see cref="IsFullyInViewport"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsFullyInViewportProperty =
        AvaloniaProperty.Register<ViewportBehavior, bool>(nameof(IsFullyInViewport));

    /// <summary>
    /// Identifies the <see cref="IsInViewport"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsInViewportProperty =
        AvaloniaProperty.Register<ViewportBehavior, bool>(nameof(IsInViewport));

    /// <summary>
    /// Identifies the <see cref="IsAlwaysOn"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAlwaysOnProperty =
        AvaloniaProperty.Register<ViewportBehavior, bool>(nameof(IsAlwaysOn));

    /// <summary>
    /// Gets or sets a value indicating whether this behavior will remain attached after the associated element enters the viewport.
    /// When false, the behavior will remove itself after entering.
    /// </summary>
    public bool IsAlwaysOn
    {
        get => GetValue(IsAlwaysOnProperty);
        set => SetValue(IsAlwaysOnProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the associated element is fully in the <see cref="ScrollViewer"/> viewport.
    /// </summary>
    public bool IsFullyInViewport
    {
        get => GetValue(IsFullyInViewportProperty);
        private set => SetValue(IsFullyInViewportProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the associated element is in the <see cref="ScrollViewer"/> viewport.
    /// </summary>
    public bool IsInViewport
    {
        get => GetValue(IsInViewportProperty);
        private set => SetValue(IsInViewportProperty, value);
    }

    /// <summary>
    /// Occurs when the associated element has fully entered the viewport.
    /// </summary>
    public event EventHandler? EnteredViewport;

    /// <summary>
    /// Occurs when the associated element has fully exited the viewport.
    /// </summary>
    public event EventHandler? ExitedViewport;

    /// <summary>
    /// Occurs when the associated element starts entering the viewport.
    /// </summary>
    public event EventHandler? EnteringViewport;

    /// <summary>
    /// Occurs when the associated element starts exiting the viewport.
    /// </summary>
    public event EventHandler? ExitingViewport;

    private ScrollViewer? _hostScrollViewer;

    static ViewportBehavior()
    {
        IsFullyInViewportProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnIsFullyInViewportChanged));
        IsInViewportProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnIsInViewportChanged));
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        _hostScrollViewer = AssociatedObject?.FindAncestorOfType<ScrollViewer>();
        if (_hostScrollViewer is null)
        {
            throw new InvalidOperationException(
                "This behavior can only be attached to an element which has a ScrollViewer as a parent.");
        }

        _hostScrollViewer.ScrollChanged += OnScrollChanged;
        EvaluateViewportState();

        return DisposableAction.Create(() =>
        {
            _hostScrollViewer.ScrollChanged -= OnScrollChanged;
            _hostScrollViewer = null;
        });
    }

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        EvaluateViewportState();
    }

    private void EvaluateViewportState()
    {
        if (AssociatedObject is null || _hostScrollViewer is null)
        {
            return;
        }

        var point = AssociatedObject.TranslatePoint(new Point(0, 0), _hostScrollViewer);
        if (point is null)
        {
            return;
        }

        var elementRect = new Rect(point.Value, AssociatedObject.Bounds.Size);
        var hostRect = new Rect(0, 0, _hostScrollViewer.Bounds.Width, _hostScrollViewer.Bounds.Height);

        if (hostRect.Intersects(elementRect))
        {
            IsInViewport = true;
            IsFullyInViewport =
                hostRect.Contains(elementRect.TopLeft) &&
                hostRect.Contains(elementRect.TopRight) &&
                hostRect.Contains(elementRect.BottomRight) &&
                hostRect.Contains(elementRect.BottomLeft);
        }
        else
        {
            IsInViewport = false;
            IsFullyInViewport = false;
        }
    }

    private static void OnIsFullyInViewportChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is not ViewportBehavior obj)
        {
            return;
        }

        var value = e.NewValue.GetValueOrDefault();

        if (value)
        {
            obj.EnteredViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);

            if (!obj.IsAlwaysOn && obj.AssociatedObject is { } element)
            {
                Interaction.GetBehaviors(element).Remove(obj);
            }
        }
        else
        {
            obj.ExitingViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);
        }
    }

    private static void OnIsInViewportChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is not ViewportBehavior obj)
        {
            return;
        }

        var value = e.NewValue.GetValueOrDefault();

        if (value)
        {
            obj.EnteringViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);
        }
        else
        {
            obj.ExitedViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);
        }
    }
}
