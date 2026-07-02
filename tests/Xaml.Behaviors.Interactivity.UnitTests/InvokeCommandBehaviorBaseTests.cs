using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class InvokeCommandBehaviorBaseTests
{
    [AvaloniaFact]
    public void CanExecuteCommand_DefaultsToTrueWithoutCommand()
    {
        var behavior = new TestInvokeCommandBehavior();
        var button = new Button();

        behavior.Attach(button);

        Assert.True(behavior.CanExecuteCommand);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UpdatesWhenCommandRaisesCanExecuteChanged()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command
        };
        var button = new Button();

        behavior.Attach(button);
        Assert.False(behavior.CanExecuteCommand);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.True(behavior.CanExecuteCommand);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UsesCurrentCommandParameter()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => Equals(parameter, "enabled")
        };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command,
            CommandParameter = "disabled"
        };
        var button = new Button();

        behavior.Attach(button);
        Assert.False(behavior.CanExecuteCommand);

        behavior.CommandParameter = "enabled";

        Assert.True(behavior.CanExecuteCommand);
        Assert.Equal("enabled", command.LastCanExecuteParameter);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UsesNullWhenCommandParameterIsUnset()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => parameter is null
        };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.True(behavior.CanExecuteCommand);
        Assert.Null(command.LastCanExecuteParameter);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_RewiresSubscriptionsWhenCommandChangesAndDetaches()
    {
        var firstCommand = new ObservableCommand { CanExecuteResult = false };
        var secondCommand = new ObservableCommand { CanExecuteResult = true };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = firstCommand
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.Equal(1, firstCommand.SubscriptionCount);
        Assert.False(behavior.CanExecuteCommand);

        behavior.Command = secondCommand;

        Assert.Equal(0, firstCommand.SubscriptionCount);
        Assert.Equal(1, secondCommand.SubscriptionCount);
        Assert.True(behavior.CanExecuteCommand);

        behavior.Detach();

        Assert.Equal(0, secondCommand.SubscriptionCount);
    }

    private sealed class TestInvokeCommandBehavior : InvokeCommandBehaviorBase;

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
