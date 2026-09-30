#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.Core;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

public partial class DataTriggerBehavior004 : Window
{
    public DataTriggerBehavior004()
    {
        InitializeComponent();
    }
}

public partial class DataTriggerBehavior004BindingSource : AvaloniaObject
{
#if UNO
    public static readonly DependencyProperty TestPropertyProperty =
        DependencyProperty.Register(nameof(TestProperty), typeof(string), typeof(DataTriggerBehavior004BindingSource), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<string?> TestPropertyProperty =
        AvaloniaProperty.Register<DataTriggerBehavior004BindingSource, string?>(nameof(TestProperty));
#endif

    public string? TestProperty
    {
        get => (string?)GetValue(TestPropertyProperty);
        set => SetValue(TestPropertyProperty, value);
    }
}
