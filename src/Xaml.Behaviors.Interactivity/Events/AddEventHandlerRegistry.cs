// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// A registry for adding event handlers to various controls.
/// </summary>
public static class AddEventHandlerRegistry
{
    private static readonly HashSet<IAddEventHandler> s_addEventHandlers = [];

    static AddEventHandlerRegistry()
    {
#if UNO
        Register(new DelegateAddEventHandler<Microsoft.UI.Xaml.Controls.Primitives.ButtonBase, RoutedEventHandler>(
            "Click",
            static h => (s, e) => h(s, e),
            static (o, h) => o.Click += h,
            static (o, h) => o.Click -= h));

        Register(new DelegateAddEventHandler<MenuFlyoutItem, RoutedEventHandler>(
            nameof(MenuFlyoutItem.Click),
            static h => (s, e) => h(s, e),
            static (o, h) => o.Click += h,
            static (o, h) => o.Click -= h));

        Register(new DelegateAddEventHandler<Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase, EventHandler<object>>(
            "Opened",
            static h => (s, e) => h(s, e),
            static (o, h) => o.Opened += h,
            static (o, h) => o.Opened -= h));

        Register(new DelegateAddEventHandler<Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase, EventHandler<object>>(
            "Closed",
            static h => (s, e) => h(s, e),
            static (o, h) => o.Closed += h,
            static (o, h) => o.Closed -= h));
#else
        // Register(new ButtonClickEventHandler());

        Register(new FuncAddEventHandler<Button, RoutedEventArgs>(
            nameof(Button.Click), 
            (o, h) => o.Click += h, 
            (o, h) => o.Click -= h));

        Register(new FuncAddEventHandler<MenuItem, RoutedEventArgs>(
            nameof(MenuItem.Click), 
            (o, h) => o.Click += h, 
            (o, h) => o.Click -= h));

        Register(new FlyoutEventHandler());

        Register(new FuncAddEventHandler<Control, CancelRoutedEventArgs>(
            "ToolTipOpening",
            ToolTip.AddToolTipOpeningHandler,
            ToolTip.RemoveToolTipOpeningHandler));

        Register(new FuncAddEventHandler<Control, RoutedEventArgs>(
            "ToolTipClosing",
            ToolTip.AddToolTipClosingHandler,
            ToolTip.RemoveToolTipClosingHandler));
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="addEventHandler"></param>
    public static void Register(IAddEventHandler addEventHandler)
    {
        s_addEventHandlers.Add(addEventHandler);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="addEventHandler"></param>
    public static void Unregister(IAddEventHandler addEventHandler)
    {
        if (s_addEventHandlers.Contains(addEventHandler))
        {
            s_addEventHandlers.Remove(addEventHandler);
        }
    }

    internal static IDisposable? TryRegisterEventHandler(object source, string eventName, Action<object?, object> handler)
    {
        var addEventHandler = s_addEventHandlers.FirstOrDefault(x => x.Matches(source, eventName));

        return addEventHandler?.AddHandler(source, eventName, handler);
    }
}
