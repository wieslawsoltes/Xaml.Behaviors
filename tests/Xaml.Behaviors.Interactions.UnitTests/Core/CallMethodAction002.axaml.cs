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

#if WINUI
    // The event arguments of native WinUI are WinRT objects that do not derive from System.EventArgs.
    public RoutedEventArgs? Args { get; set; }
#else
    public EventArgs? Args { get; set; }
#endif

    public CallMethodAction002()
    {
        InitializeComponent();
    }

#if WINUI
    public void TestMethod(object? sender, RoutedEventArgs args)
#else
    public void TestMethod(object? sender, EventArgs args)
#endif
    {
        TestProperty = "Test String";
        Sender = sender;
        Args = args;
    }
}
