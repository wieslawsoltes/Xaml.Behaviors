// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Uno.UI.Runtime.Skia.Headless;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input.Preview.Injection;

namespace Xaml.Behaviors.Uno.Headless;

/// <summary>
/// The mouse buttons of <see cref="UnoHeadlessMouse"/>.
/// </summary>
public enum UnoHeadlessMouseButton
{
    /// <summary>The left button.</summary>
    Left,

    /// <summary>The right button.</summary>
    Right,

    /// <summary>The middle button.</summary>
    Middle,
}

/// <summary>
/// Mouse input of a <see cref="UnoHeadlessSession"/>, injected through <see cref="InputInjector"/>. Positions are in
/// window coordinates, or relative to an element. The events carry the modifier keys held on
/// <see cref="UnoHeadlessSession.Keyboard"/> (<see cref="UnoHeadlessKeyboard.Modifiers"/>) and the modifiers passed to
/// the members (<c>PointerRoutedEventArgs.KeyModifiers</c>).
/// </summary>
/// <remarks>
/// Uno Platform applies injected mouse moves relative to the current position; this type tracks the position so moves
/// are absolute. Every injected event advances the event time by one frame (16 ms; Uno Platform adds the time offset of
/// an injected event to the time of the previous one), which the drag and drop manager and the gesture recognizers
/// require; <see cref="Wait"/> lets more time pass, and showing new content starts a new input sequence. All members
/// must be called on the UI thread; each one runs the queued UI work afterwards.
/// </remarks>
public sealed class UnoHeadlessMouse
{
    private const uint FrameMilliseconds = 16;
    private const int WheelDelta = 120;
    private const uint SequenceGapMilliseconds = 1000;

    private readonly UnoHeadlessSession _session;
    private readonly HeadlessHost _host;
    private InputInjector? _injector;
    private uint _idleTime;

    internal UnoHeadlessMouse(UnoHeadlessSession session, HeadlessHost host)
    {
        _session = session;
        _host = host;
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
        var target = relativeTo is null ? position : relativeTo.TransformToVisual(null).TransformPoint(position);
        var deltaX = (int)Math.Round(target.X - Position.X);
        var deltaY = (int)Math.Round(target.Y - Position.Y);
        Position = new Point(Position.X + deltaX, Position.Y + deltaY);
        Inject(new InjectedInputMouseInfo { DeltaX = deltaX, DeltaY = deltaY, MouseOptions = InjectedInputMouseOptions.Move }, modifiers);
    }

    /// <summary>
    /// Presses <paramref name="button"/> at the current position.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Down(UnoHeadlessMouseButton button = UnoHeadlessMouseButton.Left, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        => Inject(new InjectedInputMouseInfo { MouseOptions = button switch
        {
            UnoHeadlessMouseButton.Right => InjectedInputMouseOptions.RightDown,
            UnoHeadlessMouseButton.Middle => InjectedInputMouseOptions.MiddleDown,
            _ => InjectedInputMouseOptions.LeftDown,
        } }, modifiers);

    /// <summary>
    /// Releases <paramref name="button"/> at the current position.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Up(UnoHeadlessMouseButton button = UnoHeadlessMouseButton.Left, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        => Inject(new InjectedInputMouseInfo { MouseOptions = button switch
        {
            UnoHeadlessMouseButton.Right => InjectedInputMouseOptions.RightUp,
            UnoHeadlessMouseButton.Middle => InjectedInputMouseOptions.MiddleUp,
            _ => InjectedInputMouseOptions.LeftUp,
        } }, modifiers);

    /// <summary>
    /// Moves to <paramref name="position"/>, then presses and releases <paramref name="button"/>.
    /// </summary>
    /// <param name="relativeTo">The element the position is relative to.</param>
    /// <param name="position">The position; the center of the element when <see langword="null"/>.</param>
    /// <param name="button">The button.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Click(FrameworkElement relativeTo, Point? position = null, UnoHeadlessMouseButton button = UnoHeadlessMouseButton.Left, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        ArgumentNullException.ThrowIfNull(relativeTo);
        MoveTo(position ?? new Point(relativeTo.ActualWidth / 2, relativeTo.ActualHeight / 2), relativeTo, modifiers);
        Down(button, modifiers);
        Up(button, modifiers);
    }

    /// <summary>
    /// Rotates the vertical wheel at the current position (<c>PointerWheelChanged</c>, a mouse wheel delta of 120 per
    /// notch).
    /// </summary>
    /// <param name="notches">The number of notches; positive values scroll up.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void Wheel(int notches, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        => Inject(new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.Wheel, MouseData = unchecked((uint)(notches * WheelDelta)) }, modifiers);

    /// <summary>
    /// Rotates the horizontal wheel at the current position (<c>PointerWheelChanged</c> with
    /// <c>IsHorizontalMouseWheel</c>, a mouse wheel delta of 120 per notch).
    /// </summary>
    /// <param name="notches">The number of notches; positive values scroll right.</param>
    /// <param name="modifiers">The modifier keys held, in addition to the ones held on the keyboard.</param>
    public void HorizontalWheel(int notches, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        => Inject(new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.HWheel, MouseData = unchecked((uint)(notches * WheelDelta)) }, modifiers);

    /// <summary>
    /// Lets time pass without input: the next injected event is <paramref name="duration"/> later than the previous
    /// one, so it does not continue a gesture of the previous input (for example a double tap of two clicks made by
    /// different tests at the same position).
    /// </summary>
    /// <param name="duration">The time without input.</param>
    public void Wait(TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);
        _session.EnsureThreadAccess();
        _idleTime = checked(_idleTime + (uint)Math.Ceiling(duration.TotalMilliseconds));
    }

    /// <summary>
    /// Starts a new input sequence (new content was shown): the next event is timed well after the previous ones, so the
    /// gesture recognizers do not combine taps on the new content with earlier taps (for example into a double tap).
    /// </summary>
    internal void StartNewSequence() => _idleTime = Math.Max(_idleTime, SequenceGapMilliseconds);

    private void Inject(InjectedInputMouseInfo info, VirtualKeyModifiers modifiers)
    {
        _session.EnsureThreadAccess();
        _injector ??= InputInjector.TryCreate() ?? throw new InvalidOperationException("Input injection is not available.");
        // The offset is relative to the previous injected event (Uno Platform adds it to the previous timestamp).
        info.TimeOffsetInMilliseconds = FrameMilliseconds + _idleTime;
        _idleTime = 0;
        _host.InjectMouseInput(_injector, info, modifiers | _session.Keyboard.Modifiers);
        _session.RunJobs();
    }
}
