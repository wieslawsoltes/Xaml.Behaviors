// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Avalonia <c>KeyGesture.Matches</c> for the WinUI <see cref="KeyboardAccelerator"/> (the Uno Platform key gesture).
/// </summary>
internal static class KeyboardAcceleratorExtensions
{
    /// <summary>
    /// Determines whether the key event matches the key and the modifiers of the accelerator.
    /// </summary>
    /// <param name="accelerator">The accelerator.</param>
    /// <param name="e">The key event.</param>
    /// <returns><c>true</c> when the key and the pressed modifiers match.</returns>
    public static bool Matches(this KeyboardAccelerator accelerator, KeyRoutedEventArgs e)
        => e.Key == accelerator.Key && GetModifiers() == accelerator.Modifiers;

    private static VirtualKeyModifiers GetModifiers()
    {
        var modifiers = VirtualKeyModifiers.None;
        if (IsDown(VirtualKey.Control))
        {
            modifiers |= VirtualKeyModifiers.Control;
        }

        if (IsDown(VirtualKey.Menu))
        {
            modifiers |= VirtualKeyModifiers.Menu;
        }

        if (IsDown(VirtualKey.Shift))
        {
            modifiers |= VirtualKeyModifiers.Shift;
        }

        if (IsDown(VirtualKey.LeftWindows) || IsDown(VirtualKey.RightWindows))
        {
            modifiers |= VirtualKeyModifiers.Windows;
        }

        return modifiers;
    }

    private static bool IsDown(VirtualKey key)
        => (InputKeyboardSource.GetKeyStateForCurrentThread(key) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
}
