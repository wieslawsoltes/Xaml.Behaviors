using System;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Delays the visibility of the associated control when it is attached to the visual tree.
/// </summary>
public sealed class DelayedLoadBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="Delay"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DelayProperty =
        AvaloniaProperty.Register<DelayedLoadBehavior, TimeSpan>(nameof(Delay), TimeSpan.FromMilliseconds(500));

    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the delay before the control becomes visible.
    /// </summary>
    public TimeSpan Delay
    {
        get => GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        AssociatedObject.IsVisible = false;

        _timer = new DispatcherTimer { Interval = Delay };
        _timer.Tick += OnTick;
        _timer.Start();

        return new DisposableAction(DisposeTimer);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.SetCurrentValue(Visual.IsVisibleProperty, true);
        }
        DisposeTimer();
    }

    private void DisposeTimer()
    {
        if (_timer is not null)
        {
            _timer.Tick -= OnTick;
            _timer.Stop();
            _timer = null;
        }
    }
}
