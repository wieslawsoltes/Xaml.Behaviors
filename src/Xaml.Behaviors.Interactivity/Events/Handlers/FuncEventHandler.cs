using System;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="add"></param>
/// <param name="remove"></param>
/// <typeparam name="TTarget"></typeparam>
/// <typeparam name="TEventArgs"></typeparam>
public class FuncAddEventHandler<TTarget, TEventArgs>(
    string name,
    Action<TTarget, EventHandler<TEventArgs>> add,
    Action<TTarget, EventHandler<TEventArgs>> remove)
    : IAddEventHandler where TTarget : class where TEventArgs : EventArgs
{
    /// <inheritdoc/>
    public bool Matches(object source, string eventName) 
        => source is TTarget && eventName == name;

    /// <inheritdoc/>
    public IDisposable? AddHandler(
        object source, 
        string eventName, 
        Action<object?, object> handler)
    {
        if (source is not TTarget target || eventName != name)
        {
            return null;
        }

        add(target, EventHandler);

        return DisposableAction.Create(() => remove(target, EventHandler));

        void EventHandler(object? sender, TEventArgs e) => handler(sender, e);
    }
}
