// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A behavior that listens for a specified event on its source and executes its actions when that event is fired.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public abstract class EventTriggerBase : StyledElementTrigger
{
    private const string EventNameDefaultValue = "AttachedToVisualTree";

    /// <summary>
    /// Identifies the <seealso cref="EventName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> EventNameProperty =
        AvaloniaProperty.Register<EventTriggerBase, string?>(nameof(EventName), EventNameDefaultValue);

    /// <summary>
    /// Identifies the <seealso cref="SourceObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> SourceObjectProperty =
        AvaloniaProperty.Register<EventTriggerBase, object?>(nameof(SourceObject));

    private object? _resolvedSource;
    private Delegate? _eventHandler;
    private bool _isLoadedEventRegistered;
    private IDisposable? _tryRegisterEventHandlerDisposable;

    /// <summary>
    /// Gets or sets the name of the event to listen for. This is an avalonia property.
    /// </summary>
    public string? EventName
    {
        get => GetValue(EventNameProperty);
        set => SetValue(EventNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the source object from which this behavior listens for events.
    /// If <seealso cref="SourceObject"/> is not set, the source will default to <seealso cref="IBehavior.AssociatedObject"/>. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public object? SourceObject
    {
        get => GetValue(SourceObjectProperty);
        set => SetValue(SourceObjectProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == EventNameProperty)
        {
            var oldEventName = change.GetOldValue<string?>();
            if (oldEventName is not null)
            {
                UnregisterEvent(oldEventName, _resolvedSource);
            }

            var newEventName = change.GetNewValue<string?>();
            if (newEventName is not null)
            {
                RegisterEvent(newEventName, _resolvedSource);
            }
        }
        else if (change.Property == SourceObjectProperty)
        {
            SetResolvedSource(EventName, SourceObject ?? AssociatedObject);
        }
    }

    /// <summary>
    /// Called after the behavior is attached to the <see cref="IBehavior.AssociatedObject"/>.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        SetResolvedSource(EventName, SourceObject ?? AssociatedObject);
    }

    /// <summary>
    /// Called when the behavior is being detached from its <see cref="IBehavior.AssociatedObject"/>.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        SetResolvedSource(EventName, null);
    }

    private void SetResolvedSource(string? eventName, object? newSource)
    {
        if (_resolvedSource == newSource)
        {
            return;
        }

        if (_resolvedSource is not null)
        {
            UnregisterEvent(eventName, _resolvedSource);
        }

        _resolvedSource = newSource;

        if (_resolvedSource is not null)
        {
            RegisterEvent(eventName, _resolvedSource);
        }
    }

    private void RegisterEvent(string? eventName, object? resolvedSource)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            return;
        }

        if (eventName != EventNameDefaultValue)
        {
            RegisterUserEvent(eventName!, resolvedSource);
        }
        else if (!_isLoadedEventRegistered)
        {
            RegisterDefaultEvent(resolvedSource);
        }
    }

    private void UnregisterEvent(string? eventName, object? resolvedSource)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            return;
        }

        if (eventName != EventNameDefaultValue)
        {
            UnregisterUserEvent(eventName!, resolvedSource);
        }
        else if (_isLoadedEventRegistered)
        {
            UnregisterDefaultEvent(resolvedSource);
        }
    }

    private void RegisterUserEvent(string eventName, object? resolvedSource)
    {
        if (resolvedSource is null)
        {
            return;
        }

        var disposable = AddEventHandlerRegistry.TryRegisterEventHandler(resolvedSource, eventName, AttachedToVisualTree);
        if (disposable is not null)
        {
            _tryRegisterEventHandlerDisposable = disposable;
            return;
        }

        AddEventHandler(eventName, resolvedSource);
    }

    private void UnregisterUserEvent(string eventName, object? resolvedSource)
    {
        if (_tryRegisterEventHandlerDisposable is not null)
        {
            _tryRegisterEventHandlerDisposable.Dispose();
            _tryRegisterEventHandlerDisposable = null;
            return;
        }

        if (_eventHandler is null)
        {
            return;
        }

        if (resolvedSource is not null)
        {
            RemoveEventHandler(eventName, resolvedSource);
        }

        _eventHandler = null;
    }

    private void AddEventHandler(string eventName, object resolvedSource)
    {   
        var sourceObjectType = resolvedSource.GetType();
        var eventInfo = sourceObjectType.GetRuntimeEvent(eventName);
        if (eventInfo is null)
        {
            return;
        }

        var methodInfo = typeof(EventTriggerBase).GetTypeInfo().GetDeclaredMethod("AttachedToVisualTree");
        if (methodInfo is not null)
        {
            var eventHandlerType = eventInfo.EventHandlerType;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (eventHandlerType is not null)
            {
                _eventHandler = methodInfo.CreateDelegate(eventHandlerType, this);
                if (_eventHandler is not null)
                {
                    eventInfo.AddEventHandler(resolvedSource, _eventHandler);
                }
            }
        }
    }

    private void RemoveEventHandler(string eventName, object resolvedSource)
    {
        var eventInfo = resolvedSource.GetType().GetRuntimeEvent(eventName);
        eventInfo?.RemoveEventHandler(resolvedSource, _eventHandler);
    }

    private void RegisterDefaultEvent(object? resolvedSource)
    {
        if (resolvedSource is Control element && !IsElementLoaded(element))
        {
            _isLoadedEventRegistered = true;
            element.AttachedToVisualTree += AttachedToVisualTree;
        }
    }

    private void UnregisterDefaultEvent(object? resolvedSource)
    {
        _isLoadedEventRegistered = false;
        if (resolvedSource is Control element)
        {
            element.AttachedToVisualTree -= AttachedToVisualTree; 
        }
    }

    /// <summary>
    /// Raised when the control is attached to a rooted visual tree.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="eventArgs">The event args.</param>
    protected virtual void AttachedToVisualTree(object? sender, object eventArgs)
    {
        OnEvent(eventArgs);
    }

    /// <summary>
    /// Executes the actions associated with this behavior.
    /// </summary>
    /// <param name="eventArgs">The event args.</param>
    protected virtual void OnEvent(object? eventArgs)
    {
        if (!IsEnabled)
        {
            return;
        }

        var sender = AssociatedObject ?? _resolvedSource;
        if (sender is null)
        {
            return;
        }

        Interaction.ExecuteActions(sender, Actions, eventArgs);
    }

    private static bool IsElementLoaded(Control element) => element.Parent is not null;
}
