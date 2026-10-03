// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using System.Windows.Input;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
public abstract partial class ExecuteCommandBehaviorBase : AttachedToVisualTreeBehavior<Control>
{
    private readonly CommandCanExecuteObserver _commandCanExecuteObserver;

    /// <summary>
    /// 
    /// </summary>
#if UNO
    public static readonly AvaloniaProperty FocusControlProperty =
        AvaloniaProperty.Register(nameof(FocusControl), typeof(Control), typeof(ExecuteCommandBehaviorBase), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<Control?> FocusControlProperty =
        AvaloniaProperty.Register<ExecuteCommandBehaviorBase, Control?>(nameof(FocusControl));
#endif

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
#if UNO
    // WinUI has no top level element type: the root element to focus (defaults to XamlRoot.Content).
    [StyledProperty]
    public partial UIElement? TopLevel { get; set; }
#else
    [StyledProperty]
    public partial TopLevel? TopLevel { get; set; }
#endif
    
    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial ICommand? Command { get; set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="Command"/> can execute with the current <see cref="CommandParameter"/>.
    /// </summary>
    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecuteCommand { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial object? CommandParameter { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial bool FocusTopLevel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ResolveByName]
    public Control? FocusControl
    {
        get => (Control?)GetValue(FocusControlProperty);
        set => SetValue(FocusControlProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? SourceControl { get; set; }

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
#if UNO
            Dispatcher.UIThread.Post(() => (TopLevel ?? AssociatedObject?.XamlRoot?.Content)?.Focus());
#else
            Dispatcher.UIThread.Post(() => (TopLevel ?? AssociatedObject?.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.Focus());
#endif
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
