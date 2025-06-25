using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class StubTextBlockBehavior : StyledElementBehavior<TextBlock>
{
    public int AttachCount { get; private set; }

    protected override void OnAttached()
    {
        AttachCount++;
        base.OnAttached();
    }
}

