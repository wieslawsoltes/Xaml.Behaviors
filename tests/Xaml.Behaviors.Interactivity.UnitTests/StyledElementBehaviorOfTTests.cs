using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class StyledElementBehaviorOfTTests
{
    [AvaloniaFact]
    public void Attach_WrongType_Throws()
    {
        var behavior = new StubTextBlockBehavior();
        var button = new Button();

        Assert.Throws<InvalidOperationException>(() => behavior.Attach(button));
    }

    [AvaloniaFact]
    public void Attach_CorrectType_Succeeds()
    {
        var behavior = new StubTextBlockBehavior();
        var textBlock = new TextBlock();

        behavior.Attach(textBlock);

        Assert.Equal(textBlock, behavior.AssociatedObject);
        Assert.Equal(1, behavior.AttachCount);
    }
}

