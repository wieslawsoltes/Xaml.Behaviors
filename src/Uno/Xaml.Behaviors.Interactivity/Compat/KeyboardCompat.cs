// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia keyboard modifier APIs used by the shared sources.
/// </summary>
/// <remarks>
/// Avalonia <c>KeyModifiers</c> map to <see cref="VirtualKeyModifiers"/>: <c>Control</c>, <c>Shift</c>,
/// <c>Alt</c> (<see cref="VirtualKeyModifiers.Menu"/>) and <c>Meta</c> (<see cref="VirtualKeyModifiers.Windows"/>).
/// </remarks>
internal static class KeyboardCompat
{
    extension(KeyRoutedEventArgs e)
    {
        /// <summary>
        /// Gets the keyboard modifiers that are pressed while the key event is raised (Avalonia
        /// <c>KeyEventArgs.KeyModifiers</c>).
        /// </summary>
        public VirtualKeyModifiers KeyModifiers => GetKeyModifiers();
    }

    extension(CharacterReceivedRoutedEventArgs e)
    {
        /// <summary>
        /// Gets the text entered (Avalonia <c>TextInputEventArgs.Text</c>).
        /// </summary>
        public string Text => e.Character.ToString();
    }

    /// <summary>
    /// Gets the keyboard modifiers currently pressed on the UI thread.
    /// </summary>
    /// <returns>The pressed modifiers.</returns>
    public static VirtualKeyModifiers GetKeyModifiers()
    {
        var modifiers = VirtualKeyModifiers.None;

        if (IsDown(VirtualKey.Control) || IsDown(VirtualKey.LeftControl) || IsDown(VirtualKey.RightControl))
        {
            modifiers |= VirtualKeyModifiers.Control;
        }

        if (IsDown(VirtualKey.Shift) || IsDown(VirtualKey.LeftShift) || IsDown(VirtualKey.RightShift))
        {
            modifiers |= VirtualKeyModifiers.Shift;
        }

        if (IsDown(VirtualKey.Menu) || IsDown(VirtualKey.LeftMenu) || IsDown(VirtualKey.RightMenu))
        {
            modifiers |= VirtualKeyModifiers.Menu;
        }

        if (IsDown(VirtualKey.LeftWindows) || IsDown(VirtualKey.RightWindows))
        {
            modifiers |= VirtualKeyModifiers.Windows;
        }

        return modifiers;
    }

    private static bool IsDown(VirtualKey key)
        => (InputKeyboardSource.GetKeyStateForCurrentThread(key) & CoreVirtualKeyStates.Down) != 0;
}
