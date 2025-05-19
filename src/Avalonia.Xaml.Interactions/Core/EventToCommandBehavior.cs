using System;
using System.Reflection;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Simplified behavior that connects an event to an <see cref="ICommand"/>.
/// </summary>
public class EventToCommandBehavior : StyledElementBehavior
{
    /// <summary>
    /// Identifies the <seealso cref="EventName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> EventNameProperty =
        AvaloniaProperty.Register<EventToCommandBehavior, string?>(nameof(EventName));

    /// <summary>
    /// Identifies the <seealso cref="SourceObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> SourceObjectProperty =
        AvaloniaProperty.Register<EventToCommandBehavior, object?>(nameof(SourceObject));

    /// <summary>
    /// Identifies the <seealso cref="Command"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<EventToCommandBehavior, ICommand?>(nameof(Command));

    /// <summary>
    /// Identifies the <seealso cref="CommandParameter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<EventToCommandBehavior, object?>(nameof(CommandParameter));

    /// <summary>
    /// Identifies the <seealso cref="PassEventArgsToCommand"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> PassEventArgsToCommandProperty =
        AvaloniaProperty.Register<EventToCommandBehavior, bool>(nameof(PassEventArgsToCommand));

    private object? _resolvedSource;
    private Delegate? _eventHandler;

    /// <summary>
    /// Gets or sets the name of the event to listen for.
    /// </summary>
    public string? EventName
    {
        get => GetValue(EventNameProperty);
        set => SetValue(EventNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the source object from which this behavior listens for events.
    /// </summary>
    [ResolveByName]
    public object? SourceObject
    {
        get => GetValue(SourceObjectProperty);
        set => SetValue(SourceObjectProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when the event fires.
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command parameter.
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the event args should be passed to the command.
    /// </summary>
    public bool PassEventArgsToCommand
    {
        get => GetValue(PassEventArgsToCommandProperty);
        set => SetValue(PassEventArgsToCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        _resolvedSource = SourceObject ?? AssociatedObject;
        RegisterEvent();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        UnregisterEvent();
        _resolvedSource = null;
        base.OnDetaching();
    }

    private void RegisterEvent()
    {
        if (_resolvedSource is null || string.IsNullOrEmpty(EventName))
        {
            return;
        }

        var eventInfo = _resolvedSource.GetType().GetRuntimeEvent(EventName!);
        if (eventInfo is null)
        {
            return;
        }

        var methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod(nameof(OnEvent));
        if (methodInfo is null)
        {
            return;
        }

        var eventHandlerType = eventInfo.EventHandlerType;
        if (eventHandlerType is null)
        {
            return;
        }

        _eventHandler = methodInfo.CreateDelegate(eventHandlerType, this);
        eventInfo.AddEventHandler(_resolvedSource, _eventHandler);
    }

    private void UnregisterEvent()
    {
        if (_eventHandler is null || _resolvedSource is null || string.IsNullOrEmpty(EventName))
        {
            return;
        }

        var eventInfo = _resolvedSource.GetType().GetRuntimeEvent(EventName!);
        eventInfo?.RemoveEventHandler(_resolvedSource, _eventHandler);
        _eventHandler = null;
    }

    private void OnEvent(object? sender, object eventArgs)
    {
        if (!IsEnabled)
        {
            return;
        }

        var parameter = PassEventArgsToCommand ? eventArgs : CommandParameter;
        if (Command?.CanExecute(parameter) == true)
        {
            Command.Execute(parameter);
        }
    }
}
