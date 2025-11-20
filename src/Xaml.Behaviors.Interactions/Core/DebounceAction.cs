// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that will execute its child actions after a specified delay.
/// If invoked again before the delay elapses, the timer is reset.
/// </summary>
public class DebounceAction : StyledElementAction
{
    private DispatcherTimer? _timer;
    private object? _lastSender;
    private object? _lastParameter;

    /// <summary>
    /// Identifies the <seealso cref="Delay"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DelayProperty =
        AvaloniaProperty.Register<DebounceAction, TimeSpan>(nameof(Delay));

    /// <summary>
    /// Identifies the <seealso cref="Actions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ActionCollection?> ActionsProperty =
        AvaloniaProperty.Register<DebounceAction, ActionCollection?>(nameof(Actions));

    /// <summary>
    /// Gets or sets the delay to wait before executing the actions. This is an avalonia property.
    /// </summary>
    public TimeSpan Delay
    {
        get => GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of actions to execute. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebounceAction"/> class.
    /// </summary>
    public DebounceAction()
    {
        SetCurrentValue(ActionsProperty, new ActionCollection());
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ActionsProperty)
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
    }

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        AttachActionsToLogicalTree(Actions);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        DetachActionsFromLogicalTree(Actions);
        base.OnDetachedFromLogicalTree(e);
    }

    private void AttachActionsToLogicalTree(ActionCollection? actions)
    {
        if (actions is null)
        {
            return;
        }

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
        if (actions is null)
        {
            return;
        }

        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.DetachActionFromLogicalTree(this);
            }
        }
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        _lastSender = sender;
        _lastParameter = parameter;

        if (_timer == null)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
        }

        _timer.Interval = Delay;
        _timer.Stop();
        _timer.Start();

        return null;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _timer?.Stop();
        Interaction.ExecuteActions(_lastSender, Actions, _lastParameter);
        _lastSender = null;
        _lastParameter = null;
    }
}
