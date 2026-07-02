using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class CommandCanExecuteObserverTests
{
    [Fact]
    public void Constructor_ThrowsForNullCallback()
    {
        Assert.Throws<ArgumentNullException>(() => new CommandCanExecuteObserver(null!));
    }

    [Fact]
    public void Start_ReportsTrueWhenCommandIsNull()
    {
        var canExecute = false;
        var observer = new CommandCanExecuteObserver(value => canExecute = value);

        observer.Start(null, null);

        Assert.True(canExecute);
    }

    [Fact]
    public void Update_UsesCurrentCommandParameter()
    {
        var canExecute = false;
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => Equals(parameter, "enabled")
        };
        var observer = new CommandCanExecuteObserver(value => canExecute = value);

        observer.Start(command, "disabled");
        Assert.False(canExecute);

        observer.Update(command, "enabled");

        Assert.True(canExecute);
        Assert.Equal("enabled", command.LastCanExecuteParameter);
    }

    [Fact]
    public void Start_WithUnknownParameter_ReportsTrueWithoutCallingCanExecute()
    {
        var canExecute = false;
        var command = new ObservableCommand
        {
            CanExecuteCallback = _ => throw new InvalidOperationException("The parameter is not available yet.")
        };
        var observer = new CommandCanExecuteObserver(value => canExecute = value);

        observer.Start(command, null, false);

        Assert.True(canExecute);
        Assert.Equal(0, command.CanExecuteCallCount);
    }

    [Fact]
    public void CanExecuteChanged_WithUnknownParameter_DoesNotCallCanExecute()
    {
        var canExecute = false;
        var command = new ObservableCommand
        {
            CanExecuteCallback = _ => throw new InvalidOperationException("The parameter is not available yet.")
        };
        var observer = new CommandCanExecuteObserver(value => canExecute = value);

        observer.Start(command, null, false);
        command.RaiseCanExecuteChanged();

        Assert.True(canExecute);
        Assert.Equal(0, command.CanExecuteCallCount);
    }

    [Fact]
    public void Update_FromUnknownToKnownParameter_ProbesCurrentParameter()
    {
        var canExecute = false;
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => Equals(parameter, "enabled")
        };
        var observer = new CommandCanExecuteObserver(value => canExecute = value);

        observer.Start(command, null, false);
        observer.Update(command, "enabled", true);

        Assert.True(canExecute);
        Assert.Equal("enabled", command.LastCanExecuteParameter);
        Assert.Equal(1, command.CanExecuteCallCount);
    }

    [Fact]
    public void Update_RewiresCommandSubscriptions()
    {
        var canExecute = false;
        var firstCommand = new ObservableCommand { CanExecuteResult = false };
        var secondCommand = new ObservableCommand { CanExecuteResult = true };
        var observer = new CommandCanExecuteObserver(value => canExecute = value);

        observer.Start(firstCommand, null);

        Assert.Equal(1, firstCommand.SubscriptionCount);
        Assert.False(canExecute);

        observer.Update(secondCommand, null);

        Assert.Equal(0, firstCommand.SubscriptionCount);
        Assert.Equal(1, secondCommand.SubscriptionCount);
        Assert.True(canExecute);
    }

    [Fact]
    public void Dispose_RemovesCommandSubscription()
    {
        var command = new ObservableCommand();
        var observer = new CommandCanExecuteObserver(_ => { });

        observer.Start(command, null);
        Assert.Equal(1, command.SubscriptionCount);

        observer.Dispose();

        Assert.Equal(0, command.SubscriptionCount);
    }

    [Fact]
    public void CommandCanExecuteChangedSubscription_DoesNotRetainObserverOrCallbackTarget()
    {
        var command = new ObservableCommand();

        var references = CreateAndReleaseObserver(command);

        Assert.Equal(1, command.SubscriptionCount);
        AssertCollected(references.Observer, references.CallbackTarget);

        command.RaiseCanExecuteChangedWithNullSender();

        Assert.Equal(0, command.SubscriptionCount);
        GC.KeepAlive(command);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Observer, WeakReference CallbackTarget) CreateAndReleaseObserver(ObservableCommand command)
    {
        var callbackTarget = new CallbackTarget();
        var observer = new CommandCanExecuteObserver(callbackTarget.SetCanExecute);

        observer.Start(command, null);

        return (new WeakReference(observer), new WeakReference(callbackTarget));
    }

    private static void AssertCollected(params WeakReference[] references)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);

            if (Array.TrueForAll(references, reference => !reference.IsAlive))
            {
                return;
            }
        }

        Assert.All(references, reference => Assert.False(reference.IsAlive));
    }

    private sealed class CallbackTarget
    {
        public bool CanExecute { get; private set; }

        public void SetCanExecute(bool value)
        {
            CanExecute = value;
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

        public void RaiseCanExecuteChangedWithNullSender()
        {
            _canExecuteChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
