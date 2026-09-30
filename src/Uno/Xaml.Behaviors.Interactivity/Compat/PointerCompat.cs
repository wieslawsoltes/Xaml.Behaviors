// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>MouseButton</c> enumeration.
/// </summary>
internal enum MouseButton
{
    /// <summary>No button.</summary>
    None,

    /// <summary>The left button.</summary>
    Left,

    /// <summary>The right button.</summary>
    Right,

    /// <summary>The middle button.</summary>
    Middle,

    /// <summary>The first extended button.</summary>
    XButton1,

    /// <summary>The second extended button.</summary>
    XButton2,
}

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>KeyModifiers</c> flags, expressed as WinUI
/// <see cref="VirtualKeyModifiers"/> so they compare with <see cref="PointerRoutedEventArgs.KeyModifiers"/>.
/// </summary>
internal static class KeyModifiers
{
    /// <summary>No modifier.</summary>
    public const VirtualKeyModifiers None = VirtualKeyModifiers.None;

    /// <summary>The Alt key (<c>Menu</c> on WinUI).</summary>
    public const VirtualKeyModifiers Alt = VirtualKeyModifiers.Menu;

    /// <summary>The Control key.</summary>
    public const VirtualKeyModifiers Control = VirtualKeyModifiers.Control;

    /// <summary>The Shift key.</summary>
    public const VirtualKeyModifiers Shift = VirtualKeyModifiers.Shift;

    /// <summary>The Meta key (<c>Windows</c> on WinUI).</summary>
    public const VirtualKeyModifiers Meta = VirtualKeyModifiers.Windows;
}

/// <summary>
/// Uno Platform counterparts of the Avalonia pointer event argument members used by the shared sources.
/// </summary>
internal static class PointerCompatExtensions
{
    extension(PointerRoutedEventArgs e)
    {
        /// <summary>
        /// Gets the button whose release raised a pointer released event (Avalonia <c>InitialPressMouseButton</c>).
        /// </summary>
        /// <remarks>Touch and pen contacts report <see cref="MouseButton.Left"/>, like on Avalonia.</remarks>
        public MouseButton InitialPressMouseButton => e.GetCurrentPoint(null).Properties.PointerUpdateKind switch
        {
            PointerUpdateKind.LeftButtonReleased => MouseButton.Left,
            PointerUpdateKind.RightButtonReleased => MouseButton.Right,
            PointerUpdateKind.MiddleButtonReleased => MouseButton.Middle,
            PointerUpdateKind.XButton1Released => MouseButton.XButton1,
            PointerUpdateKind.XButton2Released => MouseButton.XButton2,
            _ => MouseButton.None,
        };
    }
}
