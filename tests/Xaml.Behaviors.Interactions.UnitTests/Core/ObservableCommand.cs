using System;
using System.Windows.Input;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

internal sealed class ObservableCommand : ICommand
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
