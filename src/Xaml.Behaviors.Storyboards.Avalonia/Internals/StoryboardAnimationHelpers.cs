// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Provides reflection helpers used to control Avalonia animations at runtime.
/// </summary>
[RequiresUnreferencedCode("Storyboard animation helpers rely on Avalonia's internal animation types.")]
internal static class StoryboardAnimationHelpers
{
    private const BindingFlags NonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
    private static readonly Type AnimationType = typeof(Avalonia.Animation.Animation);

    [DynamicallyAccessedMembers(
        DynamicallyAccessedMemberTypes.PublicProperties |
        DynamicallyAccessedMemberTypes.NonPublicFields |
        DynamicallyAccessedMemberTypes.NonPublicMethods)]
    private static readonly Type? ClockBaseType = AnimationType.Assembly.GetType("Avalonia.Animation.ClockBase");

    [DynamicallyAccessedMembers(
        DynamicallyAccessedMemberTypes.PublicConstructors |
        DynamicallyAccessedMemberTypes.NonPublicMethods |
        DynamicallyAccessedMemberTypes.PublicProperties)]
    private static readonly Type? ClockType = AnimationType.Assembly.GetType("Avalonia.Animation.Clock");

    private static readonly Type? ClockInterfaceType = AnimationType.Assembly.GetType("Avalonia.Animation.IClock");
    private static readonly MethodInfo? RunAsyncWithClockMethod = AnimationType
        .GetMethods(NonPublicInstance)
        .FirstOrDefault(m =>
        {
            if (m.Name != "RunAsync")
            {
                return false;
            }

            var parameters = m.GetParameters();
            if (parameters.Length != 3)
            {
                return false;
            }

            return parameters[0].ParameterType == typeof(Animatable) &&
                   parameters[1].ParameterType == ClockInterfaceType &&
                   parameters[2].ParameterType == typeof(CancellationToken);
        });

    private static readonly PropertyInfo? PlayStateProperty = ClockBaseType?.GetProperty(nameof(StoryboardClockController.PlayState));
    private static readonly FieldInfo? InternalTimeField = ClockBaseType?.GetField("_internalTime", NonPublicInstance);
    private static readonly FieldInfo? PreviousTimeField = ClockBaseType?.GetField("_previousTime", NonPublicInstance);
    private static readonly MethodInfo? PulseMethod = ClockBaseType?.GetMethod("Pulse", NonPublicInstance);

    /// <summary>
    /// Creates a new controller that wraps an Avalonia internal clock instance.
    /// </summary>
    /// <returns>The created controller.</returns>
    public static StoryboardClockController CreateClockController()
    {
        EnsureReflectionState();

        var clock = Activator.CreateInstance(ClockType!);
        if (clock is null)
        {
            throw new InvalidOperationException("Failed to create Avalonia.Animation.Clock instance.");
        }

        return new StoryboardClockController(
            clock,
            PlayStateProperty!,
            InternalTimeField!,
            PreviousTimeField!,
            PulseMethod!);
    }

    /// <summary>
    /// Runs the specified animation by invoking the internal RunAsync overload that accepts a clock parameter.
    /// </summary>
    /// <param name="animation">The animation instance.</param>
    /// <param name="target">The target animatable.</param>
    /// <param name="clock">The clock passed to Avalonia.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the animation ends.</returns>
    public static Task RunAnimationAsync(
        IAnimation animation,
        Animatable target,
        object clock,
        CancellationToken cancellationToken)
    {
        EnsureReflectionState();

        if (animation is null)
        {
            throw new ArgumentNullException(nameof(animation));
        }

        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (clock is null)
        {
            throw new ArgumentNullException(nameof(clock));
        }

        if (RunAsyncWithClockMethod is null)
        {
            throw new InvalidOperationException("Unable to locate Avalonia.Animation.Animation.RunAsync overload.");
        }

        try
        {
            if (RunAsyncWithClockMethod.Invoke(animation, new[] { target, clock, cancellationToken }) is Task task)
            {
                return task;
            }
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }

        throw new InvalidOperationException("Avalonia animation failed to start because the invocation did not return a Task.");
    }

    private static void EnsureReflectionState()
    {
        if (ClockType is null || ClockBaseType is null || ClockInterfaceType is null ||
            PlayStateProperty is null || InternalTimeField is null || PreviousTimeField is null ||
            PulseMethod is null)
        {
            throw new InvalidOperationException("Avalonia animation internals are unavailable. Ensure the referenced Avalonia package is compatible.");
        }
    }
}

/// <summary>
/// Wraps an Avalonia animation clock instance to provide pause/resume/seek operations.
/// </summary>
internal sealed class StoryboardClockController
{
    private readonly object _clock;
    private readonly PropertyInfo _playStateProperty;
    private readonly FieldInfo _internalTimeField;
    private readonly FieldInfo _previousTimeField;
    private readonly MethodInfo _pulseMethod;

    public StoryboardClockController(
        object clock,
        PropertyInfo playStateProperty,
        FieldInfo internalTimeField,
        FieldInfo previousTimeField,
        MethodInfo pulseMethod)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _playStateProperty = playStateProperty ?? throw new ArgumentNullException(nameof(playStateProperty));
        _internalTimeField = internalTimeField ?? throw new ArgumentNullException(nameof(internalTimeField));
        _previousTimeField = previousTimeField ?? throw new ArgumentNullException(nameof(previousTimeField));
        _pulseMethod = pulseMethod ?? throw new ArgumentNullException(nameof(pulseMethod));
    }

    /// <summary>
    /// Gets the wrapped clock instance.
    /// </summary>
    public object Clock => _clock;

    /// <summary>
    /// Gets or sets the current play state.
    /// </summary>
    public PlayState PlayState
    {
        get => (PlayState)_playStateProperty.GetValue(_clock)!;
        set => _playStateProperty.SetValue(_clock, value);
    }

    /// <summary>
    /// Gets the current timeline position.
    /// </summary>
    public TimeSpan CurrentTime => (TimeSpan)_internalTimeField.GetValue(_clock)!;

    /// <summary>
    /// Pauses the clock.
    /// </summary>
    public void Pause() => PlayState = PlayState.Pause;

    /// <summary>
    /// Resumes the clock.
    /// </summary>
    public void Resume() => PlayState = PlayState.Run;

    /// <summary>
    /// Stops the clock.
    /// </summary>
    public void Stop() => PlayState = PlayState.Stop;

    /// <summary>
    /// Seeks the clock to the specified position.
    /// </summary>
    /// <param name="position">The desired timeline position.</param>
    public void Seek(TimeSpan position)
    {
        _internalTimeField.SetValue(_clock, position);
        var previous = (TimeSpan?)_previousTimeField.GetValue(_clock) ?? position;
        var originalState = PlayState;

        if (originalState == PlayState.Pause)
        {
            PlayState = PlayState.Run;
            _pulseMethod.Invoke(_clock, new object?[] { previous });
            PlayState = originalState;
        }
        else
        {
            _pulseMethod.Invoke(_clock, new object?[] { previous });
        }
    }
}
