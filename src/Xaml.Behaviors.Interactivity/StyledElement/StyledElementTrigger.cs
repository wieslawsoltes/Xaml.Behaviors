// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Metadata;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A base class for behaviors, implementing the basic plumbing of <seealso cref="ITrigger"/>.
/// </summary>
public abstract class StyledElementTrigger : StyledElementBehavior, ITrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Actions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ActionCollection?> ActionsProperty =
        AvaloniaProperty.Register<StyledElementTrigger, ActionCollection?>(nameof(Actions));

    /// <summary>
    /// Gets the collection of actions associated with the behavior. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StyledElementTrigger"/> class.
    /// </summary>
    protected StyledElementTrigger()
    {
        SetCurrentValue(ActionsProperty, []);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ActionsProperty)
        {
            var oldActions = change.GetOldValue<ActionCollection?>();
            var newActions = change.GetNewValue<ActionCollection?>();

            if (oldActions is not null && IsInitialized)
            {
                DetachActionsFromLogicalTree(oldActions);
            }

            if (newActions is not null && IsInitialized)
            {
                AttachActionsToLogicalTree(newActions);
            }
        }
    }

    internal override void Initialize()
    {
        base.Initialize();

        InitializeActions(Actions);
    }

    internal override void AttachBehaviorToLogicalTree()
    {
        base.AttachBehaviorToLogicalTree();

        AttachActionsToLogicalTree(Actions);
    }

    internal override void DetachBehaviorFromLogicalTree()
    {
        DetachActionsFromLogicalTree(Actions);

        base.DetachBehaviorFromLogicalTree();
    }

    private void AttachActionsToLogicalTree(ActionCollection? actions)
    {
        if (actions is null)
        {
            return;
        }
        
        StyledElement? parent;
            
        if (AssociatedObject is TopLevel topLevel)
        {
            parent = topLevel;
        }
        else
        {
            if (AssociatedObject is not StyledElement styledElement || styledElement.Parent is null)
            {
                return;
            }

            parent = this;
        }

        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.AttachActionToLogicalTree(parent);
            }
        }
    }

    private void DetachActionsFromLogicalTree(ActionCollection? actions)
    {
        if (actions is null)
        {
            return;
        }

        var parent = this;

        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.DetachActionFromLogicalTree(parent);
            }
        }
    }

    private void InitializeActions(ActionCollection? actions)
    {
        if (actions is null)
        {
            return;
        }

        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.Initialize();
            }
        }
    }
}
