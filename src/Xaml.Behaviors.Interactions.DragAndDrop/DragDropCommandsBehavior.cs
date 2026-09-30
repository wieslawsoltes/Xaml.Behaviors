using System.Windows.Input;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Behavior that exposes commands for drag-and-drop events.
/// </summary>
public sealed partial class DragDropCommandsBehavior : DragAndDropEventsBehavior
{
    private readonly CommandCanExecuteObserver _dragEnterCommandCanExecuteObserver;
    private readonly CommandCanExecuteObserver _dragOverCommandCanExecuteObserver;
    private readonly CommandCanExecuteObserver _dragLeaveCommandCanExecuteObserver;
    private readonly CommandCanExecuteObserver _dropCommandCanExecuteObserver;
    private bool _passEventArgsToCommand = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DragDropCommandsBehavior"/> class.
    /// </summary>
    public DragDropCommandsBehavior()
    {
        _dragEnterCommandCanExecuteObserver = new CommandCanExecuteObserver(SetCanExecuteDragEnterCommand);
        _dragOverCommandCanExecuteObserver = new CommandCanExecuteObserver(SetCanExecuteDragOverCommand);
        _dragLeaveCommandCanExecuteObserver = new CommandCanExecuteObserver(SetCanExecuteDragLeaveCommand);
        _dropCommandCanExecuteObserver = new CommandCanExecuteObserver(SetCanExecuteDropCommand);
    }

    /// <summary>
    /// Gets or sets the command invoked on drag enter.
    /// </summary>
    [StyledProperty]
    public partial ICommand? DragEnterCommand { get; set; }

    /// <summary>
    /// Gets or sets the command invoked on drag over.
    /// </summary>
    [StyledProperty]
    public partial ICommand? DragOverCommand { get; set; }

    /// <summary>
    /// Gets or sets the command invoked on drag leave.
    /// </summary>
    [StyledProperty]
    public partial ICommand? DragLeaveCommand { get; set; }

    /// <summary>
    /// Gets or sets the command invoked on drop.
    /// </summary>
    [StyledProperty]
    public partial ICommand? DropCommand { get; set; }

    /// <summary>
    /// Gets or sets the parameter used to evaluate the drag-and-drop commands before event-specific parameters are available.
    /// This property does not change the parameter passed to command execution.
    /// </summary>
    [StyledProperty]
    public partial object? CanExecuteCommandParameter { get; set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="DragEnterCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecuteDragEnterCommand { get; private set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="DragOverCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecuteDragOverCommand { get; private set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="DragLeaveCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecuteDragLeaveCommand { get; private set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="DropCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecuteDropCommand { get; private set; }

    /// <summary>
    /// Specifies whether the event args should be passed to the command.
    /// </summary>
    public bool PassEventArgsToCommand
    {
        get => _passEventArgsToCommand;
        set
        {
            if (_passEventArgsToCommand == value)
            {
                return;
            }

            _passEventArgsToCommand = value;
            UpdateCanExecuteObservers();
        }
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        StartCanExecuteObservers();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        _dragEnterCommandCanExecuteObserver.Stop();
        _dragOverCommandCanExecuteObserver.Stop();
        _dragLeaveCommandCanExecuteObserver.Stop();
        _dropCommandCanExecuteObserver.Stop();

        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DragEnterCommandProperty)
        {
            _dragEnterCommandCanExecuteObserver.Update(
                DragEnterCommand,
                ResolveCanExecuteParameter(),
                IsCanExecuteParameterKnown());
        }
        else if (change.Property == DragOverCommandProperty)
        {
            _dragOverCommandCanExecuteObserver.Update(
                DragOverCommand,
                ResolveCanExecuteParameter(),
                IsCanExecuteParameterKnown());
        }
        else if (change.Property == DragLeaveCommandProperty)
        {
            _dragLeaveCommandCanExecuteObserver.Update(
                DragLeaveCommand,
                ResolveCanExecuteParameter(),
                IsCanExecuteParameterKnown());
        }
        else if (change.Property == DropCommandProperty)
        {
            _dropCommandCanExecuteObserver.Update(
                DropCommand,
                ResolveCanExecuteParameter(),
                IsCanExecuteParameterKnown());
        }
        else if (change.Property == CanExecuteCommandParameterProperty)
        {
            UpdateCanExecuteObservers();
        }
    }

    private void ExecuteCommand(ICommand? command, DragEventArgs e)
    {
        if (command is null)
        {
            return;
        }

        var parameter = PassEventArgsToCommand ? (object?)e : null;

        if (command.CanExecute(parameter))
        {
            command.Execute(parameter);
        }
    }

    /// <inheritdoc />
    protected override void OnDragEnter(object? sender, DragEventArgs e) => ExecuteCommand(DragEnterCommand, e);

    /// <inheritdoc />
    protected override void OnDragOver(object? sender, DragEventArgs e) => ExecuteCommand(DragOverCommand, e);

    /// <inheritdoc />
    protected override void OnDragLeave(object? sender, DragEventArgs e) => ExecuteCommand(DragLeaveCommand, e);

    /// <inheritdoc />
    protected override void OnDrop(object? sender, DragEventArgs e) => ExecuteCommand(DropCommand, e);

    private void StartCanExecuteObservers()
    {
        var parameter = ResolveCanExecuteParameter();
        var isParameterKnown = IsCanExecuteParameterKnown();

        _dragEnterCommandCanExecuteObserver.Start(DragEnterCommand, parameter, isParameterKnown);
        _dragOverCommandCanExecuteObserver.Start(DragOverCommand, parameter, isParameterKnown);
        _dragLeaveCommandCanExecuteObserver.Start(DragLeaveCommand, parameter, isParameterKnown);
        _dropCommandCanExecuteObserver.Start(DropCommand, parameter, isParameterKnown);
    }

    private void UpdateCanExecuteObservers()
    {
        var parameter = ResolveCanExecuteParameter();
        var isParameterKnown = IsCanExecuteParameterKnown();

        _dragEnterCommandCanExecuteObserver.Update(DragEnterCommand, parameter, isParameterKnown);
        _dragOverCommandCanExecuteObserver.Update(DragOverCommand, parameter, isParameterKnown);
        _dragLeaveCommandCanExecuteObserver.Update(DragLeaveCommand, parameter, isParameterKnown);
        _dropCommandCanExecuteObserver.Update(DropCommand, parameter, isParameterKnown);
    }

    private object? ResolveCanExecuteParameter()
    {
        return IsSet(CanExecuteCommandParameterProperty) ? CanExecuteCommandParameter : null;
    }

    private bool IsCanExecuteParameterKnown()
    {
        return IsSet(CanExecuteCommandParameterProperty) || !PassEventArgsToCommand;
    }

    private void SetCanExecuteDragEnterCommand(bool value)
    {
        CanExecuteDragEnterCommand = value;
    }

    private void SetCanExecuteDragOverCommand(bool value)
    {
        CanExecuteDragOverCommand = value;
    }

    private void SetCanExecuteDragLeaveCommand(bool value)
    {
        CanExecuteDragLeaveCommand = value;
    }

    private void SetCanExecuteDropCommand(bool value)
    {
        CanExecuteDropCommand = value;
    }
}
