using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
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
    public void UseCommandCanExecuteForIsEnabled_DefaultFalse_DoesNotUpdateAssociatedObjectIsEnabled()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.False(behavior.CanExecuteCommand);
        Assert.True(button.IsEnabled);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void UseCommandCanExecuteForIsEnabled_UpdatesAssociatedObjectIsEnabled()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command,
            UseCommandCanExecuteForIsEnabled = true
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.False(button.IsEnabled);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.True(button.IsEnabled);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void UseCommandCanExecuteForIsEnabled_PreservesExistingDisabledState()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command,
            UseCommandCanExecuteForIsEnabled = true
        };
        var button = new Button
        {
            IsEnabled = false
        };

        behavior.Attach(button);

        Assert.False(button.IsEnabled);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.False(button.IsEnabled);

        button.IsEnabled = true;

        Assert.True(button.IsEnabled);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void UseCommandCanExecuteForIsEnabled_ComposesWithExistingIsEnabledBinding()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command,
            UseCommandCanExecuteForIsEnabled = true
        };
        var isEnabled = new BooleanObservable(true);
        var button = new Button();
        button.Bind(InputElement.IsEnabledProperty, isEnabled);

        behavior.Attach(button);

        Assert.False(button.IsEnabled);

        isEnabled.Value = false;

        Assert.False(button.IsEnabled);

        isEnabled.Value = true;

        Assert.False(button.IsEnabled);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.True(button.IsEnabled);

        isEnabled.Value = false;

        Assert.False(button.IsEnabled);

        command.CanExecuteResult = false;
        command.RaiseCanExecuteChanged();

        Assert.False(button.IsEnabled);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.False(button.IsEnabled);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void UseCommandCanExecuteForIsEnabled_PropertyChange_AttachesAndDetachesBinding()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.True(button.IsEnabled);

        behavior.UseCommandCanExecuteForIsEnabled = true;

        Assert.False(button.IsEnabled);

        behavior.UseCommandCanExecuteForIsEnabled = false;

        Assert.True(button.IsEnabled);

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
    public void CanExecuteCommand_DefersWhenEventParameterIsUnavailable()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = _ => throw new InvalidOperationException("The event parameter is not available yet.")
        };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command,
            PassEventArgsToCommand = true,
            UseCommandCanExecuteForIsEnabled = true
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.True(behavior.CanExecuteCommand);
        Assert.True(button.IsEnabled);
        Assert.Equal(0, command.CanExecuteCallCount);

        command.RaiseCanExecuteChanged();

        Assert.True(behavior.CanExecuteCommand);
        Assert.True(button.IsEnabled);
        Assert.Equal(0, command.CanExecuteCallCount);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UsesSeparateCanExecuteCommandParameter()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => Equals(parameter, "enabled")
        };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command,
            PassEventArgsToCommand = true,
            CanExecuteCommandParameter = "disabled"
        };
        var button = new Button();

        behavior.Attach(button);
        Assert.False(behavior.CanExecuteCommand);
        Assert.Equal("disabled", command.LastCanExecuteParameter);

        behavior.CanExecuteCommandParameter = "enabled";

        Assert.True(behavior.CanExecuteCommand);
        Assert.Equal("enabled", command.LastCanExecuteParameter);
        Assert.Equal("event parameter", behavior.ResolveParameterForTest("event parameter"));

        behavior.Detach();
    }

    [AvaloniaFact]
    public void PassEventArgsToCommand_UpdatesCanExecuteParameterAvailability()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new TestInvokeCommandBehavior
        {
            Command = command
        };
        var button = new Button();

        behavior.Attach(button);
        Assert.False(behavior.CanExecuteCommand);
        Assert.Equal(1, command.CanExecuteCallCount);

        behavior.PassEventArgsToCommand = true;

        Assert.True(behavior.CanExecuteCommand);
        Assert.Equal(1, command.CanExecuteCallCount);

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

    private sealed class TestInvokeCommandBehavior : InvokeCommandBehaviorBase
    {
        public object? ResolveParameterForTest(object? parameter)
        {
            return ResolveParameter(parameter);
        }
    }

    private sealed class ObservableCommand : ICommand
    {
        private EventHandler? _canExecuteChanged;

        public bool CanExecuteResult { get; set; } = true;

        public Func<object?, bool>? CanExecuteCallback { get; set; }

        public int SubscriptionCount { get; private set; }

        public object? LastCanExecuteParameter { get; private set; }

        public int CanExecuteCallCount { get; private set; }

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
            CanExecuteCallCount++;
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

    private sealed class BooleanObservable : IObservable<bool>
    {
        private IObserver<bool>? _observer;
        private bool _value;

        public BooleanObservable(bool value)
        {
            _value = value;
        }

        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                _observer?.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            _observer = observer;
            observer.OnNext(_value);

            return new Subscription(this, observer);
        }

        private void Unsubscribe(IObserver<bool> observer)
        {
            if (ReferenceEquals(_observer, observer))
            {
                _observer = null;
            }
        }

        private sealed class Subscription : IDisposable
        {
            private readonly BooleanObservable _owner;
            private readonly IObserver<bool> _observer;
            private bool _disposed;

            public Subscription(BooleanObservable owner, IObserver<bool> observer)
            {
                _owner = owner;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _owner.Unsubscribe(_observer);
            }
        }
    }
}
