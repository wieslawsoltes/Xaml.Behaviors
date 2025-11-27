using System;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class TestCommand : System.Windows.Input.ICommand
{
    public bool CanExecuteValue { get; set; } = true;
    public bool Executed { get; private set; }
    public object? ExecutedParameter { get; private set; }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => CanExecuteValue;

    public void Execute(object? parameter)
    {
        Executed = true;
        ExecutedParameter = parameter;
    }
    
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
