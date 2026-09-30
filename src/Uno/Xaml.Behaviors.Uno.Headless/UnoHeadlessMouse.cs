// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Windows.Foundation;
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
/// window coordinates, or relative to an element.
/// </summary>
/// <remarks>
/// Uno Platform applies injected mouse moves relative to the current position; this type tracks the position so moves
/// are absolute. Every injected event advances the event time, which the drag and drop manager requires. All members
/// must be called on the UI thread; each one runs the queued UI work afterwards.
/// </remarks>
public sealed class UnoHeadlessMouse
{
    private const uint FrameMilliseconds = 16;
    private const int WheelDelta = 120;

    private readonly UnoHeadlessSession _session;
    private InputInjector? _injector;
    private uint _time;

    internal UnoHeadlessMouse(UnoHeadlessSession session) => _session = session;

    /// <summary>
    /// Gets the current pointer position in window coordinates.
    /// </summary>
    public Point Position { get; private set; }

    /// <summary>
    /// Moves the pointer to <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The position, relative to <paramref name="relativeTo"/>.</param>
    /// <param name="relativeTo">The element the position is relative to, or <see langword="null"/> for the window.</param>
    public void MoveTo(Point position, UIElement? relativeTo = null)
    {
        var target = relativeTo is null ? position : relativeTo.TransformToVisual(null).TransformPoint(position);
        var deltaX = (int)Math.Round(target.X - Position.X);
        var deltaY = (int)Math.Round(target.Y - Position.Y);
        Position = new Point(Position.X + deltaX, Position.Y + deltaY);
        Inject(new InjectedInputMouseInfo { DeltaX = deltaX, DeltaY = deltaY, MouseOptions = InjectedInputMouseOptions.Move });
    }

    /// <summary>
    /// Presses <paramref name="button"/> at the current position.
    /// </summary>
    /// <param name="button">The button.</param>
    public void Down(UnoHeadlessMouseButton button = UnoHeadlessMouseButton.Left)
        => Inject(new InjectedInputMouseInfo { MouseOptions = button switch
        {
            UnoHeadlessMouseButton.Right => InjectedInputMouseOptions.RightDown,
            UnoHeadlessMouseButton.Middle => InjectedInputMouseOptions.MiddleDown,
            _ => InjectedInputMouseOptions.LeftDown,
        } });

    /// <summary>
    /// Releases <paramref name="button"/> at the current position.
    /// </summary>
    /// <param name="button">The button.</param>
    public void Up(UnoHeadlessMouseButton button = UnoHeadlessMouseButton.Left)
        => Inject(new InjectedInputMouseInfo { MouseOptions = button switch
        {
            UnoHeadlessMouseButton.Right => InjectedInputMouseOptions.RightUp,
            UnoHeadlessMouseButton.Middle => InjectedInputMouseOptions.MiddleUp,
            _ => InjectedInputMouseOptions.LeftUp,
        } });

    /// <summary>
    /// Moves to <paramref name="position"/>, then presses and releases <paramref name="button"/>.
    /// </summary>
    /// <param name="relativeTo">The element the position is relative to.</param>
    /// <param name="position">The position; the center of the element when <see langword="null"/>.</param>
    /// <param name="button">The button.</param>
    public void Click(FrameworkElement relativeTo, Point? position = null, UnoHeadlessMouseButton button = UnoHeadlessMouseButton.Left)
    {
        ArgumentNullException.ThrowIfNull(relativeTo);
        MoveTo(position ?? new Point(relativeTo.ActualWidth / 2, relativeTo.ActualHeight / 2), relativeTo);
        Down(button);
        Up(button);
    }

    /// <summary>
    /// Rotates the wheel at the current position.
    /// </summary>
    /// <param name="notches">The number of notches; positive values scroll up.</param>
    public void Wheel(int notches)
        => Inject(new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.Wheel, MouseData = unchecked((uint)(notches * WheelDelta)) });

    private void Inject(InjectedInputMouseInfo info)
    {
        _session.EnsureThreadAccess();
        _injector ??= InputInjector.TryCreate() ?? throw new InvalidOperationException("Input injection is not available.");
        _time += FrameMilliseconds;
        info.TimeOffsetInMilliseconds = _time;
        _injector.InjectMouseInput([info]);
        _session.RunJobs();
    }
}
