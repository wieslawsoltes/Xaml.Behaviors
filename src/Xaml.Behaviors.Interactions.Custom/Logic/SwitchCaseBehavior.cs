using System.Collections.Specialized;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that executes a specific set of actions based on a value match.
/// </summary>
public partial class SwitchCaseBehavior : StyledElementTrigger
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
    public partial AvaloniaList<Case>? Cases { get; set; }

    /// <summary>
    /// Gets the actions to execute if no case matches.
    /// </summary>
    [StyledProperty]
    public partial ActionCollection? DefaultActions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchCaseBehavior"/> class.
    /// </summary>
    public SwitchCaseBehavior()
    {
        SetCurrentValue(CasesProperty, new AvaloniaList<Case>());
        SetCurrentValue(DefaultActionsProperty, new ActionCollection());
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            OnValueChanged(change);
        }
        else if (change.Property == CasesProperty)
        {
            var oldCases = change.GetOldValue<AvaloniaList<Case>?>();
            var newCases = change.GetNewValue<AvaloniaList<Case>?>();

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

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        // Property changes of this behavior are always raised on this instance.
        Dispatcher.UIThread.Post(() =>
        {
            Execute(parameter: args);
        });
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

    private void AttachCasesToLogicalTree(System.Collections.IList? cases)
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

    private void DetachCasesFromLogicalTree(System.Collections.IList? cases)
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

    /// <inheritdoc />
    protected override void OnInitializedEvent()
    {
        base.OnInitializedEvent();
        Execute(parameter: null);
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null || !IsEnabled)
        {
            return;
        }

        var value = Value;
        if (Cases is { } cases)
        {
            foreach (var c in cases)
            {
                if (Equals(c.Value, value))
                {
                    if (c.Actions is { } actions)
                    {
                        Interaction.ExecuteActions(AssociatedObject, actions, parameter);
                    }
                    return;
                }
            }
        }

        if (DefaultActions is { } defaultActions)
        {
            Interaction.ExecuteActions(AssociatedObject, defaultActions, parameter);
        }
    }
}
