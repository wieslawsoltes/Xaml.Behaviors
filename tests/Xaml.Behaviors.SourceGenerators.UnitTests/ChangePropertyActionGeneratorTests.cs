using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class ChangePropertyActionGeneratorTests
{
    [AvaloniaFact]
    public void SetTagAction_Should_Set_Property()
    {
        var control = new TestControl();
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlSetTagAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.Value = "TagValue";
        
        action.Execute(control, null);
        
        Assert.Equal("TagValue", control.Tag);
    }

    [Fact]
    public void ChangePropertyAction_Should_Handle_Global_Namespace()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

public class GlobalViewModel
{
    [GenerateTypedChangePropertyAction]
    public string Name { get; set; } = string.Empty;
}
";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
    }
}
