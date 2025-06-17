// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A registry for adding event handlers to various controls.
/// </summary>
public static class AddEventHandlerRegistry
{
    private static readonly HashSet<IAddEventHandler> s_addEventHandlers = [];

    static AddEventHandlerRegistry()
    {
        // Register(new ButtonClickEventHandler());

        Register(new FuncAddEventHandler<Button, RoutedEventArgs>(
            nameof(Button.Click), 
            (o, h) => o.Click += h, 
            (o, h) => o.Click -= h));

        Register(new FuncAddEventHandler<MenuItem, RoutedEventArgs>(
            nameof(MenuItem.Click), 
            (o, h) => o.Click += h, 
            (o, h) => o.Click -= h));
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
