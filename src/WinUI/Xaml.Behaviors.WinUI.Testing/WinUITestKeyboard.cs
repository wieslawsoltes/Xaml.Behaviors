// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Windows.System;
using Xaml.Behaviors.WinUI.Testing.Internal;

namespace Xaml.Behaviors.WinUI.Testing;

/// <summary>
/// Injects keyboard input into the test window of the session (<see cref="WinUITestSession.Keyboard"/>).
/// </summary>
/// <remarks>
/// The input goes to the focused element of the test window, which is brought to the foreground first. Each method
/// processes the messages of the UI thread afterwards, so the routed events have been raised when it returns.
/// </remarks>
public sealed class WinUITestKeyboard
{
    private readonly WinUITestSession _session;
    private VirtualKeyModifiers _modifiers;

    internal WinUITestKeyboard(WinUITestSession session)
    {
        _session = session;
    }

    /// <summary>
    /// Gets the modifier keys held down with <see cref="KeyDown"/>.
    /// </summary>
    public VirtualKeyModifiers Modifiers => _modifiers;

    /// <summary>
    /// Presses a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys held down with the key (pressed before it and kept down).</param>
    /// <param name="character">Ignored: the operating system produces the character of the key from the keyboard
    /// layout (use <see cref="TypeText"/> to enter arbitrary text).</param>
    /// <returns>Always <c>true</c>: injected input does not report whether it was handled.</returns>
    public bool KeyDown(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None, char? character = null)
    {
        Begin();
        PressModifiers(modifiers);
        if (ToModifier(key) is { } modifier)
        {
            _modifiers |= modifier;
        }

        InputInjector.Key(key, up: false);
        End();
        return true;
    }

    /// <summary>
    /// Releases a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys that are released after the key.</param>
    /// <returns>Always <c>true</c>: injected input does not report whether it was handled.</returns>
    public bool KeyUp(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        Begin();
        InputInjector.Key(key, up: true);
        if (ToModifier(key) is { } modifier)
        {
            _modifiers &= ~modifier;
        }

        ReleaseModifiers(modifiers);
        End();
        return true;
    }

    /// <summary>
    /// Presses and releases a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys held down with the key.</param>
    /// <param name="character">Ignored (see <see cref="KeyDown"/>).</param>
    /// <returns>Always <c>true</c>: injected input does not report whether it was handled.</returns>
    public bool Press(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None, char? character = null)
    {
        var held = _modifiers;
        KeyDown(key, modifiers, character);
        KeyUp(key, modifiers & ~held);
        return true;
    }

    /// <summary>
    /// Types text: one character input per character, independent of the keyboard layout.
    /// </summary>
    /// <param name="text">The text.</param>
    public void TypeText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        foreach (var character in text)
        {
            Begin();
            InputInjector.Character(character, up: false);
            InputInjector.Character(character, up: true);
            End();
        }
    }

    private void Begin()
    {
        _session.EnsureThreadAccess();
        _session.EnsureForeground();
    }

    private void End() => _session.Pump(_session.Options.InputDelay);

    private void PressModifiers(VirtualKeyModifiers modifiers)
    {
        foreach (var (modifier, key) in s_modifierKeys)
        {
            if ((modifiers & modifier) != 0 && (_modifiers & modifier) == 0)
            {
                InputInjector.Key(key, up: false);
                _modifiers |= modifier;
            }
        }
    }

    private void ReleaseModifiers(VirtualKeyModifiers modifiers)
    {
        foreach (var (modifier, key) in s_modifierKeys)
        {
            if ((modifiers & modifier) != 0 && (_modifiers & modifier) != 0)
            {
                InputInjector.Key(key, up: true);
                _modifiers &= ~modifier;
            }
        }
    }

    internal void PressForPointer(VirtualKeyModifiers modifiers, out VirtualKeyModifiers pressed)
    {
        pressed = modifiers & ~_modifiers;
        PressModifiers(pressed);
    }

    internal void ReleaseForPointer(VirtualKeyModifiers pressed)
    {
        // Releasing Alt without another key in between activates the window menu: press an unassigned key first.
        if ((pressed & VirtualKeyModifiers.Menu) != 0)
        {
            InputInjector.Key(MenuMaskKey, up: false);
            InputInjector.Key(MenuMaskKey, up: true);
        }

        ReleaseModifiers(pressed);
    }

    /// <summary>An unassigned virtual key (0xE8), pressed to keep Windows from treating an Alt release as a menu key.</summary>
    private const VirtualKey MenuMaskKey = (VirtualKey)0xE8;

    private static VirtualKeyModifiers? ToModifier(VirtualKey key) => key switch
    {
        VirtualKey.Control or VirtualKey.LeftControl or VirtualKey.RightControl => VirtualKeyModifiers.Control,
        VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift => VirtualKeyModifiers.Shift,
        VirtualKey.Menu or VirtualKey.LeftMenu or VirtualKey.RightMenu => VirtualKeyModifiers.Menu,
        VirtualKey.LeftWindows or VirtualKey.RightWindows => VirtualKeyModifiers.Windows,
        _ => null,
    };

    private static readonly (VirtualKeyModifiers Modifier, VirtualKey Key)[] s_modifierKeys =
    [
        (VirtualKeyModifiers.Control, VirtualKey.Control),
        (VirtualKeyModifiers.Shift, VirtualKey.Shift),
        (VirtualKeyModifiers.Menu, VirtualKey.Menu),
        (VirtualKeyModifiers.Windows, VirtualKey.LeftWindows),
    ];
}
