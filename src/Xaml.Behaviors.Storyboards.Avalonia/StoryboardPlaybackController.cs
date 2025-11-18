// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using AnimationTimeline = Avalonia.Animation.Animation;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Manages the lifetime of a storyboard animation, exposing pause/resume/seek controls.
/// </summary>
internal sealed class StoryboardPlaybackController : IDisposable
{
    private readonly IAnimation _animation;
    private readonly Animatable _target;
    private readonly StoryboardClockController _clock;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly object _gate = new();
    private readonly Task _runTask;
    private bool _isDisposed;

    [RequiresUnreferencedCode("Storyboard animation helpers rely on Avalonia's animation internals.")]
    public StoryboardPlaybackController(IAnimation animation, Animatable target)
    {
        _animation = animation ?? throw new ArgumentNullException(nameof(animation));
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _clock = StoryboardAnimationHelpers.CreateClockController();
        _clock.Resume();
        _runTask = StoryboardAnimationHelpers.RunAnimationAsync(_animation, _target, _clock.Clock, _cancellation.Token);
    }

    /// <summary>
    /// Raised when the animation task completes.
    /// </summary>
    public event EventHandler? Completed;

    /// <summary>
    /// Gets the task representing the running animation.
    /// </summary>
    public Task RunTask => _runTask;

    /// <summary>
    /// Pauses the playback clock.
    /// </summary>
    public void Pause()
    {
        lock (_gate)
        {
            if (_isDisposed)
            {
                return;
            }

            _clock.Pause();
        }
    }

    /// <summary>
    /// Resumes the playback clock.
    /// </summary>
    public void Resume()
    {
        lock (_gate)
        {
            if (_isDisposed)
            {
                return;
            }

            _clock.Resume();
        }
    }

    /// <summary>
    /// Stops playback and cancels the running animation.
    /// </summary>
    public void Stop()
    {
        lock (_gate)
        {
            if (_isDisposed)
            {
                return;
            }

            _clock.Stop();
            _cancellation.Cancel();
        }
    }

    /// <summary>
    /// Seeks to the specified offset based on the seek origin.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="origin">The origin used to interpret the offset.</param>
    public void Seek(TimeSpan offset, SeekOrigin origin)
    {
        lock (_gate)
        {
            if (_isDisposed)
            {
                return;
            }

            var duration = GetTotalDuration();
            var target = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => SafeAddTimeSpan(_clock.CurrentTime, offset),
                SeekOrigin.End when duration != TimeSpan.MaxValue => SafeAddTimeSpan(duration, offset),
                SeekOrigin.End => throw new InvalidOperationException("SeekOrigin.End is not supported when the storyboard repeats indefinitely."),
                _ => throw new ArgumentOutOfRangeException(nameof(origin))
            };

            if (target < TimeSpan.Zero)
            {
                target = TimeSpan.Zero;
            }
            else if (duration != TimeSpan.MaxValue && target > duration)
            {
                target = duration;
            }

            EnsureSeekTargetWithinRange(target);

            _clock.Seek(target);
        }
    }

    /// <summary>
    /// Seeks to the end of the animation (fill state).
    /// </summary>
    public void SkipToFill()
    {
        var duration = GetTotalDuration();
        if (duration == TimeSpan.MaxValue)
        {
            var fillTarget = GetFillTarget();
            if (fillTarget <= TimeSpan.Zero)
            {
                return;
            }

            Seek(fillTarget, SeekOrigin.Begin);
            return;
        }

        Seek(duration, SeekOrigin.Begin);
    }

    /// <summary>
    /// Adjusts the animation speed ratio for Avalonia keyframe animations.
    /// </summary>
    /// <param name="ratio">The new ratio.</param>
    public void SetSpeedRatio(double ratio)
    {
        if (ratio <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ratio), "Speed ratio must be positive.");
        }

        lock (_gate)
        {
            if (_isDisposed)
            {
                return;
            }

            if (_animation is AnimationTimeline keyframeAnimation)
            {
                keyframeAnimation.SpeedRatio = ratio;
            }
            else
            {
                throw new InvalidOperationException("Speed ratio adjustments are only supported for Avalonia Animation instances.");
            }
        }
    }

    /// <summary>
    /// Registers a continuation that fires when the animation finishes.
    /// </summary>
    public void WatchForCompletion()
    {
        _ = _runTask.ContinueWith(
            static (task, state) =>
            {
                var controller = (StoryboardPlaybackController)state!;
                controller.OnCompleted(task);
            },
            this,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_gate)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _cancellation.Cancel();
            _clock.Stop();
        }
    }

    private void OnCompleted(Task task)
    {
        if (task.IsFaulted)
        {
            // Surface exceptions to the caller.
            task.GetAwaiter().GetResult();
        }

        Completed?.Invoke(this, EventArgs.Empty);
    }

    private TimeSpan GetTotalDuration()
    {
        if (_animation is not AnimationTimeline animation)
        {
            return TimeSpan.MaxValue;
        }

        var duration = animation.Duration;

        if (animation.IterationCount.RepeatType == IterationType.Infinite)
        {
            return TimeSpan.MaxValue;
        }

        var iterations = Math.Max(animation.IterationCount.Value, 1UL);
        var totalTicks = SafeMultiply(duration.Ticks, iterations);

        if (iterations > 1 && animation.DelayBetweenIterations > TimeSpan.Zero)
        {
            totalTicks = SafeAdd(totalTicks, SafeMultiply(animation.DelayBetweenIterations.Ticks, iterations - 1));
        }

        if (animation.Delay > TimeSpan.Zero)
        {
            totalTicks = SafeAdd(totalTicks, animation.Delay.Ticks);
        }

        return new TimeSpan(totalTicks);
    }

    private TimeSpan GetFillTarget()
    {
        if (_animation is not AnimationTimeline animation)
        {
            return TimeSpan.Zero;
        }

        var ticks = animation.Duration.Ticks;
        if (animation.Delay > TimeSpan.Zero)
        {
            ticks = SafeAdd(ticks, animation.Delay.Ticks);
        }

        return ticks > 0 ? new TimeSpan(ticks) : TimeSpan.Zero;
    }

    private static long SafeMultiply(long value, ulong multiplier)
    {
        try
        {
            checked
            {
                return value * (long)multiplier;
            }
        }
        catch (OverflowException)
        {
            return long.MaxValue;
        }
    }

    private static long SafeAdd(long first, long second)
    {
        try
        {
            checked
            {
                return first + second;
            }
        }
        catch (OverflowException)
        {
            return long.MaxValue;
        }
    }

    private static TimeSpan SafeAddTimeSpan(TimeSpan first, TimeSpan second)
    {
        try
        {
            checked
            {
                return first + second;
            }
        }
        catch (OverflowException ex)
        {
            throw new InvalidOperationException("Seek offset exceeds the supported range for storyboard playback.", ex);
        }
    }

    private static readonly TimeSpan MaxSupportedSeekTarget = TimeSpan.FromDays(365_000); // ~1000 years.

    private static void EnsureSeekTargetWithinRange(TimeSpan target)
    {
        if (target >= MaxSupportedSeekTarget)
        {
            throw new InvalidOperationException("Seek target is too large. Storyboard clocks cannot represent positions beyond 1,000 years.");
        }
    }
}
