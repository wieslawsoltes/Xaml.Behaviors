using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class InvokeCommandActionBaseTests
{
    [AvaloniaFact]
    public void CanExecuteCommand_DefaultsToTrueWithoutCommand()
    {
        var action = new TestInvokeCommandAction();
        var parent = new Border();

        action.AttachActionToLogicalTree(parent);

        Assert.True(action.CanExecuteCommand);

        action.DetachActionFromLogicalTree(parent);
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UpdatesWhenCommandRaisesCanExecuteChanged()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var action = new TestInvokeCommandAction
        {
            Command = command
        };
        var parent = new Border();

        action.AttachActionToLogicalTree(parent);
        Assert.False(action.CanExecuteCommand);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.True(action.CanExecuteCommand);

        action.DetachActionFromLogicalTree(parent);
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UsesCurrentCommandParameter()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => Equals(parameter, "enabled")
        };
        var action = new TestInvokeCommandAction
        {
            Command = command,
            CommandParameter = "disabled"
        };
        var parent = new Border();

        action.AttachActionToLogicalTree(parent);
        Assert.False(action.CanExecuteCommand);

        action.CommandParameter = "enabled";

        Assert.True(action.CanExecuteCommand);
        Assert.Equal("enabled", command.LastCanExecuteParameter);

        action.DetachActionFromLogicalTree(parent);
    }

    [AvaloniaFact]
    public void CanExecuteCommand_RewiresSubscriptionsWhenCommandChangesAndDetaches()
    {
        var firstCommand = new ObservableCommand { CanExecuteResult = false };
        var secondCommand = new ObservableCommand { CanExecuteResult = true };
        var action = new TestInvokeCommandAction
        {
            Command = firstCommand
        };
        var parent = new Border();

        action.AttachActionToLogicalTree(parent);

        Assert.Equal(1, firstCommand.SubscriptionCount);
        Assert.False(action.CanExecuteCommand);

        action.Command = secondCommand;

        Assert.Equal(0, firstCommand.SubscriptionCount);
        Assert.Equal(1, secondCommand.SubscriptionCount);
        Assert.True(action.CanExecuteCommand);

        action.DetachActionFromLogicalTree(parent);

        Assert.Equal(0, secondCommand.SubscriptionCount);
    }

    private sealed class TestInvokeCommandAction : InvokeCommandActionBase
    {
        public override object? Execute(object? sender, object? parameter)
        {
            return null;
        }
    }

    private sealed class ObservableCommand : ICommand
    {
        private EventHandler? _canExecuteChanged;

        public bool CanExecuteResult { get; set; } = true;

        public Func<object?, bool>? CanExecuteCallback { get; set; }

        public int SubscriptionCount { get; private set; }

        public object? LastCanExecuteParameter { get; private set; }

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                SubscriptionCount++;
            }
            remove
            {
                _canExecuteChanged -= value;
                SubscriptionCount--;
            }
        }

        public bool CanExecute(object? parameter)
        {
            LastCanExecuteParameter = parameter;
            return CanExecuteCallback?.Invoke(parameter) ?? CanExecuteResult;
        }

        public void Execute(object? parameter)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            _canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
