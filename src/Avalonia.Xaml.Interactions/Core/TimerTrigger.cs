using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A trigger that invokes its actions after a specified interval.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class TimerTrigger : EventTriggerBehavior
{
    /// <summary>
    /// Identifies the <see cref="MillisecondsPerTick"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> MillisecondsPerTickProperty =
        AvaloniaProperty.Register<TimerTrigger, int>(nameof(MillisecondsPerTick), 1000);

    /// <summary>
    /// Identifies the <see cref="TotalTicks"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> TotalTicksProperty =
        AvaloniaProperty.Register<TimerTrigger, int>(nameof(TotalTicks), 1);

    /// <summary>
    /// Identifies the <see cref="RepeatForever"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> RepeatForeverProperty =
        AvaloniaProperty.Register<TimerTrigger, bool>(nameof(RepeatForever));

    private DispatcherTimer? _timer;
    private int _currentTick;

    /// <summary>
    /// Gets or sets the time, in milliseconds, between timer ticks.
    /// </summary>
    public int MillisecondsPerTick
    {
        get => GetValue(MillisecondsPerTickProperty);
        set => SetValue(MillisecondsPerTickProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of ticks after which the trigger stops firing.
    /// </summary>
    public int TotalTicks
    {
        get => GetValue(TotalTicksProperty);
        set => SetValue(TotalTicksProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the timer repeats indefinitely.
    /// </summary>
    public bool RepeatForever
    {
        get => GetValue(RepeatForeverProperty);
        set => SetValue(RepeatForeverProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        StartTimer();
    }

    /// <inheritdoc />
    protected override void OnEvent(object? eventArgs)
    {
        StopTimer();
        StartTimer();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        StopTimer();
        base.OnDetaching();
    }

    private void StartTimer()
    {
        if (_timer is not null)
        {
            return;
        }

        _currentTick = 0;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(MillisecondsPerTick)
        };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void StopTimer()
    {
        if (_timer is null)
        {
            return;
        }

        _timer.Tick -= OnTick;
        _timer.Stop();
        _timer = null;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _currentTick++;
        Execute(e);

        if (!RepeatForever && _currentTick >= TotalTicks)
        {
            StopTimer();
        }
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
