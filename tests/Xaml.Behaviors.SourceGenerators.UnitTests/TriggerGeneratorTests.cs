using System;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class TriggerGeneratorTests
{
    [AvaloniaFact]
    public void TestEventTrigger_Should_Execute_Actions_On_Event()
    {
        var control = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("TestControlTestEventTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        
        trigger.Actions!.Add(action);
        trigger.Attach(control);
        
        control.RaiseTestEvent();
        
        Assert.True(control.MethodCalled);
    }

    [AvaloniaFact]
    public void Trigger_Should_Rewire_When_SourceObject_Changes()
    {
        var host = new TestControl();
        var source1 = new SourceTrackingControl();
        var source2 = new SourceTrackingControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("SourceTrackingControlSourceEventTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        trigger.Actions!.Add(action);
        trigger.SourceObject = source1;
        trigger.Attach(host);

        Assert.Equal(1, source1.SubscriptionCount);

        source1.Raise();
        Assert.True(host.MethodCalled);

        host.MethodCalled = false;
        trigger.SourceObject = source2;

        Assert.Equal(0, source1.SubscriptionCount);
        Assert.Equal(1, source2.SubscriptionCount);

        source1.Raise();
        Assert.False(host.MethodCalled);

        source2.Raise();
        Assert.True(host.MethodCalled);
    }

    [AvaloniaFact]
    public void Trigger_Should_Unsubscribe_When_Detached()
    {
        var host = new TestControl();
        var source = new SourceTrackingControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("SourceTrackingControlSourceEventTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        trigger.Actions!.Add(action);
        trigger.SourceObject = source;
        trigger.Attach(host);

        Assert.Equal(1, source.SubscriptionCount);

        trigger.Detach();
        source.Raise();
        Assert.Equal(0, source.SubscriptionCount);
    }
}
