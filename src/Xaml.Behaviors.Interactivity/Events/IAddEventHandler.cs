using System;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A service that provides a way to register event handlers for specific events on various controls.
/// </summary>
public interface IAddEventHandler
{
    /// <summary>
    /// Checks if the given source object and event name match the criteria for this event handler.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="eventName"></param>
    /// <returns></returns>
    bool Matches(object source, string eventName);

    /// <summary>
    /// Registers an event handler for a specific event on the given source object.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="eventName"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    IDisposable? AddHandler(object source, string eventName, Action<object?, object> handler);
}
