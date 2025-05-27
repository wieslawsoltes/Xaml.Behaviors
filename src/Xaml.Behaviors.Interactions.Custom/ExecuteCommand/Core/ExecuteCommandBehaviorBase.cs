// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public abstract class ExecuteCommandBehaviorBase : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<TopLevel?> TopLevelProperty =
        AvaloniaProperty.Register<ExecuteCommandBehaviorBase, TopLevel?>(nameof(TopLevel));
    
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ExecuteCommandBehaviorBase, ICommand?>(nameof(Command));

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ExecuteCommandBehaviorBase, object?>(nameof(CommandParameter));

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<bool> FocusTopLevelProperty =
        AvaloniaProperty.Register<ExecuteCommandBehaviorBase, bool>(nameof(FocusTopLevel));

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<Control?> FocusControlProperty =
        AvaloniaProperty.Register<ExecuteCommandBehaviorBase, Control?>(nameof(CommandParameter));
 
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<Control?> SourceControlProperty =
        AvaloniaProperty.Register<ExecuteCommandBehaviorBase, Control?>(nameof(SourceControl));

    /// <summary>
    /// 
    /// </summary>
    public TopLevel? TopLevel
    {
        get => GetValue(TopLevelProperty);
        set => SetValue(TopLevelProperty, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool FocusTopLevel
    {
        get => GetValue(FocusTopLevelProperty);
        set => SetValue(FocusTopLevelProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [ResolveByName]
    public Control? FocusControl
    {
        get => GetValue(FocusControlProperty);
        set => SetValue(FocusControlProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [ResolveByName]
    public Control? SourceControl
    {
        get => GetValue(SourceControlProperty);
        set => SetValue(SourceControlProperty, value);
    }


    /// <summary>
    /// Executes the associated command.
    /// </summary>
    /// <returns>True if the command was executed; otherwise, false.</returns>
    protected virtual bool ExecuteCommand()
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (AssociatedObject is not { IsVisible: true, IsEnabled: true })
        {
            return false;
        }

        if (Command?.CanExecute(CommandParameter) != true)
        {
            return false;
        }

        if (FocusTopLevel)
        {
            Dispatcher.UIThread.Post(() => (TopLevel ?? AssociatedObject?.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.Focus());
        }

        if (FocusControl is { } focusControl)
        {
            Dispatcher.UIThread.Post(() => focusControl.Focus());
        }

        Command.Execute(CommandParameter);
        return true;
    }
}
