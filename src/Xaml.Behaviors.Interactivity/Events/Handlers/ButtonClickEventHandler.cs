using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactivity;

internal class ButtonClickEventHandler : IAddEventHandler
{
    private static string EventName => nameof(Button.Click);

    public bool Matches(object source, string eventName) 
        => source is Button && eventName == EventName;

    public IDisposable? AddHandler(object source, string eventName, Action<object?, object> handler)
    {
        if (source is not Button target || eventName != EventName)
        {
            return null;
        }

        target.Click += EventHandler;

        return DisposableAction.Create(() => target.Click -= EventHandler);

        void EventHandler(object? sender, RoutedEventArgs e) => handler(sender, e);
    }
}
