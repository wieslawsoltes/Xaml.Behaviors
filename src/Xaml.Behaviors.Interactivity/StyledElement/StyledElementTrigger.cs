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
    public static readonly StyledProperty<ActionCollection> ActionsProperty =
        AvaloniaProperty.Register<StyledElementTrigger, ActionCollection>(nameof(Actions));

    /// <summary>
    /// Gets the collection of actions associated with the behavior. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection Actions
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

    internal override void Initialize()
    {
        base.Initialize();
        
        foreach (var action in Actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.Initialize();
            }
        }
    }

    internal override void AttachBehaviorToLogicalTree()
    {
        base.AttachBehaviorToLogicalTree();

        StyledElement? parent = null;
        
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

        foreach (var action in Actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.AttachActionToLogicalTree(parent);
            }
        }
    }

    internal override void DetachBehaviorFromLogicalTree()
    {
        var parent = this;

        foreach (var action in Actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.DetachActionFromLogicalTree(parent);
            }
        }

        base.DetachBehaviorFromLogicalTree();
    }
}
