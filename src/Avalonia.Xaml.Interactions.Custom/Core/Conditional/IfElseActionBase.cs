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
        BindingProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<object>>(OnBindingChanged));
    }

    public IfElseActionBase()
    {
        Actions.CollectionChanged += Actions_CollectionChanged;
    }

    public ConditionType ConditionType { get; protected set; }

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
    protected virtual bool Compare(object? binding) => Compare(binding, ComparisonCondition, Value);

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

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    public static bool Compare(object? leftOperand, ComparisonConditionType operatorType, object? rightOperand)
    {
        if (leftOperand is not null && rightOperand is not null)
        {
            Type leftType = leftOperand.GetType();
            Type rightType = rightOperand.GetType();

            if (leftType != rightType)
            {
                // cast via converter
                var value = rightOperand.ToString();
                if (value is not null)
                {
                    try
                    {
                        rightOperand = TypeConverterHelper.Convert(value, leftType);
                        if (rightOperand is not null)
                            rightType = rightOperand.GetType();
                    }
                    catch
                    {
                        // nothing
                    }
                }
            }

            // compare via IComparable
            if (leftOperand is IComparable leftComparableOperand && rightOperand is IComparable rightComparableOperand)
            {
                if (leftType == rightType)
                    return EvaluateComparable(leftComparableOperand, operatorType, rightComparableOperand);
                else
                    return ConvertAndEvaluateComparable(leftComparableOperand, operatorType, rightComparableOperand);
            }
        }

        // compare links
        switch (operatorType)
        {
            case ComparisonConditionType.Equal:
                return Equals(leftOperand, rightOperand);

            case ComparisonConditionType.NotEqual:
                return !Equals(leftOperand, rightOperand);

            default:
                return false;
            //throw new InvalidOperationException();
        }
    }

    private static bool EvaluateComparable(IComparable leftOperand, ComparisonConditionType operatorType, IComparable rightOperand)
    {
        var comparison = leftOperand.CompareTo(rightOperand);
        return operatorType switch
        {
            ComparisonConditionType.Equal => comparison == 0,
            ComparisonConditionType.NotEqual => comparison != 0,
            ComparisonConditionType.LessThan => comparison < 0,
            ComparisonConditionType.LessThanOrEqual => comparison <= 0,
            ComparisonConditionType.GreaterThan => comparison > 0,
            ComparisonConditionType.GreaterThanOrEqual => comparison >= 0,
            _ => false
        };
    }

    private static bool ConvertAndEvaluateComparable(IComparable leftOperand, ComparisonConditionType operatorType, IComparable rightOperand)
    {
        object? convertedOperand = null;
        try
        {
            convertedOperand = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
        }
        catch (FormatException)
        {
            // FormatException: Convert.ChangeType("hello", typeof(double), ...);
        }
        catch (InvalidCastException)
        {
            // InvalidCastException: Convert.ChangeType(4.0d, typeof(Rectangle), ...);
        }
        catch
        {
            return false;
        }

        if (convertedOperand is null || convertedOperand is not IComparable comparableOperand)
        {
            return operatorType == ComparisonConditionType.NotEqual;
        }

        return EvaluateComparable(leftOperand, operatorType, comparableOperand);
    }

    
}
