using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class TestControl : Control
{
    public bool MethodCalled { get; private set; }
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
