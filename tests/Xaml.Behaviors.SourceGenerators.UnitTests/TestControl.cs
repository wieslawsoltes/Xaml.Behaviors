using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

public class TestControl : Control
{
    public bool MethodCalled { get; internal set; }
    public string? MethodParameter { get; private set; }

    public void TestMethod()
    {
        MethodCalled = true;
    }

    public void TestMethodWithParameter(string parameter)
    {
        MethodCalled = true;
        MethodParameter = parameter;
    }

    public event EventHandler<RoutedEventArgs>? TestEvent;

    public void RaiseTestEvent()
    {
        TestEvent?.Invoke(this, new RoutedEventArgs());
    }
}
