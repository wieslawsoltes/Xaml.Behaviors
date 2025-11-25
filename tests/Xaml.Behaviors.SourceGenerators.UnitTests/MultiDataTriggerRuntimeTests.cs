using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class MultiDataTriggerRuntimeTests
{
    [AvaloniaFact]
    public void MultiDataTrigger_Evaluates_When_Attached_With_Preexisting_Values()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("TypedMultiDataTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);

        trigger.Value1 = "A";
        trigger.Value2 = "B";

        trigger.Attach(new TestControl());
        trigger.Detach();
        action.SeenParameters.Clear();

        trigger.Attach(new TestControl());

        Assert.Single(action.SeenParameters);
    }
}
