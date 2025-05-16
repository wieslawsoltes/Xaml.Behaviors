using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class IfElseTriggerBehavior : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<IfElseTriggerBehavior, object?>(nameof(Binding));

    /// <summary>
    /// Gets or sets the bound object that the <see cref="IfElseTriggerBehavior"/> will listen to. This is an avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    static IfElseTriggerBehavior()
    {
        BindingProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<object?>>(OnValueChanged));
    }

    /// <summary>
    /// 
    /// </summary>
    public IfElseTriggerBehavior()
    {
        Actions.CollectionChanged += Actions_CollectionChanged;
    }

    private void Actions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var action in e.NewItems.OfType<IfElseActionBase>())
            {
                action.ParentBehavior = this;
                action.BindingChanged += Action_BindingChanged;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (var action in e.OldItems.OfType<IfElseActionBase>())
            {
                action.BindingChanged -= Action_BindingChanged;
                action.ParentBehavior = null;
            }
        }
    }

    private static void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not IfElseTriggerBehavior behavior || behavior.AssociatedObject is null)
        {
            return;
        }

        behavior.RaiseValueChanged(args);
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is StyledElement styledElement)
        {
            if (styledElement.IsInitialized)
            {
                RaiseValueChanged();
            }
            else
            {
                styledElement.Initialized += AssociatedObject_Initialized;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is StyledElement styledElement)
        {
            styledElement.Initialized -= AssociatedObject_Initialized;
        }
    }

    private void AssociatedObject_Initialized(object? sender, EventArgs e)
    {
        RaiseValueChanged();
    }

    private void Action_BindingChanged(object? sender, EventArgs e)
    {
        RaiseValueChanged();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    private void RaiseValueChanged(AvaloniaPropertyChangedEventArgs? args = null)
    {
        ExecuteIfElseActions(Actions, AssociatedObject, args);
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    internal static IEnumerable<object> ExecuteIfElseActions(ActionCollection actions, object? sender, object? args = null)
    {
        List<object> results = new();

        var currentState = false;

        foreach (var action in actions.OfType<IAction>())
        {
            // if it is "if-else" action, then we try to compute them
            if (action is not IfElseActionBase ifElseAction)
            {
                // if it is a usual action, then we perform it anyway
                ExecuteAction(action, sender, args);
                currentState = false;
            }
            else
            {
                switch (ifElseAction)
                {
                    case IfAction:
                    {
                        // "if" is always executed
                        var canExecute = ifElseAction.CanExecute();
                        if (canExecute)
                        {
                            ExecuteAction(action, sender, args);
                        }

                        currentState = canExecute;
                        break;
                    }

                    case ElseAction:
                    {
                        // "else if" is executed if the previous action failed
                        if (currentState)
                        {
                            break;
                        }

                        var canExecute = ifElseAction.CanExecute();
                        if (canExecute)
                        {
                            ExecuteAction(action, sender, args);
                        }

                        currentState = canExecute;
                        break;
                    }

                    case ElseIfAction:
                    {
                        // "else" executed if the previous action failed
                        if (currentState)
                        {
                            break;
                        }

                        // at the same time, the conditions are not checked in "else"
                        ExecuteAction(action, sender, args);

                        currentState = true;
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(ifElseAction));
                }
            }
        }

        return results;

        void ExecuteAction(IAction action, object? s, object? parameter)
        {
            var result = action.Execute(s, parameter);
            if (result is not null)
            {
                results.Add(result);
            }
        }
    }
}
