using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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
        AvaloniaProperty.Register<IfElseActionBase, ComparisonConditionType>(nameof(ComparisonCondition), ComparisonConditionType.Equal);

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
    /// Gets or sets the bound object that the <see cref="IfElseActionBase"/> will listen to. This is a avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of comparison to be performed between <see cref="IfElseActionBase.Binding"/> and <see cref="IfElseActionBase.Value"/>. This is a avalonia property.
    /// </summary>
    public ComparisonConditionType ComparisonCondition
    {
        get => GetValue(ComparisonConditionProperty);
        set => SetValue(ComparisonConditionProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to be compared with the value of <see cref="IfElseActionBase.Binding"/>. This is a avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private ActionCollection? _actions;

    /// <summary>
    /// Gets the collection of actions associated with the behavior. This is a avalonia property.
    /// </summary>
    [Content]
    public ActionCollection Actions => _actions ??= new ActionCollection();

    internal IfElseBehavior? ParentBehavior { get; set; }
    internal IfElseActionBase? ParentAction { get; set; }

    internal event EventHandler? BindingChanged;

    static IfElseActionBase()
    {
        BindingProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<object?>>(OnBindingChanged));
    }

    public IfElseActionBase()
    {
        Actions.CollectionChanged += Actions_CollectionChanged;
    }

    private static void OnBindingChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not IfElseActionBase ifElseAction)
            return;

        ifElseAction.RaiseBindingChanged(args);
    }

    private void RaiseBindingChanged(AvaloniaPropertyChangedEventArgs args)
    {
        BindingChanged?.Invoke(this, args);
        if (ParentBehavior is null)
            ParentAction?.RaiseBindingChanged(args);
    }

    private void Actions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<IfElseActionBase>())
                item.ParentAction = this;
        }
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<IfElseActionBase>())
                item.ParentAction = null;
        }
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    public bool CanExecute() => Compare(Binding ?? GetParentBinding());
   
    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    protected virtual bool Compare(object? binding)
    {
        return ComparisonConditionTypeHelper.Compare(binding, ComparisonCondition, Value);
    }

    protected object? GetParentBinding()
    {
        if (ParentAction is not null)
        {
            return ParentAction.Binding ?? ParentAction.GetParentBinding();
        }

        return ParentBehavior?.Binding;
    }

    public override object? Execute(object? sender, object? parameter)
    {
        return IfElseBehavior.ExecuteIfElseActions(Actions, sender, parameter);
    }
}
