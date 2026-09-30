// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation.Metadata;
using Windows.System;

namespace Xaml.Interactivity;

/// <summary>
/// Defines a keyboard input combination (Uno Platform counterpart of the Avalonia <c>KeyGesture</c>).
/// </summary>
/// <remarks>
/// Parsed from strings such as <c>"Ctrl+S"</c>, <c>"Ctrl+Shift+F5"</c> or <c>"Alt+Enter"</c> (also in XAML).
/// Modifier names: <c>Ctrl</c>/<c>Control</c>, <c>Shift</c>, <c>Alt</c>/<c>Menu</c> and
/// <c>Win</c>/<c>Meta</c>/<c>Cmd</c>/<c>Windows</c>. Key names are <see cref="VirtualKey"/> names, digits and the
/// characters <c>+ - , . ;</c>.
/// </remarks>
[CreateFromString(MethodName = "Xaml.Interactivity.KeyGesture.Parse")]
public sealed class KeyGesture : IEquatable<KeyGesture>
{
    private static readonly Dictionary<string, VirtualKeyModifiers> s_modifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Ctrl"] = VirtualKeyModifiers.Control,
        ["Control"] = VirtualKeyModifiers.Control,
        ["Shift"] = VirtualKeyModifiers.Shift,
        ["Alt"] = VirtualKeyModifiers.Menu,
        ["Menu"] = VirtualKeyModifiers.Menu,
        ["Win"] = VirtualKeyModifiers.Windows,
        ["Windows"] = VirtualKeyModifiers.Windows,
        ["Meta"] = VirtualKeyModifiers.Windows,
        ["Cmd"] = VirtualKeyModifiers.Windows,
    };

    private static readonly Dictionary<string, VirtualKey> s_keys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["+"] = (VirtualKey)0xBB,
        ["Plus"] = (VirtualKey)0xBB,
        ["OemPlus"] = (VirtualKey)0xBB,
        [","] = (VirtualKey)0xBC,
        ["OemComma"] = (VirtualKey)0xBC,
        ["-"] = (VirtualKey)0xBD,
        ["Minus"] = (VirtualKey)0xBD,
        ["OemMinus"] = (VirtualKey)0xBD,
        ["."] = (VirtualKey)0xBE,
        ["OemPeriod"] = (VirtualKey)0xBE,
        [";"] = (VirtualKey)0xBA,
        ["Return"] = VirtualKey.Enter,
        ["Esc"] = VirtualKey.Escape,
        ["Del"] = VirtualKey.Delete,
        ["Ins"] = VirtualKey.Insert,
        ["PgUp"] = VirtualKey.PageUp,
        ["PgDn"] = VirtualKey.PageDown,
        ["Alt"] = VirtualKey.Menu,
        ["Ctrl"] = VirtualKey.Control,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyGesture"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifiers.</param>
    public KeyGesture(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        Key = key;
        KeyModifiers = modifiers;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    public VirtualKey Key { get; }

    /// <summary>
    /// Gets the modifiers.
    /// </summary>
    public VirtualKeyModifiers KeyModifiers { get; }

    /// <summary>
    /// Parses a gesture such as <c>"Ctrl+Shift+S"</c>.
    /// </summary>
    /// <param name="gesture">The gesture text.</param>
    /// <returns>The gesture.</returns>
    /// <exception cref="ArgumentException">The text is not a valid gesture.</exception>
    public static KeyGesture Parse(string gesture)
    {
        ArgumentNullException.ThrowIfNull(gesture);

        var text = gesture.Trim();
        if (text.Length == 0)
        {
            throw new ArgumentException("A key gesture cannot be empty.", nameof(gesture));
        }

        var modifiers = VirtualKeyModifiers.None;
        var start = 0;
        while (start < text.Length)
        {
            // A '+' directly after a separator (or at the end) is the key itself ("Ctrl++").
            var separator = text.IndexOf('+', start + 1);
            if (separator < 0)
            {
                break;
            }

            var part = text.Substring(start, separator - start).Trim();
            if (!s_modifiers.TryGetValue(part, out var modifier))
            {
                throw new ArgumentException($"Invalid modifier '{part}' in key gesture '{gesture}'.", nameof(gesture));
            }

            modifiers |= modifier;
            start = separator + 1;
        }

        return new KeyGesture(ParseKey(text.Substring(start).Trim(), gesture), modifiers);
    }

    /// <summary>
    /// Determines whether a key event matches this gesture.
    /// </summary>
    /// <param name="keyEvent">The key event.</param>
    /// <returns><c>true</c> when the key and the pressed modifiers match.</returns>
    public bool Matches(KeyRoutedEventArgs? keyEvent)
        => keyEvent is not null && keyEvent.Key == Key && KeyboardCompat.GetKeyModifiers() == KeyModifiers;

    /// <inheritdoc />
    public bool Equals(KeyGesture? other)
        => other is not null && other.Key == Key && other.KeyModifiers == KeyModifiers;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is KeyGesture other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Key, KeyModifiers);

    /// <inheritdoc />
    public override string ToString()
    {
        var builder = new StringBuilder();
        Append(builder, VirtualKeyModifiers.Control, "Ctrl");
        Append(builder, VirtualKeyModifiers.Shift, "Shift");
        Append(builder, VirtualKeyModifiers.Menu, "Alt");
        Append(builder, VirtualKeyModifiers.Windows, "Win");
        builder.Append(FormatKey(Key));
        return builder.ToString();
    }

    private void Append(StringBuilder builder, VirtualKeyModifiers modifier, string name)
    {
        if ((KeyModifiers & modifier) != 0)
        {
            builder.Append(name).Append('+');
        }
    }

    private static string FormatKey(VirtualKey key)
    {
        if (key is >= VirtualKey.Number0 and <= VirtualKey.Number9)
        {
            return ((int)(key - VirtualKey.Number0)).ToString(CultureInfo.InvariantCulture);
        }

        return (int)key switch
        {
            0xBB => "+",
            0xBC => ",",
            0xBD => "-",
            0xBE => ".",
            0xBA => ";",
            _ => key.ToString(),
        };
    }

    private static VirtualKey ParseKey(string key, string gesture)
    {
        if (key.Length == 1 && key[0] is >= '0' and <= '9')
        {
            return VirtualKey.Number0 + (key[0] - '0');
        }

        if (s_keys.TryGetValue(key, out var mapped))
        {
            return mapped;
        }

        if (key.Length > 0 && !char.IsDigit(key[0]) && Enum.TryParse<VirtualKey>(key, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException($"Invalid key '{key}' in key gesture '{gesture}'.", nameof(gesture));
    }
}
