using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that exposes commands for drag-and-drop events.
/// </summary>
public sealed class DragDropCommandsBehavior : DragAndDropEventsBehavior
{
    private readonly CommandCanExecuteObserver _dragEnterCommandCanExecuteObserver;
    private readonly CommandCanExecuteObserver _dragOverCommandCanExecuteObserver;
    private readonly CommandCanExecuteObserver _dragLeaveCommandCanExecuteObserver;
    private readonly CommandCanExecuteObserver _dropCommandCanExecuteObserver;
    private bool _canExecuteDragEnterCommand = true;
    private bool _canExecuteDragOverCommand = true;
    private bool _canExecuteDragLeaveCommand = true;
    private bool _canExecuteDropCommand = true;
    private bool _passEventArgsToCommand = true;

    /// <summary>
    /// Identifies the <see cref="DragEnterCommand"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> DragEnterCommandProperty =
        AvaloniaProperty.Register<DragDropCommandsBehavior, ICommand?>(nameof(DragEnterCommand));

    /// <summary>
    /// Identifies the <see cref="DragOverCommand"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> DragOverCommandProperty =
        AvaloniaProperty.Register<DragDropCommandsBehavior, ICommand?>(nameof(DragOverCommand));

    /// <summary>
    /// Identifies the <see cref="DragLeaveCommand"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> DragLeaveCommandProperty =
        AvaloniaProperty.Register<DragDropCommandsBehavior, ICommand?>(nameof(DragLeaveCommand));

    /// <summary>
    /// Identifies the <see cref="DropCommand"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> DropCommandProperty =
        AvaloniaProperty.Register<DragDropCommandsBehavior, ICommand?>(nameof(DropCommand));

    /// <summary>
    /// Identifies the <see cref="CanExecuteCommandParameter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> CanExecuteCommandParameterProperty =
        AvaloniaProperty.Register<DragDropCommandsBehavior, object?>(nameof(CanExecuteCommandParameter));

    /// <summary>
    /// Identifies the <see cref="CanExecuteDragEnterCommand"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<DragDropCommandsBehavior, bool> CanExecuteDragEnterCommandProperty =
        AvaloniaProperty.RegisterDirect<DragDropCommandsBehavior, bool>(
            nameof(CanExecuteDragEnterCommand),
            behavior => behavior.CanExecuteDragEnterCommand);

    /// <summary>
    /// Identifies the <see cref="CanExecuteDragOverCommand"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<DragDropCommandsBehavior, bool> CanExecuteDragOverCommandProperty =
        AvaloniaProperty.RegisterDirect<DragDropCommandsBehavior, bool>(
            nameof(CanExecuteDragOverCommand),
            behavior => behavior.CanExecuteDragOverCommand);

    /// <summary>
    /// Identifies the <see cref="CanExecuteDragLeaveCommand"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<DragDropCommandsBehavior, bool> CanExecuteDragLeaveCommandProperty =
        AvaloniaProperty.RegisterDirect<DragDropCommandsBehavior, bool>(
            nameof(CanExecuteDragLeaveCommand),
            behavior => behavior.CanExecuteDragLeaveCommand);

    /// <summary>
    /// Identifies the <see cref="CanExecuteDropCommand"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<DragDropCommandsBehavior, bool> CanExecuteDropCommandProperty =
        AvaloniaProperty.RegisterDirect<DragDropCommandsBehavior, bool>(
            nameof(CanExecuteDropCommand),
            behavior => behavior.CanExecuteDropCommand);

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
    public ICommand? DragEnterCommand
    {
        get => GetValue(DragEnterCommandProperty);
        set => SetValue(DragEnterCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked on drag over.
    /// </summary>
    public ICommand? DragOverCommand
    {
        get => GetValue(DragOverCommandProperty);
        set => SetValue(DragOverCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked on drag leave.
    /// </summary>
    public ICommand? DragLeaveCommand
    {
        get => GetValue(DragLeaveCommandProperty);
        set => SetValue(DragLeaveCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked on drop.
    /// </summary>
    public ICommand? DropCommand
    {
        get => GetValue(DropCommandProperty);
        set => SetValue(DropCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter used to evaluate the drag-and-drop commands before event-specific parameters are available.
    /// This property does not change the parameter passed to command execution.
    /// </summary>
    public object? CanExecuteCommandParameter
    {
        get => GetValue(CanExecuteCommandParameterProperty);
        set => SetValue(CanExecuteCommandParameterProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="DragEnterCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    public bool CanExecuteDragEnterCommand
    {
        get => _canExecuteDragEnterCommand;
        private set => SetAndRaise(CanExecuteDragEnterCommandProperty, ref _canExecuteDragEnterCommand, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="DragOverCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    public bool CanExecuteDragOverCommand
    {
        get => _canExecuteDragOverCommand;
        private set => SetAndRaise(CanExecuteDragOverCommandProperty, ref _canExecuteDragOverCommand, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="DragLeaveCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    public bool CanExecuteDragLeaveCommand
    {
        get => _canExecuteDragLeaveCommand;
        private set => SetAndRaise(CanExecuteDragLeaveCommandProperty, ref _canExecuteDragLeaveCommand, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="DropCommand"/> can execute with the current can-execute parameter.
    /// </summary>
    public bool CanExecuteDropCommand
    {
        get => _canExecuteDropCommand;
        private set => SetAndRaise(CanExecuteDropCommandProperty, ref _canExecuteDropCommand, value);
    }

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
