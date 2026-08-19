using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public partial class DataTriggerBehavior004 : Window
{
    public DataTriggerBehavior004()
    {
        InitializeComponent();
    }
}

public class DataTriggerBehavior004BindingSource : AvaloniaObject
{
    public static readonly StyledProperty<string?> TestPropertyProperty =
        AvaloniaProperty.Register<DataTriggerBehavior004BindingSource, string?>(nameof(TestProperty));

    public string? TestProperty
    {
        get => GetValue(TestPropertyProperty);
        set => SetValue(TestPropertyProperty, value);
    }
}
