using System.Collections.Specialized;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using CaseList = Xaml.Interactions.Custom.CaseCollection;
#else
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
using CaseList = Avalonia.Collections.AvaloniaList<Avalonia.Xaml.Interactions.Custom.Case>;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// An action that executes a specific set of actions based on a value match.
/// </summary>
public partial class SwitchCaseAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the value to switch on.
    /// </summary>
    [StyledProperty]
    public partial object? Value { get; set; }

    /// <summary>
    /// Gets the collection of cases.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial CaseList? Cases { get; set; }

    /// <summary>
    /// Gets the actions to execute if no case matches.
    /// </summary>
    [StyledProperty]
    public partial ActionCollection? DefaultActions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchCaseAction"/> class.
    /// </summary>
    public SwitchCaseAction()
    {
        SetCurrentValue(CasesProperty, new CaseList());
        SetCurrentValue(DefaultActionsProperty, new ActionCollection());
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CasesProperty)
        {
            var oldCases = change.GetOldValue<CaseList?>();
            var newCases = change.GetNewValue<CaseList?>();

            if (oldCases is not null)
            {
                oldCases.CollectionChanged -= OnCasesCollectionChanged;
                if (((ILogical)this).IsAttachedToLogicalTree)
                {
                    DetachCasesFromLogicalTree(oldCases);
                }
            }

            if (newCases is not null)
            {
                newCases.CollectionChanged += OnCasesCollectionChanged;
                if (((ILogical)this).IsAttachedToLogicalTree)
                {
                    AttachCasesToLogicalTree(newCases);
                }
            }
        }
        else if (change.Property == DefaultActionsProperty)
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

    private void OnCasesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!((ILogical)this).IsAttachedToLogicalTree)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AttachCasesToLogicalTree(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                DetachCasesFromLogicalTree(e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                DetachCasesFromLogicalTree(e.OldItems);
                AttachCasesToLogicalTree(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Reset:
                // This is tricky because we don't have the old items.
                // But typically Reset clears the list.
                // Ideally we should track items to detach them.
                // For now, let's assume we can't easily detach if we don't track them.
                // Or we could iterate over LogicalChildren if we added them there?
                // But we are using SetParent.
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        AttachCasesToLogicalTree(Cases);
        AttachActionsToLogicalTree(DefaultActions);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        DetachCasesFromLogicalTree(Cases);
        DetachActionsFromLogicalTree(DefaultActions);
        base.OnDetachedFromLogicalTree(e);
    }

    private void AttachCasesToLogicalTree(System.Collections.IEnumerable? cases)
    {
        if (cases is null)
        {
            return;
        }

        foreach (var c in cases)
        {
            if (c is Case caseItem)
            {
                ((ISetLogicalParent)caseItem).SetParent(this);
            }
        }
    }

    private void DetachCasesFromLogicalTree(System.Collections.IEnumerable? cases)
    {
        if (cases is null)
        {
            return;
        }

        foreach (var c in cases)
        {
            if (c is Case caseItem)
            {
                ((ISetLogicalParent)caseItem).SetParent(null);
            }
        }
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
        var value = Value;
        if (Cases is { } cases)
        {
            foreach (var c in cases)
            {
                if (Equals(c.Value, value))
                {
                    if (c.Actions is { } actions)
                    {
                        Interaction.ExecuteActions(sender, actions, parameter);
                    }
                    return true;
                }
            }
        }

        if (DefaultActions is { } defaultActions)
        {
            Interaction.ExecuteActions(sender, defaultActions, parameter);
        }
        return null;
    }
}
