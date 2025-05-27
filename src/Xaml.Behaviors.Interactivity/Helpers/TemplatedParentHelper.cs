// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Reflection;

namespace Avalonia.Xaml.Interactivity;

internal static class TemplatedParentHelper
{
    private static readonly Action<StyledElement, AvaloniaObject?>? s_setterDelegate;

    static TemplatedParentHelper()
    {
        var templatedParentProperty = typeof(StyledElement).GetProperty(
            "TemplatedParent",
            BindingFlags.Public | BindingFlags.Instance);

        var setMethod = templatedParentProperty?.GetSetMethod(true);
        if (setMethod != null)
        {
            s_setterDelegate = (Action<StyledElement, AvaloniaObject?>)Delegate.CreateDelegate(
                typeof(Action<StyledElement, AvaloniaObject?>),
                setMethod);
        }
    }

    public static void SetTemplatedParent(StyledElement target, AvaloniaObject? value)
    {
        s_setterDelegate?.Invoke(target, value);
    }
}
