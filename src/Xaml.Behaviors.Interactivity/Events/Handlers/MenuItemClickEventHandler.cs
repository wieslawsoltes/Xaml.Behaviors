using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactivity;

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
