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
            DropCommand = dropCommand
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
            DropCommand = firstDropCommand
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
    public void CanExecuteCommandProperties_UseNullBeforeDragEventArgsAreAvailable()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => parameter is null
        };
        var behavior = new DragDropCommandsBehavior
        {
            DropCommand = command
        };
        var border = new Border();

        behavior.Attach(border);

        Assert.True(behavior.CanExecuteDropCommand);
        Assert.Null(command.LastCanExecuteParameter);

        behavior.Detach();
    }
}
