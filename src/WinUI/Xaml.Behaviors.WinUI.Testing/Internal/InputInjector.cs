// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.System;

namespace Xaml.Behaviors.WinUI.Testing.Internal;

/// <summary>
/// Injects keyboard and mouse input with <c>SendInput</c>.
/// </summary>
internal static class InputInjector
{
    private static readonly int s_inputSize = Marshal.SizeOf<NativeMethods.INPUT>();

    public static void Key(VirtualKey key, bool up)
    {
        var flags = up ? NativeMethods.KEYEVENTF_KEYUP : 0;
        if (IsExtendedKey(key))
        {
            flags |= NativeMethods.KEYEVENTF_EXTENDEDKEY;
        }

        Send(new NativeMethods.INPUT
        {
            type = NativeMethods.INPUT_KEYBOARD,
            u = new NativeMethods.INPUTUNION { ki = new NativeMethods.KEYBDINPUT { wVk = (ushort)key, dwFlags = flags } },
        });
    }

    public static void Character(char character, bool up)
    {
        Send(new NativeMethods.INPUT
        {
            type = NativeMethods.INPUT_KEYBOARD,
            u = new NativeMethods.INPUTUNION
            {
                ki = new NativeMethods.KEYBDINPUT
                {
                    wScan = character,
                    dwFlags = NativeMethods.KEYEVENTF_UNICODE | (up ? NativeMethods.KEYEVENTF_KEYUP : 0),
                },
            },
        });
    }

    public static void MoveTo(int screenX, int screenY)
    {
        var left = NativeMethods.GetSystemMetrics(NativeMethods.SM_XVIRTUALSCREEN);
        var top = NativeMethods.GetSystemMetrics(NativeMethods.SM_YVIRTUALSCREEN);
        var width = Math.Max(2, NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVIRTUALSCREEN));
        var height = Math.Max(2, NativeMethods.GetSystemMetrics(NativeMethods.SM_CYVIRTUALSCREEN));

        Send(new NativeMethods.INPUT
        {
            type = NativeMethods.INPUT_MOUSE,
            u = new NativeMethods.INPUTUNION
            {
                mi = new NativeMethods.MOUSEINPUT
                {
                    dx = (int)Math.Round((screenX - left) * 65535.0 / (width - 1)),
                    dy = (int)Math.Round((screenY - top) * 65535.0 / (height - 1)),
                    dwFlags = NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE | NativeMethods.MOUSEEVENTF_VIRTUALDESK,
                },
            },
        });
    }

    public static void Button(uint flags) => Mouse(flags, 0);

    public static void Wheel(int delta, bool horizontal)
        => Mouse(horizontal ? NativeMethods.MOUSEEVENTF_HWHEEL : NativeMethods.MOUSEEVENTF_WHEEL, delta);

    private static void Mouse(uint flags, int data)
    {
        Send(new NativeMethods.INPUT
        {
            type = NativeMethods.INPUT_MOUSE,
            u = new NativeMethods.INPUTUNION { mi = new NativeMethods.MOUSEINPUT { dwFlags = flags, mouseData = data } },
        });
    }

    private static void Send(NativeMethods.INPUT input)
    {
        if (NativeMethods.SendInput(1, [input], s_inputSize) != 1)
        {
            throw new InvalidOperationException("SendInput failed: the input was blocked (is the desktop locked or another window in front?).", new Win32Exception(Marshal.GetLastPInvokeError()));
        }
    }

    private static bool IsExtendedKey(VirtualKey key) => key switch
    {
        VirtualKey.Left or VirtualKey.Right or VirtualKey.Up or VirtualKey.Down or
        VirtualKey.Home or VirtualKey.End or VirtualKey.PageUp or VirtualKey.PageDown or
        VirtualKey.Insert or VirtualKey.Delete or VirtualKey.RightControl or VirtualKey.RightMenu or
        VirtualKey.Divide or VirtualKey.NumberKeyLock or VirtualKey.Snapshot or
        VirtualKey.LeftWindows or VirtualKey.RightWindows or VirtualKey.Application => true,
        _ => false,
    };
}
