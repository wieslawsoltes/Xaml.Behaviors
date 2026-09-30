// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Microsoft.UI.Xaml;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// A trigger that invokes its actions after a specified interval.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class TimerTrigger : EventTriggerBase
{

    private DispatcherTimer? _timer;
    private int _currentTick;

    /// <summary>
    /// Gets or sets the time, in milliseconds, between timer ticks.
    /// </summary>
    [StyledProperty(DefaultValue = 1000)]
    public partial int MillisecondsPerTick { get; set; }

    /// <summary>
    /// Gets or sets the number of ticks after which the trigger stops firing.
    /// </summary>
    [StyledProperty(DefaultValue = 1)]
    public partial int TotalTicks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the timer repeats indefinitely.
    /// </summary>
    [StyledProperty]
    public partial bool RepeatForever { get; set; }

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

#if UNO
    private void OnTick(object? sender, object e)
#else
    private void OnTick(object? sender, EventArgs e)
#endif
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
