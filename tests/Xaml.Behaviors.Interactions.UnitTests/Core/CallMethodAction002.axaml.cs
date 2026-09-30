using System;
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

public partial class CallMethodAction002 : Window
{
    public string? TestProperty { get; set; }
    
    public object? Sender { get; set; }

    public EventArgs? Args { get; set; }

    public CallMethodAction002()
    {
        InitializeComponent();
    }

    public void TestMethod(object? sender, EventArgs args)
    {
        TestProperty = "Test String";
        Sender = sender;
        Args = args;
    }
}
