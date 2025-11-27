using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class PropertyTriggerRuntimeTests
{
    [AvaloniaFact]
    public void PropertyTrigger_Fires_When_Value_Matches()
    {
        var control = new RuntimePropertyHost();
        var target = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("RuntimePropertyHostFooPropertyTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = target;

        trigger.Actions!.Add(action);
        trigger.Value = "match";
        trigger.ComparisonCondition = ComparisonConditionType.Equal;

        trigger.Attach(control);

        control.Foo = "match";

        Assert.True(target.MethodCalled);
    }

    [AvaloniaFact]
    public void PropertyTrigger_Does_Not_Fire_When_Value_Does_Not_Match()
    {
        var control = new RuntimePropertyHost();
        var target = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("RuntimePropertyHostFooPropertyTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = target;

        trigger.Actions!.Add(action);
        trigger.Value = "match";
        trigger.ComparisonCondition = ComparisonConditionType.Equal;

        trigger.Attach(control);

        control.Foo = "different";

        Assert.False(target.MethodCalled);
    }

    [AvaloniaFact]
    public void PropertyTrigger_Evaluates_Null_On_Attach()
    {
        var control = new RuntimePropertyHost();
        var target = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("RuntimePropertyHostFooPropertyTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = target;

        trigger.Actions!.Add(action);
        trigger.Value = null;
        trigger.ComparisonCondition = ComparisonConditionType.Equal;

        trigger.Attach(control);

        Assert.True(target.MethodCalled);
    }
}
