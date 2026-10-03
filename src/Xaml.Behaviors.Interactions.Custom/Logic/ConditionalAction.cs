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
/// An action that executes different collections of actions depending on the specified condition.
/// </summary>
public partial class ConditionalAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the condition that determines which actions are executed.
    /// </summary>
    [StyledProperty]
    public partial bool Condition { get; set; }

    /// <summary>
    /// Gets the actions executed when <see cref="Condition"/> evaluates to <c>true</c>.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial ActionCollection? Actions { get; set; }

    /// <summary>
    /// Gets the actions executed when <see cref="Condition"/> evaluates to <c>false</c>.
    /// </summary>
    [StyledProperty]
    public partial ActionCollection? ElseActions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalAction"/> class.
    /// </summary>
    public ConditionalAction()
    {
        SetCurrentValue(ActionsProperty, new ActionCollection());
        SetCurrentValue(ElseActionsProperty, new ActionCollection());
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
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        AttachActionsToLogicalTree(Actions);
        AttachActionsToLogicalTree(ElseActions);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        DetachActionsFromLogicalTree(Actions);
        DetachActionsFromLogicalTree(ElseActions);
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

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>Returns null.</returns>
    public override object? Execute(object? sender, object? parameter)
    {
        if (Condition)
        {
            if (Actions is { } actions)
            {
                Interaction.ExecuteActions(sender, actions, parameter);
            }
        }
        else
        {
            if (ElseActions is { } elseActions)
            {
                Interaction.ExecuteActions(sender, elseActions, parameter);
            }
        }
        return null;
    }
}
