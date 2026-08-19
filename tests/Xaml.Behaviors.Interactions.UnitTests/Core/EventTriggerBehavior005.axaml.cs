using System.Windows.Input;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public partial class EventTriggerBehavior005 : Window
{
    public EventTriggerBehavior005()
    {
        InitializeComponent();
    }
}

public sealed class FlyoutEventBindingSource
{
    public FlyoutEventBindingSource()
    {
        OpenedCommand = new Command(parameter =>
        {
            OpenedCount++;
            OpenedParameter = parameter;
        });
        ClosedCommand = new Command(parameter =>
        {
            ClosedCount++;
            ClosedParameter = parameter;
        });
    }

    public ICommand OpenedCommand { get; }

    public ICommand ClosedCommand { get; }

    public int OpenedCount { get; private set; }

    public int ClosedCount { get; private set; }

    public object? OpenedParameter { get; private set; }

    public object? ClosedParameter { get; private set; }
}
