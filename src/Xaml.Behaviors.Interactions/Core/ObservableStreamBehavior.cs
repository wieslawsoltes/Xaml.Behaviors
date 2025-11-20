// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A behavior that subscribes to an <see cref="IObservable{T}"/> and executes actions on OnNext, OnError, and OnCompleted.
/// </summary>
public class ObservableStreamBehavior : StyledElementBehavior
{
    private IDisposable? _subscription;

    /// <summary>
    /// Identifies the <seealso cref="Source"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> SourceProperty =
        AvaloniaProperty.Register<ObservableStreamBehavior, object?>(nameof(Source));

    /// <summary>
    /// Identifies the <seealso cref="Actions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ActionCollection?> ActionsProperty =
        AvaloniaProperty.Register<ObservableStreamBehavior, ActionCollection?>(nameof(Actions));

    /// <summary>
    /// Identifies the <seealso cref="ErrorActions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ActionCollection?> ErrorActionsProperty =
        AvaloniaProperty.Register<ObservableStreamBehavior, ActionCollection?>(nameof(ErrorActions));

    /// <summary>
    /// Identifies the <seealso cref="CompletedActions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ActionCollection?> CompletedActionsProperty =
        AvaloniaProperty.Register<ObservableStreamBehavior, ActionCollection?>(nameof(CompletedActions));

    /// <summary>
    /// Gets or sets the source observable. This is an avalonia property.
    /// </summary>
    public object? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of actions to execute when the observable emits a value. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of actions to execute when the observable emits an error. This is an avalonia property.
    /// </summary>
    public ActionCollection? ErrorActions
    {
        get => GetValue(ErrorActionsProperty);
        set => SetValue(ErrorActionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of actions to execute when the observable completes. This is an avalonia property.
    /// </summary>
    public ActionCollection? CompletedActions
    {
        get => GetValue(CompletedActionsProperty);
        set => SetValue(CompletedActionsProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableStreamBehavior"/> class.
    /// </summary>
    public ObservableStreamBehavior()
    {
        SetCurrentValue(ActionsProperty, new ActionCollection());
        SetCurrentValue(ErrorActionsProperty, new ActionCollection());
        SetCurrentValue(CompletedActionsProperty, new ActionCollection());
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            UpdateSubscription();
        }
        else if (change.Property == ActionsProperty)
        {
            HandleActionCollectionChanged(change);
        }
        else if (change.Property == ErrorActionsProperty)
        {
            HandleActionCollectionChanged(change);
        }
        else if (change.Property == CompletedActionsProperty)
        {
            HandleActionCollectionChanged(change);
        }
    }

    private void HandleActionCollectionChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var oldActions = change.GetOldValue<ActionCollection?>();
        var newActions = change.GetNewValue<ActionCollection?>();

        if (oldActions is not null && ((ILogical)this).IsAttachedToLogicalTree)
        {
            DetachActionsFromLogicalTree(oldActions);
        }

        if (newActions is not null && ((ILogical)this).IsAttachedToLogicalTree)
        {
            AttachActionsToLogicalTree(newActions);
        }
    }

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        AttachActionsToLogicalTree(Actions);
        AttachActionsToLogicalTree(ErrorActions);
        AttachActionsToLogicalTree(CompletedActions);
        UpdateSubscription();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _subscription?.Dispose();
        _subscription = null;
        DetachActionsFromLogicalTree(Actions);
        DetachActionsFromLogicalTree(ErrorActions);
        DetachActionsFromLogicalTree(CompletedActions);
        base.OnDetachedFromLogicalTree(e);
    }

    private void UpdateSubscription()
    {
        _subscription?.Dispose();
        _subscription = null;

        if (Source is IObservable<object> observable)
        {
            _subscription = observable
                .Subscribe(new AnonymousObserver<object>(
                    onNext: value => Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, Actions, value)),
                    onError: error => Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, ErrorActions, error)),
                    onCompleted: () => Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, CompletedActions, null))
                ));
        }
    }

    private void AttachActionsToLogicalTree(ActionCollection? actions)
    {
        if (actions is null) return;
        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.AttachActionToLogicalTree(this);
            }
        }
    }

    private void DetachActionsFromLogicalTree(ActionCollection? actions)
    {
        if (actions is null) return;
        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.DetachActionFromLogicalTree(this);
            }
        }
    }
}
