// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.System;
using Xaml.Behaviors.WinUI.Testing.Internal;

namespace Xaml.Behaviors.WinUI.Testing;

/// <summary>
/// The mouse buttons of <see cref="WinUITestMouse"/>.
/// </summary>
public enum WinUITestMouseButton
{
    /// <summary>The left button.</summary>
    Left,

    /// <summary>The right button.</summary>
    Right,

    /// <summary>The middle button.</summary>
    Middle,
}

/// <summary>
/// Injects mouse input into the test window of the session (<see cref="WinUITestSession.Mouse"/>). Positions are in
/// window coordinates (device independent pixels relative to the window content), or relative to an element.
/// </summary>
/// <remarks>
/// The input goes through the operating system to the test window, which is brought to the foreground first; the
/// modifier keys are held on the keyboard around each event. Each member processes the messages of the UI thread
/// afterwards, so the routed events have been raised when it returns. <see cref="Wait"/> and showing new content delay
/// the next event, so it does not continue a gesture of the previous input (for example into a double tap).
/// </remarks>
public sealed class WinUITestMouse
{
    private const int WheelDelta = 120;
    private readonly WinUITestSession _session;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private TimeSpan _lastInput = TimeSpan.MinValue;
    private TimeSpan _pendingIdle;
    private readonly System.Collections.Generic.HashSet<WinUITestMouseButton> _held = [];

    internal WinUITestMouse(WinUITestSession session)
    {
        _session = session;
    }

    /// <summary>
    /// Gets the current pointer position in window coordinates.
    /// </summary>
    public Point Position { get; private set; }

