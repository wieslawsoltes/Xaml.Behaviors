using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class IfElseBehavior : StyledElementTrigger
{
    static IfElseBehavior()
    {
        BindingProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<object>>(OnValueChanged));
    }

    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<IfElseBehavior, object?>(nameof(Binding));

    /// <summary>
    /// Gets or sets the bound object that the <see cref="IfElseBehavior"/> will listen to. This is a avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    public IfElseBehavior()
    {
        Actions.CollectionChanged += Actions_CollectionChanged;
    }

    private void Actions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<IfElseActionBase>())
            {
                item.ParentBehavior = this;
                item.BindingChanged += Item_BindingChanged;
            }
        }
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<IfElseActionBase>())
            {
                item.BindingChanged -= Item_BindingChanged;
                item.ParentBehavior = null;
            }
        }
    }

    private static void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not IfElseBehavior behavior || behavior.AssociatedObject is null)
            return;

        behavior.RaiseValueChanged(args);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is StyledElement styled)
        {
            if (styled.IsInitialized)
                Init();
            else
                styled.Initialized += Styled_Initialized;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is StyledElement styled)
            styled.Initialized -= Styled_Initialized;
    }

    private void Styled_Initialized(object? sender, System.EventArgs e)
    {
        Init();
    }

    private void Init() => RaiseValueChanged();

    private void Item_BindingChanged(object? sender, System.EventArgs e)
    {
        RaiseValueChanged();
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    protected void RaiseValueChanged() => RaiseValueChanged(null);

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    protected void RaiseValueChanged(AvaloniaPropertyChangedEventArgs? args)
    {
        ExecuteIfElseActions(Actions, AssociatedObject, args);
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    internal static IEnumerable<object> ExecuteIfElseActions(IEnumerable collection, object? sender, object? args = null)
    {
        List<object> results = new();

        object? ExecuteAction(IAction action, object? sender, object? parameter)
        {
            var result = action.Execute(sender, parameter);
            if (result is not null)
            {
                results.Add(result);
            }
            return result;
        }

        bool currentState = false;
        foreach (var action in collection.OfType<IAction>())
        {
            // if it is "if-else" action, then we try to compute them
            if (action is IfElseActionBase ifElseAction)
            {
                switch (ifElseAction.ConditionType)
                {
                    case ConditionType.If:
                    {
                        // "if" is always executed
                        bool canExecute = ifElseAction.CanExecute();
                        if (canExecute)
                            ExecuteAction(action, sender, args);

                        currentState = canExecute;
                        break;
                    }

                    case ConditionType.ElseIf:
                    {
                        // "else if" executed if the previous action failed
                        if (currentState)
                            break;

                        bool canExecute = ifElseAction.CanExecute();
                        if (canExecute)
                            ExecuteAction(action, sender, args);

                        currentState = canExecute;
                        break;
                    }

                    case ConditionType.Else:
                    {
                        // "else" executed if the previous action failed
                        if (currentState)
                            break;

                        // at the same time, the conditions are not checked in "else"
                        ExecuteAction(action, sender, args);

                        currentState = true;
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(ifElseAction.ConditionType));
                }
            }
            else // if it is a usual action, then we perform it anyway
            {
                ExecuteAction(action, sender, args);
                currentState = false;
            }
        }

        return results;
    }
}

