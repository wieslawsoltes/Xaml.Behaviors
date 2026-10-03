// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
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

internal class MenuItemClickEventHandler : IAddEventHandler
{
    private static string EventName => nameof(MenuItem.Click);

    public bool Matches(object source, string eventName) 
        => source is MenuItem && eventName == EventName;

    public IDisposable? AddHandler(object source, string eventName, Action<object?, object> handler)
    {
        if (source is not MenuItem target || eventName != EventName)
        {
            return null;
        }

        target.Click += EventHandler;

        return DisposableAction.Create(() => target.Click -= EventHandler);

        void EventHandler(object? sender, RoutedEventArgs e) => handler(sender, e);
    }
}