    /// <summary>
    /// Moves the pointer to <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The position, relative to <paramref name="relativeTo"/>.</param>
    /// <param name="relativeTo">The element the position is relative to, or <see langword="null"/> for the window.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void MoveTo(Point position, UIElement? relativeTo = null, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        _session.EnsureThreadAccess();
        var target = relativeTo is null ? position : relativeTo.TransformToVisual(null).TransformPoint(position);
        Position = target;
        var (x, y) = ToScreen(target);
        _session.EnsureWindowAt(x, y);
        Inject(() => InputInjector.MoveTo(x, y), modifiers);
    }

    /// <summary>
    /// Presses <paramref name="button"/> at the current position.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Down(WinUITestMouseButton button = WinUITestMouseButton.Left, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        _held.Add(button);
        Inject(() => InputInjector.Button(button switch
        {
            WinUITestMouseButton.Right => NativeMethods.MOUSEEVENTF_RIGHTDOWN,
            WinUITestMouseButton.Middle => NativeMethods.MOUSEEVENTF_MIDDLEDOWN,
            _ => NativeMethods.MOUSEEVENTF_LEFTDOWN,
        }), modifiers);
    }

    /// <summary>
    /// Releases <paramref name="button"/> at the current position.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Up(WinUITestMouseButton button = WinUITestMouseButton.Left, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        _held.Remove(button);
        Inject(() => InputInjector.Button(button switch
        {
            WinUITestMouseButton.Right => NativeMethods.MOUSEEVENTF_RIGHTUP,
            WinUITestMouseButton.Middle => NativeMethods.MOUSEEVENTF_MIDDLEUP,
            _ => NativeMethods.MOUSEEVENTF_LEFTUP,
        }), modifiers);
    }

    /// <summary>
    /// Moves to <paramref name="position"/>, then presses and releases <paramref name="button"/>.
    /// </summary>
    /// <param name="relativeTo">The element the position is relative to.</param>
    /// <param name="position">The position; the center of the element when <see langword="null"/>.</param>
    /// <param name="button">The button.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Click(FrameworkElement relativeTo, Point? position = null, WinUITestMouseButton button = WinUITestMouseButton.Left, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        ArgumentNullException.ThrowIfNull(relativeTo);
        MoveTo(position ?? new Point(relativeTo.ActualWidth / 2, relativeTo.ActualHeight / 2), relativeTo, modifiers);
        Down(button, modifiers);
        Up(button, modifiers);
    }

    /// <summary>
    /// Rotates the vertical wheel at the current position (a mouse wheel delta of 120 per notch).
    /// </summary>
    /// <param name="notches">The number of notches; positive values scroll up.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Wheel(int notches, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        => Inject(() => InputInjector.Wheel(notches * WheelDelta, horizontal: false), modifiers);

    /// <summary>
    /// Rotates the horizontal wheel at the current position (a mouse wheel delta of 120 per notch).
    /// </summary>
    /// <param name="notches">The number of notches; positive values scroll right.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void HorizontalWheel(int notches, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        => Inject(() => InputInjector.Wheel(notches * WheelDelta, horizontal: true), modifiers);

    /// <summary>
    /// Moves the pointer to <paramref name="position"/> without blocking the UI thread.
    /// </summary>
    /// <param name="position">The position, relative to <paramref name="relativeTo"/>.</param>
    /// <param name="relativeTo">The element the position is relative to, or <see langword="null"/> for the window.</param>
    /// <returns>A task completed once the input has been processed.</returns>
    /// <remarks>
    /// The asynchronous members inject the input from a background thread and let the UI thread run its own message
    /// loop until the input is processed, so they also drive a drag and drop operation, which runs a modal message loop
    /// on the UI thread (the synchronous members would wait inside it). Await them from the UI thread.
    /// </remarks>
    public Task MoveToAsync(Point position, UIElement? relativeTo = null)
    {
        _session.EnsureThreadAccess();
        var target = relativeTo is null ? position : relativeTo.TransformToVisual(null).TransformPoint(position);
        Position = target;
        var (x, y) = ToScreen(target);
        return InjectAsync(() => InputInjector.MoveTo(x, y));
    }

    /// <summary>
    /// Presses <paramref name="button"/> at the current position without blocking the UI thread.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <returns>A task completed once the input has been processed.</returns>
    public Task DownAsync(WinUITestMouseButton button = WinUITestMouseButton.Left)
    {
        _held.Add(button);
        return InjectAsync(() => InputInjector.Button(button switch
        {
            WinUITestMouseButton.Right => NativeMethods.MOUSEEVENTF_RIGHTDOWN,
            WinUITestMouseButton.Middle => NativeMethods.MOUSEEVENTF_MIDDLEDOWN,
            _ => NativeMethods.MOUSEEVENTF_LEFTDOWN,
        }));
    }

    /// <summary>
    /// Releases <paramref name="button"/> at the current position without blocking the UI thread.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <returns>A task completed once the input has been processed.</returns>
    public Task UpAsync(WinUITestMouseButton button = WinUITestMouseButton.Left)
    {
        _held.Remove(button);
        return InjectAsync(() => InputInjector.Button(button switch
        {
            WinUITestMouseButton.Right => NativeMethods.MOUSEEVENTF_RIGHTUP,
            WinUITestMouseButton.Middle => NativeMethods.MOUSEEVENTF_MIDDLEUP,
            _ => NativeMethods.MOUSEEVENTF_LEFTUP,
        }));
    }

    /// <summary>
    /// Lets time pass without input: the next injected event is at least <paramref name="duration"/> later than the
    /// previous one, so it does not continue a gesture of the previous input.
    /// </summary>
    /// <param name="duration">The time without input.</param>
    public void Wait(TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);
        _session.EnsureThreadAccess();
        _pendingIdle += duration;
    }

    /// <summary>
    /// Starts a new input sequence (new content was shown): the next event is delayed beyond the double click time.
    /// </summary>
    internal void StartNewSequence()
    {
        // A button a test left pressed would turn the moves on the new content into a drag.
        foreach (var button in new System.Collections.Generic.List<WinUITestMouseButton>(_held))
        {
            Up(button);
        }


        var gap = TimeSpan.FromMilliseconds(NativeMethods.GetDoubleClickTime() + 50);
        if (_pendingIdle < gap)
        {
            _pendingIdle = gap;
        }
    }

    private void Inject(Action inject, VirtualKeyModifiers modifiers)
    {
        _session.EnsureThreadAccess();
        _session.EnsureForeground();

        if (_pendingIdle > TimeSpan.Zero && _lastInput != TimeSpan.MinValue)
        {
            var remaining = _lastInput + _pendingIdle - _clock.Elapsed;
            if (remaining > TimeSpan.Zero)
            {
                _session.Pump(remaining);
            }
        }

        _pendingIdle = TimeSpan.Zero;

        // Windows reads the modifier keys when the window processes the event: hold them until then.
        _session.Keyboard.PressForPointer(modifiers, out var pressed);
        if (pressed != VirtualKeyModifiers.None)
        {
            _session.Pump(_session.Options.InputDelay);
        }

        try
        {
            inject();
            _lastInput = _clock.Elapsed;
            _session.Pump(_session.Options.InputDelay);
        }
        finally
        {
            if (pressed != VirtualKeyModifiers.None)
            {
                _session.Keyboard.ReleaseForPointer(pressed);
                _session.Pump(_session.Options.InputDelay);
            }
        }
    }

    private async Task InjectAsync(Action inject)
    {
        _session.EnsureThreadAccess();
        _session.EnsureForegroundAsyncSafe();

        if (_pendingIdle > TimeSpan.Zero && _lastInput != TimeSpan.MinValue)
        {
            var remaining = _lastInput + _pendingIdle - _clock.Elapsed;
            if (remaining > TimeSpan.Zero)
            {
                await Task.Delay(remaining);
            }
        }

        _pendingIdle = TimeSpan.Zero;
        await Task.Run(inject);
        _lastInput = _clock.Elapsed;
        await Task.Delay(_session.Options.InputDelay);
        await _session.WaitForIdleAsync();
    }

    private (int X, int Y) ToScreen(Point position)
    {
        var scale = _session.Window.Content?.XamlRoot?.RasterizationScale ?? 1.0;
        var origin = new NativeMethods.POINT();
        NativeMethods.ClientToScreen(_session.WindowHandle, ref origin);
        return ((int)Math.Round(origin.X + position.X * scale), (int)Math.Round(origin.Y + position.Y * scale));
    }
}
