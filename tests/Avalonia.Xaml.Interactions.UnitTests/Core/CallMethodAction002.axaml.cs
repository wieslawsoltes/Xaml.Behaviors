using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Core;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public partial class CallMethodAction002 : Window
{
    public string? TestProperty { get; set; }
    
    public object? Sender { get; set; }

    public EventArgs? Args { get; set; }

    public CallMethodAction002()
    {
        InitializeComponent();
    }

    [GenerateInvoke]
    public void TestMethod(object? sender, EventArgs args)
    {
        TestProperty = "Test String";
        Sender = sender;
        Args = args;
    }
}
