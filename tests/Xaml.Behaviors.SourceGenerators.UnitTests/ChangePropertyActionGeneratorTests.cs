using Avalonia.Headless.XUnit;
using Xaml.Behaviors.Generated;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class ChangePropertyActionGeneratorTests
{
    [AvaloniaFact]
    public void SetTagAction_Should_Set_Property()
    {
        var control = new TestControl();
        var action = new SetTagAction();
        action.Value = "TagValue";
        
        action.Execute(control, null);
        
        Assert.Equal("TagValue", control.Tag);
    }
}
