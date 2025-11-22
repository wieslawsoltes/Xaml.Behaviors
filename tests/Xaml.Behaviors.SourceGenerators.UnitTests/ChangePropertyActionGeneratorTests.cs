using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class ChangePropertyActionGeneratorTests
{
    [AvaloniaFact]
    public void SetTagAction_Should_Set_Property()
    {
        var control = new TestControl();
        dynamic action = GeneratedTypeHelper.CreateInstance("SetTagAction");
        action.Value = "TagValue";
        
        action.Execute(control, null);
        
        Assert.Equal("TagValue", control.Tag);
    }
}
