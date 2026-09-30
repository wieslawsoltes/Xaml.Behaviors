// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Xaml.Behaviors.Uno.Headless;
#if UNO_TESTS_INTERACTIVITY
using MouseButton = Xaml.Interactivity.MouseButton;
#endif

namespace Xaml.Behaviors.Uno.TestCompat;

/// <summary>
/// Mouse input at a position relative to an element, like the <c>HeadlessWindowExtensions</c> helpers of the Avalonia
/// test projects (<c>window.MouseDown(element, point, button)</c>) used by the drag and drop tests.
/// </summary>
/// <remarks>
/// The pointer moves through <see cref="UnoHeadlessSession.Mouse"/>, so a press followed by moves starts a real WinUI
/// drag operation when the pressed element is a drag source. A helper declared in the namespace of a test takes
/// precedence over these (extension methods of enclosing namespaces are found before the global usings).
/// </remarks>
internal static class DragAndDropTestCompat
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    /// <summary>Moves the mouse to <paramref name="point"/> relative to <paramref name="relativeTo"/> and presses a button.</summary>
    public static void MouseDown(this UIElement topLevel, UIElement relativeTo, Point point, MouseButton button, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        Session.Mouse.MoveTo(point, relativeTo);
        Session.Mouse.Down(ToButton(button));
    }

    /// <summary>Moves the mouse to <paramref name="point"/> relative to <paramref name="relativeTo"/>.</summary>
    public static void MouseMove(this UIElement topLevel, UIElement relativeTo, Point point, RawInputModifiers modifiers = RawInputModifiers.None)
        => Session.Mouse.MoveTo(point, relativeTo);

    /// <summary>Moves the mouse to <paramref name="point"/> relative to <paramref name="relativeTo"/> and releases a button.</summary>
    public static void MouseUp(this UIElement topLevel, UIElement relativeTo, Point point, MouseButton button, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        Session.Mouse.MoveTo(point, relativeTo);
        Session.Mouse.Up(ToButton(button));
    }

    private static UnoHeadlessMouseButton ToButton(MouseButton button) => button switch
    {
        MouseButton.Right => UnoHeadlessMouseButton.Right,
        MouseButton.Middle => UnoHeadlessMouseButton.Middle,
        _ => UnoHeadlessMouseButton.Left,
    };
}
