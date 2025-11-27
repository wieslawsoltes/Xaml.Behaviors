using System;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class DataTriggerGeneratorTests
{
    [AvaloniaFact]
    public void StringDataTrigger_Should_Execute_Actions_When_Condition_Met()
    {
        var control = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("StringDataTrigger", "Xaml.Behaviors.Generated");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        
        trigger.Actions!.Add(action);
        trigger.Binding = "Match";
        trigger.Value = "Match";
        trigger.ComparisonCondition = ComparisonConditionType.Equal;
        
        trigger.Attach(control);
        
        // Trigger logic runs on property change.
        trigger.Binding = "NoMatch";
        trigger.Binding = "Match";
        
        Assert.True(control.MethodCalled);
    }

    [AvaloniaFact]
    public void StringDataTrigger_Should_Evaluate_On_Attach_When_Matching()
    {
        var control = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("StringDataTrigger", "Xaml.Behaviors.Generated");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        trigger.Actions!.Add(action);
        trigger.Binding = "Match";
        trigger.Value = "Match";
        trigger.ComparisonCondition = ComparisonConditionType.Equal;

        trigger.Attach(control);

        Assert.True(control.MethodCalled);
    }

    [Fact]
    public void Should_Handle_IComparable_Generic_Interface()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class GenericComparable : IComparable<GenericComparable>
    {
        public int Value { get; }
        public GenericComparable(int value) => Value = value;
        public int CompareTo(GenericComparable? other) => Value.CompareTo(other?.Value ?? 0);
    }
}

[assembly: GenerateTypedDataTrigger(typeof(TestNamespace.GenericComparable))]
";

        var (_, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(sources, s => s.Contains("System.IComparable<global::TestNamespace.GenericComparable>", StringComparison.Ordinal));
    }
}
