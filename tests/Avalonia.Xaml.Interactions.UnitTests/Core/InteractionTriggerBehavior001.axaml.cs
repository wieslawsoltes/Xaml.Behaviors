using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public partial class InteractionTriggerBehavior001 : Window
{
    public InteractionTriggerBehavior001()
    {
        InitializeComponent();
        DataContext = this;
    }

    public Interaction<Unit, Unit> TestInteraction { get; } = new();

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        TestInteraction.Handle(Unit.Default).Subscribe();
    }
}
