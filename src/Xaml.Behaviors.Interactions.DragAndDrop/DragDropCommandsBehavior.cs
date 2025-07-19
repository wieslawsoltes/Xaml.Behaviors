using System.Windows.Input;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that exposes commands for drag-and-drop events.
/// </summary>
public sealed class DragDropCommandsBehavior : DragAndDropEventsBehavior
{
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
    /// Specifies whether the event args should be passed to the command.
    /// </summary>
    public bool PassEventArgsToCommand { get; set; } = true;

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
}
