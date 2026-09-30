#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that executes different collections of actions depending on the specified condition.
/// </summary>
public partial class ConditionalBehavior : StyledElementTrigger
{

    /// <summary>
    /// Gets or sets the condition that determines which actions are executed.
    /// </summary>
    [StyledProperty]
    public partial bool Condition { get; set; }

    /// <summary>
    /// Gets the actions executed when <see cref="Condition"/> evaluates to <c>false</c>.
    /// </summary>
    [StyledProperty]
    public partial ActionCollection? ElseActions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalBehavior"/> class.
    /// </summary>
    public ConditionalBehavior()
    {
        SetCurrentValue(ElseActionsProperty, new ActionCollection());
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ConditionProperty)
        {
            Execute(change.GetNewValue<bool>());
        }
        else if (change.Property == ElseActionsProperty)
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
    protected override void OnAttachedToLogicalTree()
    {
        base.OnAttachedToLogicalTree();
        AttachActionsToLogicalTree(ElseActions);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree()
    {
        DetachActionsFromLogicalTree(ElseActions);
        base.OnDetachedFromLogicalTree();
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
                styledElementAction.DetachActionFromLogicalTree(parent);
            }
        }
    }

    private void Execute(bool condition)
    {
        if (condition)
        {
            if (Actions is { } actions)
            {
                Interaction.ExecuteActions(AssociatedObject, actions, null);
            }
        }
        else
        {
            if (ElseActions is { } elseActions)
            {
                Interaction.ExecuteActions(AssociatedObject, elseActions, null);
            }
        }
    }
}
