// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Uno.UI.Runtime.Skia.Headless;
using Windows.System;

namespace Xaml.Behaviors.Uno.Headless;

/// <summary>
/// Keyboard input of a <see cref="UnoHeadlessSession"/>. Key events go to the focused element (the root element when
/// nothing has focus) through the regular Uno Platform keyboard pipeline: <c>PreviewKeyDown</c>, <c>KeyDown</c> and, for
/// keys producing a character, <c>CharacterReceived</c>.
/// </summary>
/// <remarks>
/// The keyboard tracks the modifier keys pressed with <see cref="KeyDown"/> and not released yet
/// (<see cref="Modifiers"/>): key and mouse events carry them in addition to the modifiers passed to the members. All
/// members must be called on the UI thread.
/// </remarks>
public sealed class UnoHeadlessKeyboard
{
    private readonly UnoHeadlessSession _session;
    private readonly HeadlessHost _host;
    private VirtualKeyModifiers _left;
    private VirtualKeyModifiers _right;

    internal UnoHeadlessKeyboard(UnoHeadlessSession session, HeadlessHost host)
    {
        _session = session;
        _host = host;
    }

    /// <summary>
    /// Gets the modifier keys that are held: pressed with <see cref="KeyDown"/> and not released with
    /// <see cref="KeyUp"/>.
    /// </summary>
    public VirtualKeyModifiers Modifiers => _left | _right;

    /// <summary>
    /// Raises the key down events of <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys held.</param>
    /// <param name="character">The character the key produces, raised as <c>CharacterReceived</c>; <see langword="null"/> for none.</param>
    /// <returns><see langword="true"/> when the key event was handled.</returns>
    public bool KeyDown(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None, char? character = null)
    {
        _session.EnsureThreadAccess();
        UpdateModifiers(key, down: true);
        return _host.RaiseKey(key, modifiers | Modifiers, down: true, character);
    }

    /// <summary>
    /// Raises the key up events of <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys held.</param>
    /// <returns><see langword="true"/> when the key event was handled.</returns>
    public bool KeyUp(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        _session.EnsureThreadAccess();
        UpdateModifiers(key, down: false);
        return _host.RaiseKey(key, modifiers | Modifiers, down: false, character: null);
    }

    /// <summary>
    /// Presses and releases <paramref name="key"/>, then runs the queued UI work.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys held.</param>
    /// <param name="character">The character the key produces; <see langword="null"/> for none.</param>
    /// <returns><see langword="true"/> when the key down event was handled.</returns>
    public bool Press(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None, char? character = null)
    {
        var handled = KeyDown(key, modifiers, character);
        KeyUp(key, modifiers);
        _session.RunJobs();
        return handled;
    }

    /// <summary>
    /// Types <paramref name="text"/>: presses and releases a key for every character (raising <c>CharacterReceived</c>
    /// with the character), then runs the queued UI work.
    /// </summary>
    /// <param name="text">The text.</param>
    public void TypeText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        _session.EnsureThreadAccess();
        foreach (var character in text)
        {
            var (key, modifiers) = ToKey(character);
            _host.RaiseKey(key, modifiers | Modifiers, down: true, character);
            _host.RaiseKey(key, modifiers | Modifiers, down: false, character: null);
        }

        _session.RunJobs();
    }

    private void UpdateModifiers(VirtualKey key, bool down)
    {
        // The generic keys (Control, Shift, Menu) count as the left keys.
        var (modifier, isRight) = key switch
        {
            VirtualKey.Control or VirtualKey.LeftControl => (VirtualKeyModifiers.Control, false),
            VirtualKey.RightControl => (VirtualKeyModifiers.Control, true),
            VirtualKey.Shift or VirtualKey.LeftShift => (VirtualKeyModifiers.Shift, false),
            VirtualKey.RightShift => (VirtualKeyModifiers.Shift, true),
            VirtualKey.Menu or VirtualKey.LeftMenu => (VirtualKeyModifiers.Menu, false),
            VirtualKey.RightMenu => (VirtualKeyModifiers.Menu, true),
            VirtualKey.LeftWindows => (VirtualKeyModifiers.Windows, false),
            VirtualKey.RightWindows => (VirtualKeyModifiers.Windows, true),
            _ => (VirtualKeyModifiers.None, false),
        };

        if (isRight)
        {
            _right = down ? _right | modifier : _right & ~modifier;
        }
        else
        {
            _left = down ? _left | modifier : _left & ~modifier;
        }
    }

    private static (VirtualKey Key, VirtualKeyModifiers Modifiers) ToKey(char character) => character switch
    {
        >= 'a' and <= 'z' => (VirtualKey.A + (character - 'a'), VirtualKeyModifiers.None),
        >= 'A' and <= 'Z' => (VirtualKey.A + (character - 'A'), VirtualKeyModifiers.Shift),
        >= '0' and <= '9' => (VirtualKey.Number0 + (character - '0'), VirtualKeyModifiers.None),
        ' ' => (VirtualKey.Space, VirtualKeyModifiers.None),
        '\n' or '\r' => (VirtualKey.Enter, VirtualKeyModifiers.None),
        '\t' => (VirtualKey.Tab, VirtualKeyModifiers.None),
        _ => (VirtualKey.None, VirtualKeyModifiers.None),
    };
}
