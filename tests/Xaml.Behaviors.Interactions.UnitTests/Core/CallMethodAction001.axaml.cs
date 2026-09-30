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

public partial class CallMethodAction001 : Window
{
    public string? TestProperty { get; set; }
    
    public CallMethodAction001()
    {
        InitializeComponent();
    }

    public void TestMethod()
    {
        TestProperty = "Test String";
    }
}
