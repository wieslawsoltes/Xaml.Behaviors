// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public abstract class ExecuteCommandBehaviorBase : AttachedToVisualTreeBehavior<Control>
{
    private readonly CommandCanExecuteObserver _commandCanExecuteObserver;
    private bool _canExecuteCommand = true;

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
    /// Identifies the <seealso cref="CanExecuteCommand"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ExecuteCommandBehaviorBase, bool> CanExecuteCommandProperty =
        AvaloniaProperty.RegisterDirect<ExecuteCommandBehaviorBase, bool>(nameof(CanExecuteCommand), behavior => behavior.CanExecuteCommand);

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
    /// Initializes a new instance of the <see cref="ExecuteCommandBehaviorBase"/> class.
    /// </summary>
    protected ExecuteCommandBehaviorBase()
    {
        _commandCanExecuteObserver = new CommandCanExecuteObserver(SetCanExecuteCommand);
    }

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
    /// Gets a value indicating whether <see cref="Command"/> can execute with the current <see cref="CommandParameter"/>.
    /// </summary>
    public bool CanExecuteCommand
    {
        get => _canExecuteCommand;
        private set => SetAndRaise(CanExecuteCommandProperty, ref _canExecuteCommand, value);
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

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        _commandCanExecuteObserver.Start(Command, CommandParameter);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        _commandCanExecuteObserver.Stop();

        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CommandProperty || change.Property == CommandParameterProperty)
        {
            _commandCanExecuteObserver.Update(Command, CommandParameter);
        }
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

    private void SetCanExecuteCommand(bool value)
    {
        CanExecuteCommand = value;
    }
}
