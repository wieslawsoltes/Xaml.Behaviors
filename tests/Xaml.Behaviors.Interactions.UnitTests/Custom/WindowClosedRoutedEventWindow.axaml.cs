using System;
using System.Windows.Input;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public partial class WindowClosedRoutedEventWindow : Window
{
    public WindowClosedRoutedEventWindow()
    {
        InitializeComponent();
    }
}

public class WindowClosedBindingSource
{
    public WindowClosedBindingSource()
    {
        CloseCommand = new CountingCommand();
    }

    public CountingCommand CloseCommand { get; }
}

public class CountingCommand : ICommand
{
    public int ExecutionCount { get; private set; }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        ExecutionCount++;
    }
}
