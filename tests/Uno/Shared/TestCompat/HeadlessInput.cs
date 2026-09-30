// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.System;
using Xaml.Behaviors.Uno.Headless;
#if UNO_TESTS_INTERACTIVITY
using MouseButton = Xaml.Interactivity.MouseButton;
#endif

namespace Xaml.Behaviors.Uno.TestCompat;

/// <summary>
/// The Avalonia <c>RawInputModifiers</c> used by the shared tests.
/// </summary>
[Flags]
public enum RawInputModifiers
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Meta = 8,
    LeftMouseButton = 16,
    RightMouseButton = 32,
    MiddleMouseButton = 64,
}

#if !UNO_TESTS_INTERACTIVITY
/// <summary>
/// The Avalonia <c>MouseButton</c> used by the shared tests (Xaml.Behaviors.Uno.Interactivity provides it otherwise).
/// </summary>
internal enum MouseButton
{
    None,
    Left,
    Right,
    Middle,
    XButton1,
    XButton2,
}
#endif

/// <summary>
/// A rendered frame (Avalonia <c>CaptureRenderedFrame</c>); the Uno headless host renders no pixels.
/// </summary>
public sealed class RenderedFrame
{
    /// <summary>Does nothing: there is no image to save.</summary>
    public void Save(string fileName)
    {
    }
}

/// <summary>
/// Uno Platform counterparts of the Avalonia headless input and rendering extensions (<c>Avalonia.Headless</c>) used
/// by the shared tests. Input goes through <see cref="UnoHeadlessSession.Keyboard"/> and <see cref="UnoHeadlessSession.Mouse"/>;
/// every member runs the queued UI work before returning.
/// </summary>
internal static class HeadlessInput
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    /// <summary>
    /// Runs the queued UI work and the layout (Avalonia renders a frame); no image is produced.
    /// </summary>
    public static RenderedFrame? CaptureRenderedFrame(this UIElement topLevel)
    {
        Session.RunJobs();
        topLevel.UpdateLayout();
        Session.RunJobs();
        return null;
    }

    /// <summary>
    /// Presses and releases a key on the focused element (QWERTY layout).
    /// </summary>
    /// <remarks>
    /// Avalonia's <c>KeyPressQwerty</c> only raises the key down event and the shared tests never release the key:
    /// Avalonia buttons click on key down, WinUI (Uno Platform) buttons on key up, so the key is pressed and released.
    /// </remarks>
    public static void KeyPressQwerty(this UIElement topLevel, PhysicalKey key, RawInputModifiers modifiers)
    {
        var (virtualKey, character) = key.ToVirtualKey(modifiers);
        Session.Keyboard.KeyDown(virtualKey, ToKeyModifiers(modifiers), character);
        Session.Keyboard.KeyUp(virtualKey, ToKeyModifiers(modifiers));
        Session.RunJobs();
    }

    /// <summary>Releases a key on the focused element (QWERTY layout).</summary>
    public static void KeyReleaseQwerty(this UIElement topLevel, PhysicalKey key, RawInputModifiers modifiers)
    {
        var (virtualKey, _) = key.ToVirtualKey(modifiers);
        Session.Keyboard.KeyUp(virtualKey, ToKeyModifiers(modifiers));
        Session.RunJobs();
    }

    /// <summary>Types text into the focused element.</summary>
    public static void KeyTextInput(this UIElement topLevel, string text) => Session.Keyboard.TypeText(text);

    /// <summary>Moves the mouse to a position relative to <paramref name="topLevel"/>.</summary>
    public static void MouseMove(this UIElement topLevel, Point point, RawInputModifiers modifiers = RawInputModifiers.None)
        => Session.Mouse.MoveTo(point, topLevel);

    /// <summary>Moves the mouse to a position relative to <paramref name="topLevel"/> and presses a button.</summary>
    public static void MouseDown(this UIElement topLevel, Point point, MouseButton button, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        Session.Mouse.MoveTo(point, topLevel);
        Session.Mouse.Down(ToButton(button));
    }

    /// <summary>Moves the mouse to a position relative to <paramref name="topLevel"/> and releases a button.</summary>
    public static void MouseUp(this UIElement topLevel, Point point, MouseButton button, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        Session.Mouse.MoveTo(point, topLevel);
        Session.Mouse.Up(ToButton(button));
    }

    /// <summary>Rotates the mouse wheel at a position relative to <paramref name="topLevel"/>.</summary>
    public static void MouseWheel(this UIElement topLevel, Point point, Vector delta, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        Session.Mouse.MoveTo(point, topLevel);
        Session.Mouse.Wheel((int)Math.Round(delta.Y));
    }

    /// <summary>Finds a named element (Avalonia <c>FindControl</c>).</summary>
    public static T? FindControl<T>(this FrameworkElement element, string name)
        where T : class
        => element.FindName(name) as T;

    private static UnoHeadlessMouseButton ToButton(MouseButton button) => button switch
    {
        MouseButton.Right => UnoHeadlessMouseButton.Right,
        MouseButton.Middle => UnoHeadlessMouseButton.Middle,
        _ => UnoHeadlessMouseButton.Left,
    };

    private static VirtualKeyModifiers ToKeyModifiers(RawInputModifiers modifiers)
    {
        var result = VirtualKeyModifiers.None;
        if (modifiers.HasFlag(RawInputModifiers.Alt))
        {
            result |= VirtualKeyModifiers.Menu;
        }

        if (modifiers.HasFlag(RawInputModifiers.Control))
        {
            result |= VirtualKeyModifiers.Control;
        }

        if (modifiers.HasFlag(RawInputModifiers.Shift))
        {
            result |= VirtualKeyModifiers.Shift;
        }

        if (modifiers.HasFlag(RawInputModifiers.Meta))
        {
            result |= VirtualKeyModifiers.Windows;
        }

        return result;
    }
}
