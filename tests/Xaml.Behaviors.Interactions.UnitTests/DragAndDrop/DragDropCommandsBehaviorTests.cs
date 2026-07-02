using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Avalonia.Xaml.Interactions.UnitTests.Core;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;

public class DragDropCommandsBehaviorTests
{
    [AvaloniaFact]
    public void CanExecuteCommandProperties_UpdateWhenCommandsRaiseCanExecuteChanged()
    {
        var dragEnterCommand = new ObservableCommand { CanExecuteResult = false };
        var dragOverCommand = new ObservableCommand { CanExecuteResult = false };
        var dragLeaveCommand = new ObservableCommand { CanExecuteResult = false };
        var dropCommand = new ObservableCommand { CanExecuteResult = false };
        var behavior = new DragDropCommandsBehavior
        {
            DragEnterCommand = dragEnterCommand,
            DragOverCommand = dragOverCommand,
            DragLeaveCommand = dragLeaveCommand,
            DropCommand = dropCommand,
            PassEventArgsToCommand = false
        };
        var border = new Border();

        behavior.Attach(border);

        Assert.False(behavior.CanExecuteDragEnterCommand);
        Assert.False(behavior.CanExecuteDragOverCommand);
        Assert.False(behavior.CanExecuteDragLeaveCommand);
        Assert.False(behavior.CanExecuteDropCommand);

        dragEnterCommand.CanExecuteResult = true;
        dragOverCommand.CanExecuteResult = true;
        dragLeaveCommand.CanExecuteResult = true;
        dropCommand.CanExecuteResult = true;
        dragEnterCommand.RaiseCanExecuteChanged();
        dragOverCommand.RaiseCanExecuteChanged();
        dragLeaveCommand.RaiseCanExecuteChanged();
        dropCommand.RaiseCanExecuteChanged();

        Assert.True(behavior.CanExecuteDragEnterCommand);
        Assert.True(behavior.CanExecuteDragOverCommand);
        Assert.True(behavior.CanExecuteDragLeaveCommand);
        Assert.True(behavior.CanExecuteDropCommand);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommandProperties_RewireSubscriptionsWhenCommandsChangeAndDetach()
    {
        var firstDropCommand = new ObservableCommand { CanExecuteResult = false };
        var secondDropCommand = new ObservableCommand { CanExecuteResult = true };
        var behavior = new DragDropCommandsBehavior
        {
            DropCommand = firstDropCommand,
            PassEventArgsToCommand = false
        };
        var border = new Border();

        behavior.Attach(border);

        Assert.Equal(1, firstDropCommand.SubscriptionCount);
        Assert.False(behavior.CanExecuteDropCommand);

        behavior.DropCommand = secondDropCommand;

        Assert.Equal(0, firstDropCommand.SubscriptionCount);
        Assert.Equal(1, secondDropCommand.SubscriptionCount);
        Assert.True(behavior.CanExecuteDropCommand);

        behavior.Detach();

        Assert.Equal(0, secondDropCommand.SubscriptionCount);
    }

    [AvaloniaFact]
    public void CanExecuteCommandProperties_UseNullWhenEventArgsAreNotPassed()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => parameter is null
        };
        var behavior = new DragDropCommandsBehavior
        {
            DropCommand = command,
            PassEventArgsToCommand = false
        };
        var border = new Border();

        behavior.Attach(border);

        Assert.True(behavior.CanExecuteDropCommand);
        Assert.Null(command.LastCanExecuteParameter);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommandProperties_DeferWhenDragEventArgsAreUnavailable()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = _ => throw new System.InvalidOperationException("The drag event args are not available yet.")
        };
        var behavior = new DragDropCommandsBehavior
        {
            DropCommand = command
        };
        var border = new Border();

        behavior.Attach(border);

        Assert.True(behavior.CanExecuteDropCommand);
        Assert.Equal(0, command.CanExecuteCallCount);

        command.RaiseCanExecuteChanged();

        Assert.True(behavior.CanExecuteDropCommand);
        Assert.Equal(0, command.CanExecuteCallCount);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommandProperties_UseCanExecuteCommandParameterWhenEventArgsArePassed()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => Equals(parameter, "enabled")
        };
        var behavior = new DragDropCommandsBehavior
        {
            DropCommand = command,
            CanExecuteCommandParameter = "disabled"
        };
        var border = new Border();

        behavior.Attach(border);
        Assert.False(behavior.CanExecuteDropCommand);
        Assert.Equal("disabled", command.LastCanExecuteParameter);

        behavior.CanExecuteCommandParameter = "enabled";

        Assert.True(behavior.CanExecuteDropCommand);
        Assert.Equal("enabled", command.LastCanExecuteParameter);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void PassEventArgsToCommand_UpdatesCanExecuteParameterAvailability()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new DragDropCommandsBehavior
        {
            DropCommand = command
        };
        var border = new Border();

        behavior.Attach(border);
        Assert.True(behavior.CanExecuteDropCommand);
        Assert.Equal(0, command.CanExecuteCallCount);

        behavior.PassEventArgsToCommand = false;

        Assert.False(behavior.CanExecuteDropCommand);
        Assert.Equal(1, command.CanExecuteCallCount);

        behavior.Detach();
    }
}
