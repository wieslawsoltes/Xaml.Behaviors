using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public abstract class IfElseActionBase : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<IfElseActionBase, object?>(nameof(Binding));

    /// <summary>
    /// Identifies the <seealso cref="ComparisonCondition"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> ComparisonConditionProperty =
        AvaloniaProperty.Register<IfElseActionBase, ComparisonConditionType>(nameof(ComparisonCondition));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<IfElseActionBase, object?>(nameof(Value));

    /// <summary>
    /// Identifies the <seealso cref="Actions"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<IfElseActionBase, ActionCollection> ActionsProperty =
        AvaloniaProperty.RegisterDirect<IfElseActionBase, ActionCollection>(nameof(Actions), t => t.Actions);

    /// <summary>
    /// Gets or sets the bound object that the <see cref="IfElseActionBase"/> will listen to. This is an avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of comparison to be performed between <see cref="IfElseActionBase.Binding"/> and <see cref="IfElseActionBase.Value"/>. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType ComparisonCondition
    {
        get => GetValue(ComparisonConditionProperty);
        set => SetValue(ComparisonConditionProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to be compared with the value of <see cref="IfElseActionBase.Binding"/>. This is an avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private ActionCollection? _actions;

    /// <summary>
    /// Gets the collection of actions associated with the behavior. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection Actions => _actions ??= new ActionCollection();

    internal IfElseTriggerBehavior? ParentBehavior { get; set; }

    private IfElseActionBase? ParentAction { get; set; }

    internal event EventHandler? BindingChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="IfElseActionBase"/> class.
    /// </summary>
    protected IfElseActionBase()
    {
        Actions.CollectionChanged += Actions_CollectionChanged;
    }

    internal override void AttachActionToLogicalTree(StyledElement parent)
    {
        base.AttachActionToLogicalTree(parent);

        foreach (var action in Actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.AttachActionToLogicalTree(this);
            }
        }
    }

    internal override void DetachActionFromLogicalTree(StyledElement parent)
    {
        base.DetachActionFromLogicalTree(parent);

        foreach (var action in Actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.DetachActionFromLogicalTree(this);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
                
        if (change.Property == BindingProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == ComparisonConditionProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == ValueProperty)
        {
            OnValueChanged(change);
        }
    }

    private static void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not IfElseActionBase ifElseAction)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            ifElseAction.RaiseBindingChanged(args);
        });
    }

    private void RaiseBindingChanged(AvaloniaPropertyChangedEventArgs args)
    {
        BindingChanged?.Invoke(this, args);
        if (ParentBehavior is null)
        {
            ParentAction?.RaiseBindingChanged(args);
        }
    }

    private void Actions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<IfElseActionBase>())
            {
                item.ParentAction = this;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<IfElseActionBase>())
            {
                item.ParentAction = null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    public bool CanExecute()
    {
        var binding = Binding ?? GetParentBinding();
        return ComparisonConditionTypeHelper.Compare(binding, ComparisonCondition, Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private object? GetParentBinding()
    {
        if (ParentAction is null)
        {
            return ParentBehavior?.Binding;
        }

        var binding = ParentAction.Binding ?? ParentAction.GetParentBinding();
        return binding;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public override object Execute(object? sender, object? parameter)
    {
        return IfElseTriggerBehavior.ExecuteIfElseActions(Actions, sender, parameter);
    }
}
