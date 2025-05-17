using Avalonia.Controls;

using Avalonia.Xaml.Interactions.Core;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public partial class CallMethodAction001 : Window
{
    public string? TestProperty { get; set; }
    
    public CallMethodAction001()
    {
        InitializeComponent();
    }

    [GenerateInvoke]
    public void TestMethod()
    {
        TestProperty = "Test String";
    }
}
